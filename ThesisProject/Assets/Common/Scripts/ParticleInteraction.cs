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

    public override bool Equals(object obj) {
        if (!(obj is ParticleInteraction other)) return false;
        return (interactionType == other.interactionType) &&
            ((particle1 == other.particle1 && particle2 == other.particle2) || (particle1 == other.particle2 && particle2 == other.particle1));
    }

    public bool Equals(ParticleInteraction other) {
        return (interactionType == other.interactionType) &&
            ((particle1 == other.particle1 && particle2 == other.particle2) || (particle1 == other.particle2 && particle2 == other.particle1));
    }

    public override int GetHashCode() {
        int hashCode = -867283276;
        hashCode = hashCode * -1521134295 + interactionType.GetHashCode();
        hashCode = hashCode * -1521134295 + (particle1 + particle2).GetHashCode();
        return hashCode;
    }
}
