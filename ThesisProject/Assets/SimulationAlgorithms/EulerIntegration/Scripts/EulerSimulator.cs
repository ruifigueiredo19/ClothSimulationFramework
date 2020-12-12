using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerSimulator : BaseSimulator<EulerParticle> {
    // This uses an Explicit integrator to compute the position of the particles in the current frame, given the forces.
    readonly EulerConstants constants;

    public EulerSimulator(ref EulerConstants constants) {
        this.constants = constants;
    }

    // INITIAL CONDITIONS
    protected override void SetInitialConditions() {
        for (int i = 0; i < nParticles; i++) {
            particles[i] = new EulerParticle(initialPositions[i], simulationManager.particleMass);
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

        /*
        // Imported Cylinder
        int[] topParticlesIndices = { 20, 21, 22, 23, 24, 28, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90, 96, 102, 108, 114 };
        foreach (int i in topParticlesIndices) {
            particles[i].isFixed = true;
        }
        // */
    }


    // On interactions from the editor
    public override void ParticleWasDragged(int index, Vector3 newPosition) {
        particles[index].velocity = (newPosition - particles[index].Position) / Time.deltaTime;
        particles[index].ForceMoveParticle(newPosition);   // Force it even if particle is fixed
        //particles[index].velocity = Vector3.zero;
    }

    // UPDATE STEPS
    void AddInternalForces() {
        for (int interactionIndex = particleInteractions.Count - 1; interactionIndex >= 0; interactionIndex--) {
            // Using a reverse loop allows us to safely remove items from the indices list
            ParticleInteraction interaction = particleInteractions[interactionIndex];
            SpringForceSettings springForceOfInteraction = simulationManager.GetSpringForce(interaction.interactionType);
            if (springForceOfInteraction.enabled) {
                EulerParticle particle1 = particles[interaction.particle1];
                EulerParticle particle2 = particles[interaction.particle2];

                Vector3 direction = particle1.Position - particle2.Position;
                float magnitude = direction.magnitude;

                // Cloth ripping
                if (constants.enableTearing && magnitude > interaction.equilibriumDistance * constants.maxTearingRatio) {
                    // Break the connection (we could do the same for the reverse one). No force added.
                    particleInteractions.RemoveAt(interactionIndex);
                    continue;
                }

                Vector3 force = direction / magnitude * (interaction.equilibriumDistance - magnitude) * springForceOfInteraction.forceConstant;
                Vector3 dampingForce = (particles[interaction.particle2].velocity - particles[interaction.particle1].velocity) * constants.springDampingConstant;

                particle1.AddForce(force + dampingForce);
            }
        }
    }

    void AddAirDampingForce() {
        for (int index = 0; index < nParticles; index++) {
            Vector3 dampingForce = -constants.airDampingConstant * particles[index].velocity;
            particles[index].AddForce(dampingForce);
        }
    }

    protected override void SimulateNextPhysicsStep() {
        // Calculate Internal and External Forces
        AddInternalForces();
        AddExternalForces();
        AddAirDampingForce();


        // Update positions
        for (int i = 0; i < nParticles; i++) {
            particles[i].TimeStep();
        }
    }
}
