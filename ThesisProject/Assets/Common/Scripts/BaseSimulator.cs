using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class BaseSimulator<T> where T : BaseParticle, new() {
    public T[] particles;

    // Get these from the target (TargetManager)
    protected int nParticles;
    protected Vector3[] initialPositions;
    public List<ParticleInteraction> particleInteractions;


    // Access to other managers
    public SimulationManager simulationManager;
    public TargetManager targetManager;
    public ParticleDisplayer particleDisplayer;


    // Initial conditions
    protected virtual void SetInitialConditions() {
        // This function is a placeholder to be overridden
        Debug.Log($"{MethodBase.GetCurrentMethod().Name}: Running Base Simulator on initial conditions");

        for (int i = 0; i < nParticles; i++) {
            particles[i] = new T {
                Position = initialPositions[i]
            };
        }
    }

    protected virtual void InitialConditionDisplacement() {
        // This function does nothing, just a placeholder to be overridden
    }


    // On interactions from the editor
    public virtual void ParticleWasDragged(int index, Vector3 newPosition) {
        // By default, if the particle was dragged, we set it's position to the new position, and reset the velocity to 0
        particles[index].Position = newPosition;
    }

    // SOLVING SIMULATION FUNCTIONS (TO BE OVERRIDEN BY CHILD CLASSES)
    protected virtual void SimulateNextPhysicsStep() {
        // This function does nothing, just a placeholder to be overridden
        Debug.Log($"{MethodBase.GetCurrentMethod().Name}: Running Base Simulator, so nothing happens");
    }


    protected virtual void AddExternalForces() {
        if (simulationManager.gravityForce.enabled) {
            for (int i = 0; i < nParticles; i++) {
                particles[i].AddForce(simulationManager.gravityForce.GetForce() * particles[i].mass);
            }
        }

        if (simulationManager.windForce.enabled) {
            for (int i = 0; i < nParticles; i++) {
                particles[i].AddForce(simulationManager.windForce.GetForce());
            }
        }
    }




    // "UNITY" FUNCTIONS TO RUN THE SIMULATION
    // Since this class doesn't inherit from monobehaviour, these functions need to be called explicitely by the simulator manager
    public void Start() {
        // Get info from the target
        initialPositions = targetManager.GetTargetParticlePositions();  // System.Array.Copy(targetManager.GetTargetParticlePositions(), initialPositions, targetManager.NParticles);
        particleInteractions = targetManager.GetSpringInteractions();
        nParticles = targetManager.NParticles;  // this must be set after the initial positions

        particles = new T[nParticles];

        // Initial conditions
        SetInitialConditions();
        InitialConditionDisplacement();
    }


    public void FixedUpdate() {
        // Update Physics
        SimulateNextPhysicsStep();
    }
}
