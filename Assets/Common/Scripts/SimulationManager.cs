using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReferenceManager))]
public class SimulationManager : MonoBehaviour {

    // only instantiate when we need them
    EulerSimulator eulerSimulator;
    VerletSimulator verletSimulator;

    public enum SimulationMode { Euler, Verlet };
    [Header("General Settings")]
    public SimulationMode simulationMode = SimulationMode.Verlet;
    public float particleMass = 1f;
    public VerletConstants verletConstants;
    public EulerConstants eulerConstants;

    // These can be overwritten in each force
    [Header("Global Spring Force Settings")]
    public float globalEquilibriumDistance = 1f;
    public float globalForceConstant = 1f;

    [Header("Specific Spring Force Settings")]
    // Horizontal / Vertical forces
    public SpringForceSettings stretchForce = new SpringForceSettings(true, 1f, 5f) {
        type = InteractionType.Stretch
    };
    // Diagonal Forces
    public SpringForceSettings shearForce = new SpringForceSettings(true, Mathf.Sqrt(2), 0.5f) {
        type = InteractionType.Shear
    };
    // Horizal / Vertical that are 2 apart
    public SpringForceSettings bendForce = new SpringForceSettings(true, 2, 0.5f) {
        type = InteractionType.Bend
    };


    [Header("External Forces")]
    public GravityExternalForce gravityForce = new GravityExternalForce();
    public WindExternalForce windForce = new WindExternalForce();



    // Neighbours/Active Interactions
    public List<ParticleInteraction> ParticleInteractions {
        get {
            switch (simulationMode) {
                case SimulationMode.Verlet:
                    return verletSimulator.particleInteractions;
                case SimulationMode.Euler:
                    return eulerSimulator.particleInteractions;
                default:
                    return default;
            }
        }
    }

    // SPRING VALUE FUNCTIONS
    public void ResetSpringValues() {
        // Reset all spring forces to their default values
        stretchForce.ResetDefaultValues();
        shearForce.ResetDefaultValues();
        bendForce.ResetDefaultValues();
    }

    public void UpdateDefaultSpringValues() {
        // update spring's default values according to the global values we set
        stretchForce.SetDefaultValues(globalEquilibriumDistance, globalForceConstant);
        shearForce.SetDefaultValues(Mathf.Sqrt(2) * globalEquilibriumDistance, globalForceConstant);
        bendForce.SetDefaultValues(2 * globalEquilibriumDistance, globalForceConstant);
    }

    public SpringForceSettings GetSpringForce(InteractionType interactionType) {
        switch (interactionType) {
            case InteractionType.Stretch:
                return stretchForce;
            case InteractionType.Shear:
                return shearForce;
            case InteractionType.Bend:
                return bendForce;
            default:
                return default;
        }
    }

    // Getters/Accessors
    private ReferenceManager Scripts;
    public TargetManager TargetManager {
        get {
            return Scripts.TargetManager;
        }
    }
    public ParticleDisplayer Displayer {
        get {
            return Scripts.Displayer;
        }
    }


    public BaseParticle[] Particles {
        get {
            switch(simulationMode) {
                case SimulationMode.Verlet:
                    return verletSimulator.particles;
                case SimulationMode.Euler:
                    return eulerSimulator.particles;

                default:
                    // We should never reach here, given this is an enum
                    Debug.LogWarning("SimulationMode not defined");
                    return new BaseParticle[TargetManager.NParticles];
            }
        }

    }


    public void ParticleWasDragged(int index, Vector3 newPosition) {
        switch (simulationMode) {
            case SimulationMode.Verlet:
                verletSimulator.ParticleWasDragged(index, newPosition);
                break;
            case SimulationMode.Euler:
                eulerSimulator.ParticleWasDragged(index, newPosition);
                break;
        }
    }

    void Awake() {
        Scripts = GetComponent<ReferenceManager>();
    }

    public void Start() {
        switch (simulationMode) {
            case SimulationMode.Verlet:
                verletSimulator = new VerletSimulator(ref verletConstants) {
                    simulationManager = this,
                    targetManager = TargetManager,
                    particleDisplayer = Displayer
                };
                verletSimulator.Start();
                break;

            case SimulationMode.Euler:
                eulerSimulator = new EulerSimulator(ref eulerConstants) {
                    simulationManager = this,
                    targetManager = TargetManager,
                    particleDisplayer = Displayer
                };
                eulerSimulator.Start();
                break;
        }

        // Call this from here to make sure all the required variables are set
        Displayer.SpawnParticles();
    }

    public void FixedUpdate() {
        switch (simulationMode) {
            case SimulationMode.Verlet:
                verletSimulator.FixedUpdate();
                break;

            case SimulationMode.Euler:
                eulerSimulator.FixedUpdate();
                break;
        }
    }
}
