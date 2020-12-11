public enum InteractionType { Stretch, Shear, Bend };


public class ParticleInteraction {
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
}
