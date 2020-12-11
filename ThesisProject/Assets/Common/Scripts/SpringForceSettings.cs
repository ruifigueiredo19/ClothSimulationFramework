[System.Serializable]
public class SpringForceSettings {
    public bool enabled;
    public float equilibriumDistance;
    public float forceConstant;

    // Keep these stored for easy resetting
    private bool defaultEnabled;
    private float defaultEquilibriumDistance;
    private float defaultForceConstant;

    public InteractionType type;


    // Default values are assumed as initial values unless specified
    public SpringForceSettings(bool enabled, float equilibriumDistance, float forceConstant) {
        this.enabled = defaultEnabled = enabled;
        this.equilibriumDistance = defaultEquilibriumDistance = equilibriumDistance;
        this.forceConstant = defaultForceConstant = forceConstant;
    }

    public SpringForceSettings(bool enabled, float equilibriumDistance, float forceConstant, bool defaultEnabled,
                                float defaultEquilibriumDistance, float defaultForceConstant) {

        this.enabled = enabled;
        this.equilibriumDistance = equilibriumDistance;
        this.forceConstant = forceConstant;

        this.defaultEnabled = defaultEnabled;
        this.defaultEquilibriumDistance = defaultEquilibriumDistance;
        this.defaultForceConstant = defaultForceConstant;
    }


    public void ResetDefaultValues() {
        enabled = defaultEnabled;
        equilibriumDistance = defaultEquilibriumDistance;
        forceConstant = defaultForceConstant;
    }

    public void SetDefaultValues(bool enabled, float equilibriumDistance, float forceConstant) {
        defaultEnabled = enabled;
        defaultEquilibriumDistance = equilibriumDistance;
        defaultForceConstant = forceConstant;
    }

    public void SetDefaultValues(float equilibriumDistance, float forceConstant) {
        defaultEnabled = enabled;
        defaultEquilibriumDistance = equilibriumDistance;
        defaultForceConstant = forceConstant;
    }
}
