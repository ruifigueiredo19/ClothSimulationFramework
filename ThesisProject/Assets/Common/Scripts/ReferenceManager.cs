using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimulationManager), typeof(TargetManager), typeof(ParticleDisplayer))]
public class ReferenceManager : MonoBehaviour
{
    // This scripts holds references to the other managers in the same object
    public SimulationManager SimulationManager { get; private set; }
    public TargetManager TargetManager { get; private set; }
    public ParticleDisplayer Displayer { get; private set; }


    void Awake()
    {
        SimulationManager = GetComponent<SimulationManager>();
        TargetManager = GetComponent<TargetManager>();
        Displayer = GetComponent<ParticleDisplayer>();
    }
}
