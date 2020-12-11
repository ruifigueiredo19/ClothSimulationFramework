using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletSimulator : BaseSimulator<VerletParticle> {
    // This uses a Verlet integrator to compute the position of the particles in the current frame, given the forces.
    readonly VerletConstants constants;

    public VerletSimulator(ref VerletConstants constants) {
        this.constants = constants;
    }


    // INITIAL CONDITIONS
    protected override void SetInitialConditions() {
        for (int i = 0; i < nParticles; i++) {
            particles[i] = new VerletParticle(initialPositions[i], simulationManager.particleMass) {
                index = i  //not required by simulation, just useful
            };
        }
    }

    protected override void InitialConditionDisplacement() {
        /*
        particles[0].isFixed = true;
        //*/

        /*
        // Top Triangle fixed
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3 - i; j++) {
                particles[j * targetManager.matrixSize.x + i].isFixed = true;
            }
        }
        // */

        /*
        // Top Edges on each side fixed
        for (int i = 0; i < 3; i++) {
            particles[i].isFixed = true;
            particles[targetManager.matrixSize.x - 1 - i].isFixed = true;
        }
        // */

        //*
        // Imported Cylinder
        int[] topParticlesIndices = { 20, 21, 22, 23, 24, 28, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90, 96, 102, 108, 114 };
        foreach (int i in topParticlesIndices) {
            particles[i].isFixed = true;
        }
        // */
    }

    // On interactions from the editor
    public override void ParticleWasDragged(int index, Vector3 newPosition) {
        particles[index].oldPosition = particles[index].Position;
        particles[index].ForceMoveParticle(newPosition);
        // Force it even if particle is fixed
    }

    // CONSTRAINTS
    void SatisfyConstraint(ParticleInteraction interaction) {
        VerletParticle particle1 = particles[interaction.particle1];
        VerletParticle particle2 = particles[interaction.particle2];

        // Correction vector from particle1 -> particle2
        Vector3 directionVector = particle2.Position - particle1.Position;
        Vector3 correctionVector = (1 - interaction.equilibriumDistance / directionVector.magnitude) * directionVector;

        // Apply half the correction to each particle (unless one is fixed)
        if (!particle1.isFixed && !particle2.isFixed) {
            particle1.Position += correctionVector / 2;
            particle2.Position -= correctionVector / 2;
        }
        else if (particle1.isFixed && !particle2.isFixed) {
            particle2.Position -= correctionVector;
        }
        else if (!particle1.isFixed && particle2.isFixed) {
            particle1.Position += correctionVector;
        }
    }


    // UPDATE STEPS
    void SatisfyConstraints() {

        // Using a reverse loop allows us to safely remove constraints from the list
        for (int interactionIndex = particleInteractions.Count - 1; interactionIndex >= 0; interactionIndex--) {
            ParticleInteraction interaction = particleInteractions[interactionIndex];

            // If tearing, check how far apart the particles are and maybe remove them from the list
            if (constants.enableTearing) {
                float magnitude = (particles[interaction.particle1].Position - particles[interaction.particle2].Position).magnitude;
                if (magnitude > interaction.equilibriumDistance * constants.maxTearingRatio) {
                    particleInteractions.RemoveAt(interactionIndex);
                    // Don't satisfy this constraint, since they it's broken now
                    continue;
                }
            }

            SatisfyConstraint(interaction);
        }
    }

    protected override void SimulateNextPhysicsStep() {
        AddExternalForces();

        // Update positions
        for (int i = 0; i < nParticles; i++) {
            particles[i].TimeStep(constants.dampingRatio);
        }

        // Constraints
        for (int i = 0; i < constants.constraintIterations; i++) {
            SatisfyConstraints();
        }
    }
}
