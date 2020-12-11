using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReferenceManager))]
public class ParticleDisplayer : MonoBehaviour {

    [Header("Particle Settings")]
    [Range(0, 1)]
    public float size = 0.35f;
    public Material particleMaterial;

    [Header("Force Settings")]
    public bool drawSpringInteractions = true;
    public Color stretchSpringColor = Color.green;
    public Color shearSpringColor = Color.red;
    public Color bendSpringColor = Color.yellow;


    // Keep objects in aray
    private readonly string particleHolderName = "Particles";
    private GameObject[] particles;

    // Getters/Accessors
    private ReferenceManager Scripts;
    public TargetManager TargetManager {
        get {
            return Scripts.TargetManager;
        }
    }
    public SimulationManager SimulationManager {
        get {
            return Scripts.SimulationManager;
        }
    }

    private Transform ParticleHolder {
        get {
            Transform child = transform.Find(particleHolderName);
            if (child == null) {
                // If doesn't exists, create an empty child gameobject
                child = new GameObject(particleHolderName).transform;
                child.parent = transform;
                child.localPosition = Vector3.zero;
            }
            return child;
        }
    }


    // Functions
    public void SpawnParticles() {
        DestroyParticles();

        //Debug.Log("Spawning new particles");
        particles = new GameObject[TargetManager.NParticles];

        for (int i = 0; i < TargetManager.NParticles; i++) {
            Vector3 particlePosition = SimulationManager.Particles[i].Position;
            //Vector3 particlePosition = new Vector3(-ParticleManager.matrixSize.x / 2 + x, 0, -ParticleManager.matrixSize.y / 2 + y);  // we don't  need +0.5f to center, cause we are using V2Int

            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.name = string.Format("{0} ({1})", particle.name, i);
            DestroyImmediate(particle.GetComponent<Collider>());

            particle.transform.parent = ParticleHolder;  // set the parent first so we can set the local position correctly
            particle.transform.localPosition = particlePosition;
            particle.transform.localScale = Vector3.one * size;
            particle.transform.hasChanged = false;

            particles[i] = particle;


            if (particleMaterial) {
                particle.GetComponent<MeshRenderer>().material = particleMaterial;

            }
        }
    }

    public void UpdateParticlePositions() {
        
        Transform particleTransform;  // allocate it once per frame

        for (int i = 0; i < TargetManager.NParticles; i++) {
            particleTransform = particles[i].transform;
            particleTransform.position = SimulationManager.Particles[i].Position;
            particleTransform.hasChanged = false;  // don't detect this as a editor dragging event
        }
    }

    public void RedrawParticles() {
        Debug.Log("RedrawParticles");
        // No particles on the array, so create new fresh ones
        if (particles == null) {
            SpawnParticles();
            return;
        }

        // Particles already exist, so redraw them with updated scale
        for (int i = 0; i < TargetManager.NParticles; i++) {
            GameObject particle = particles[i];
            particle.transform.localScale = Vector3.one * size;
        }
    }


    public void DestroyParticles() {
        if (Application.isEditor) {
            for (int i = ParticleHolder.childCount - 1; i >= 0; i--) {
                /*
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(ParticleHolder.transform.GetChild(i).gameObject);
                };
                */
                DestroyImmediate(ParticleHolder.transform.GetChild(i).gameObject);
            }
        }
        else {
            for (int i = ParticleHolder.transform.childCount - 1; i >= 0; i--) {
                Destroy(ParticleHolder.transform.GetChild(i).gameObject);
            }
        }
    }



    // DRAWING INTERACTION FUNCTIONS
    public void DrawAllSpringInteractions() {
        if (drawSpringInteractions) {
            // Check if they exist
            if (SimulationManager.ParticleInteractions == null) {
                Debug.LogWarning("Manager didn't have neighbour indices calculated.");
            }

            foreach (ParticleInteraction interaction in SimulationManager.ParticleInteractions) {
                switch (interaction.interactionType) {
                    case InteractionType.Stretch:
                        if (SimulationManager.stretchForce.enabled) {
                            Debug.DrawLine(SimulationManager.Particles[interaction.particle1].Position, SimulationManager.Particles[interaction.particle2].Position, stretchSpringColor);
                        }
                        break;
                    case InteractionType.Shear:
                        if (SimulationManager.shearForce.enabled) {
                            Debug.DrawLine(SimulationManager.Particles[interaction.particle1].Position, SimulationManager.Particles[interaction.particle2].Position, shearSpringColor);
                        }
                        break;
                    case InteractionType.Bend:
                        if (SimulationManager.bendForce.enabled) {
                            Debug.DrawLine(SimulationManager.Particles[interaction.particle1].Position, SimulationManager.Particles[interaction.particle2].Position, bendSpringColor);
                        }
                        break;
                }
            }
        }
    }


    // ALLOW DRAGGING IN EDITOR
    
    public void CheckForDraggedParticles() {
        Transform particleTransform; // allocate it once per frame

        for (int i = 0; i < TargetManager.NParticles; i++) {
            particleTransform = particles[i].transform;
            if (particleTransform.hasChanged) {
                // This will move the particle in the simulator data, and reset it's speed to 0
                SimulationManager.ParticleWasDragged(i, particleTransform.position);
                particleTransform.hasChanged = false;
            }
        }
    }



    void Awake() {
        Scripts = GetComponent<ReferenceManager>();
    }

    /*
    private void Start() {
        SpawnParticles();
    }
    */


    void Update() {
        CheckForDraggedParticles();
        UpdateParticlePositions();
        DrawAllSpringInteractions();
    }
}
