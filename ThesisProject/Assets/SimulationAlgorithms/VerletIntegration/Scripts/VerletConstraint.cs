using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VerletConstraint
{
    public VerletParticle particle1;
    public VerletParticle particle2;

    public float restingDistance;

    public InteractionType type;


    public VerletConstraint(ref VerletParticle particle1, ref VerletParticle particle2) {
        this.particle1 = particle1;
        this.particle2 = particle2;

        // Assume resting distance as the current distance between the particles
        restingDistance = (particle1.Position - particle2.Position).magnitude;
    }

    public VerletConstraint(ref VerletParticle particle1, ref VerletParticle particle2, float restingDistance) {
        this.particle1 = particle1;
        this.particle2 = particle2;

        this.restingDistance = restingDistance;
    }

    public void SatisfyConstraint() {
        // Correction vector from particle1 -> particle2
        Vector3 directionVector = particle2.Position - particle1.Position;
        Vector3 correctionVector = (1 - restingDistance / directionVector.magnitude) * directionVector;

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
}
