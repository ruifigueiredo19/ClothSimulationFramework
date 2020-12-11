using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReferenceManager))]
public class TargetManager : MonoBehaviour {
    public enum TargetObjectMode { ImportedObject, RectangularCloth };
    [Header("General Settings")]
    public TargetObjectMode targetObjectMode = TargetObjectMode.RectangularCloth;

    // Imported object
    public GameObject importedObject;
    MeshModelData meshModelData;
    Mesh importedObjectMesh;
    
    // Rectangular Cloth
    public Vector2Int matrixSize = new Vector2Int(3, 3);

    public int NParticles {
        get {
            switch (targetObjectMode) {
                case TargetObjectMode.ImportedObject:
                    return meshModelData.nParticles;
                case TargetObjectMode.RectangularCloth:
                    return matrixSize.x * matrixSize.y;
                default:
                    return default;
            }
        }
    }


    // Getters/Accessors
    private ReferenceManager Scripts;
    public ParticleDisplayer Displayer {
        get {
            return Scripts.Displayer;
        }
    }
    public SimulationManager SimulationManager {
        get {
            return Scripts.SimulationManager;
        }
    }



    // Generating particle array
    public Vector3[] GetTargetParticlePositions() {
        switch (targetObjectMode) {
            case TargetObjectMode.ImportedObject:
                importedObjectMesh = importedObject.GetComponentInChildren<MeshFilter>().mesh;
                meshModelData = MeshImporter.GetModelData(importedObjectMesh);
                return meshModelData.particlePositions;

            case TargetObjectMode.RectangularCloth:
                return GenerateParticlesRectangularCloth();
        }

        return default;
    }


    public Vector3[] GenerateParticlesRectangularCloth() {
        Vector3[] particles = new Vector3[matrixSize.x * matrixSize.y];

        for (int x = 0; x < matrixSize.x; x++) {
            for (int y = 0; y < matrixSize.y; y++) {
                particles[y * matrixSize.x + x] = new Vector3(-matrixSize.x / 2 + x, 0, -matrixSize.y / 2 + y);  // we don't  need +0.5f to center, cause we are using V2Int
            }
        }

        return particles;
    }

    // Generating spring interactions
    public List<ParticleInteraction> GetSpringInteractions() {
        switch (targetObjectMode) {
            case TargetObjectMode.ImportedObject:
                return meshModelData.particleInteractions;

            case TargetObjectMode.RectangularCloth:
                return GetSpringInteractionsRectangularCloth();

            default:
                return new List<ParticleInteraction>();
        }
    }


    List<ParticleInteraction> GetSpringInteractionsRectangularCloth() {

        List<ParticleInteraction> particleInteractions = new List<ParticleInteraction>();
        int indexX, indexY;

        for (int x = 0; x < matrixSize.x; x++) {
            for (int y = 0; y < matrixSize.y; y++) {
                // Add index if it's inside the mesh for each of the forces available
                // Generate for all indices, regardless if force disabled or not

                /// STRETCH INDICES
                // Up
                indexX = x;
                indexY = y + 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.stretchForce.equilibriumDistance, InteractionType.Stretch);
                    particleInteractions.Add(interaction);
                }


                // Down
                indexX = x;
                indexY = y - 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.stretchForce.equilibriumDistance, InteractionType.Stretch);
                    particleInteractions.Add(interaction);
                }


                // Left
                indexX = x - 1;
                indexY = y;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.stretchForce.equilibriumDistance, InteractionType.Stretch);
                    particleInteractions.Add(interaction);
                }


                // Right
                indexX = x + 1;
                indexY = y;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.stretchForce.equilibriumDistance, InteractionType.Stretch);
                    particleInteractions.Add(interaction);
                }


                /// SHEAR INDICES
                // Up Left
                indexX = x - 1;
                indexY = y + 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.shearForce.equilibriumDistance, InteractionType.Shear);
                    particleInteractions.Add(interaction);
                }

                // Up Right
                indexX = x + 1;
                indexY = y + 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.shearForce.equilibriumDistance, InteractionType.Shear);
                    particleInteractions.Add(interaction);
                }

                // Bot Left
                indexX = x - 1;
                indexY = y - 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.shearForce.equilibriumDistance, InteractionType.Shear);
                    particleInteractions.Add(interaction);
                }

                // Bot Right
                indexX = x + 1;
                indexY = y - 1;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.shearForce.equilibriumDistance, InteractionType.Shear);
                    particleInteractions.Add(interaction);
                }


                /// BEND INDICES
                // 2 Up
                indexX = x;
                indexY = y + 2;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.bendForce.equilibriumDistance, InteractionType.Bend);
                    particleInteractions.Add(interaction);
                }


                // 2 Down
                indexX = x;
                indexY = y - 2;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.bendForce.equilibriumDistance, InteractionType.Bend);
                    particleInteractions.Add(interaction);
                }


                // 2 Left
                indexX = x - 2;
                indexY = y;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.bendForce.equilibriumDistance, InteractionType.Bend);
                    particleInteractions.Add(interaction);
                }


                // 2 Right
                indexX = x + 2;
                indexY = y;
                if (IsInsideMatrix(indexX, indexY)) {
                    ParticleInteraction interaction = new ParticleInteraction(y * matrixSize.x + x, indexY * matrixSize.x + indexX, SimulationManager.bendForce.equilibriumDistance, InteractionType.Bend);
                    particleInteractions.Add(interaction);
                }
            }
        }

        return particleInteractions;
    }

    bool IsInsideMatrix(int indexX, int indexY) {
        return indexX >= 0 && indexX < matrixSize.x && indexY >= 0 && indexY < matrixSize.y;
    }

    // Unity Methods
    void Awake() {
        Scripts = GetComponent<ReferenceManager>();
    }

    private void OnValidate() {
        // Keep mesh always non negative
        if (matrixSize.x < 0) {
            matrixSize.x = 0;
        }
        if (matrixSize.y < 0) {
            matrixSize.y = 0;
        }
    }
}
