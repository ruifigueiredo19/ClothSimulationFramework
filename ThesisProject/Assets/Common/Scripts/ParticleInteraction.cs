using System;

public enum InteractionType { Stretch, Shear, Bend };


public class ParticleInteraction : IEquatable<ParticleInteraction> {
    public InteractionType interactionType;

    // Keep track of particles by indices
    public int particle1;
    public int particle2;
    public float equilibriumDistance;

    public ParticleInteraction(int particle1, int particle2, float equilibriumDistance, InteractionType interactionType) {
        this.interactionType = interactionType;

        this.particle1 = particle1;
        this.particle2 = particle2;
        this.equilibriumDistance = equilibriumDistance;
    }


    // Comparison Operators. They disregard equilibrium distance.
    public static bool operator ==(ParticleInteraction interactionA, ParticleInteraction interactionB) {
        return (interactionA.interactionType == interactionB.interactionType) &&
            ((interactionA.particle1 == interactionB.particle1 && interactionA.particle2 == interactionB.particle2) ||
            (interactionA.particle1 == interactionB.particle2 && interactionA.particle2 == interactionB.particle1));
    }

    public static bool operator !=(ParticleInteraction interactionA, ParticleInteraction interactionB) {
        return !(interactionA == interactionB);
    }


    public override bool Equals(object obj) {
        return Equals(obj as ParticleInteraction);
    }

    public bool Equals(ParticleInteraction other) {
        // Use == operator as particle1 and particle2 are interchangeable
        return other != null && this == other;
    }

    public override int GetHashCode() {
        int hashCode = -867283276;
        hashCode = hashCode * -1521134295 + interactionType.GetHashCode();
        hashCode = hashCode * -1521134295 + (particle1 + particle2).GetHashCode();
        return hashCode;
    }
}
