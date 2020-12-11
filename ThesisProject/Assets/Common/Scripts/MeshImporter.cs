using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshImporter {


    public static MeshModelData GetModelData(Mesh mesh) {
        // Some positions are duplicated by unity... if there 
        if (mesh.GetTopology(0) != MeshTopology.Quads) {
            Debug.LogWarning("Invalid Topology for the object. Mesh must have quad topology.");
            return default;
        }


        // Get Particles
        List<Vector3> positionsList = new List<Vector3>();
        Dictionary<int, int> duplicatedVertexIndicesMap = new Dictionary<int, int>();  // duplicated -> correct
        Dictionary<int, int> newVertexIndicesMap = new Dictionary<int, int>();  // original mesh -> new index on model

        int currentVertexIndex = 0;
        for (int i = 0; i < mesh.vertexCount; i++) {
            Vector3 position = mesh.vertices[i];

            int possibleDuplicate = positionsList.IndexOf(position);
            if (possibleDuplicate != -1) {
                // Found duplicate vertex, keep track of it for indices
                // Debug.Log($"Duplicate between index {i} and {possibleDuplicate}");
                duplicatedVertexIndicesMap.Add(i, possibleDuplicate);
                continue;
            }

            // Not a duplicate, add it to our model and to the map
            positionsList.Add(position);
            newVertexIndicesMap.Add(i, currentVertexIndex);
            currentVertexIndex++;
        }

        //Debug.Log($"Total Vertices: {mesh.vertexCount}. Particles: {currentVertexIndex}");
        int nParticles = currentVertexIndex;
        Vector3[] particlePositions = positionsList.ToArray();
        List<int>[] stretchIndices = GetStretchIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);
        List<int>[] shearIndices = GetShearIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);
        List<int>[] bendIndices = GetBendIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);

        List<ParticleInteraction> particleInteractions = GetParticleInteractionList(nParticles, stretchIndices, shearIndices, bendIndices, particlePositions);

        MeshModelData meshModelData = new MeshModelData {
            nParticles = nParticles,
            particlePositions = particlePositions,
            particleInteractions = particleInteractions
        };
        /*
        int debug_ix = 0;
        Debug.Log("Stretch: " + Utils.ArrayToString(stretchIndices[debug_ix].ToArray()));
        Debug.Log("Shear: " + Utils.ArrayToString(shearIndices[debug_ix].ToArray()));
        Debug.Log("Bend: " + Utils.ArrayToString(bendIndices[debug_ix].ToArray()));
        */
        return meshModelData;
    }

    static List<int>[] GetStretchIndices(Mesh mesh,
                                         int nParticles,
                                         Dictionary<int, int> duplicatedVertexIndicesMap,
                                         Dictionary<int, int> newVertexIndicesMap) {


        List<int>[] stretchIndices = new List<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);

        // iterate over each particle
        for (int i = 0; i < nParticles; i++) {
            // Find stretch friends for that particle: Iterate over each quad
            List<int> particleStretchIndices = new List<int>();

            for (int originalMeshIndex = 0; originalMeshIndex < meshIndices.Length; originalMeshIndex++) {

                // Read index, map value to proper value
                int mappedMeshIndex = GetMappedIndex(meshIndices[originalMeshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                //Debug.Log($"Mapping originalMeshIndex {meshIndices[originalMeshIndex]} => {mappedMeshIndex}");

                // Not the index we are looking for...
                if (mappedMeshIndex != i) {
                    continue;
                }

                // Found the index on a quad! Check it's position inside the quad and then add it to the indices list
                int quadPosition = originalMeshIndex % 4;
                if (quadPosition == 0) { //  && originalMeshIndex < meshIndices.Length - 3) {
                    // Bottom left, so add right and above particle indices
                    int bottom_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(bottom_right_particle_index)) {
                        particleStretchIndices.Add(bottom_right_particle_index);
                    }

                    int top_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(top_left_particle_index)) {
                        particleStretchIndices.Add(top_left_particle_index);
                    }
                }

                else if (quadPosition == 1) { 
                    // Top left, so add top right and bottom left particle indices
                    int top_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(top_right_particle_index)) {
                        particleStretchIndices.Add(top_right_particle_index);
                    }

                    int bottom_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(bottom_left_particle_index)) {
                        particleStretchIndices.Add(bottom_left_particle_index);
                    }
                }

                else if (quadPosition == 2) {
                    // Top right, so add top left and bottom right particle indices
                    int top_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(top_left_particle_index)) {
                        particleStretchIndices.Add(top_left_particle_index);
                    }

                    int bottom_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(bottom_right_particle_index)) {
                        particleStretchIndices.Add(bottom_right_particle_index);
                    }
                }

                else if (quadPosition == 3) {
                    // Bottom right, so add top right and bottom left particle indices
                    int top_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(top_right_particle_index)) {
                        particleStretchIndices.Add(top_right_particle_index);
                    }

                    int bottom_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleStretchIndices.Contains(bottom_left_particle_index)) {
                        particleStretchIndices.Add(bottom_left_particle_index);
                    }
                }
            }

            // Finished looping over all possible quads.
            stretchIndices[i] = particleStretchIndices;
        }

        return stretchIndices;
    }


    static List<int>[] GetShearIndices(Mesh mesh,
                                       int nParticles,
                                       Dictionary<int, int> duplicatedVertexIndicesMap,
                                       Dictionary<int, int> newVertexIndicesMap) {


        List<int>[] shearIndices = new List<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);

        // iterate over each particle
        for (int i = 0; i < nParticles; i++) {
            // Find stretch friends for that particle: Iterate over each quad
            List<int> particleShearIndices = new List<int>();

            for (int originalMeshIndex = 0; originalMeshIndex < meshIndices.Length; originalMeshIndex++) {

                // Read index, map value to proper value
                int mappedMeshIndex = GetMappedIndex(meshIndices[originalMeshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                //Debug.Log($"Mapping originalMeshIndex {meshIndices[originalMeshIndex]} => {mappedMeshIndex}");

                // Not the index we are looking for...
                if (mappedMeshIndex != i) {
                    continue;
                }

                // Found the index on a quad! Check it's position inside the quad and then add it to the indices list
                int quadPosition = originalMeshIndex % 4;
                if (quadPosition == 0) {
                    // Bottom left, so top right
                    int top_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 2], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleShearIndices.Contains(top_right_particle_index)) {
                        particleShearIndices.Add(top_right_particle_index);
                    }
                }

                else if (quadPosition == 1) {
                    // Top left, so add bottom right
                    int bottom_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 2], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleShearIndices.Contains(bottom_right_particle_index)) {
                        particleShearIndices.Add(bottom_right_particle_index);
                    }
                }

                else if (quadPosition == 2) {
                    // Top right, so add bottom left
                    int bottom_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 2], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleShearIndices.Contains(bottom_left_particle_index)) {
                        particleShearIndices.Add(bottom_left_particle_index);
                    }
                }

                else if (quadPosition == 3) {
                    // Bottom right, so add top left
                    int top_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 2], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (!particleShearIndices.Contains(top_left_particle_index)) {
                        particleShearIndices.Add(top_left_particle_index);
                    }
                }
            }

            // Finished looping over all possible quads.
            shearIndices[i] = particleShearIndices;
        }

        return shearIndices;
    }



    static List<int>[] GetBendIndices(Mesh mesh,
                                      int nParticles,
                                      Dictionary<int, int> duplicatedVertexIndicesMap,
                                      Dictionary<int, int> newVertexIndicesMap) {

        List<int>[] bendIndices = new List<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);

        // iterate over each particle
        for (int i = 0; i < nParticles; i++) {
            // Find stretch friends for that particle: Iterate over each quad
            List<int> particleBendIndices = new List<int>();
            //bendIndices[i] = particleBendIndices; continue;

            //int[] quadMeshIndices = System.Array.FindAll(meshIndices, ix => GetMappedIndex(ix, duplicatedVertexIndicesMap, newVertexIndicesMap) == i);
          
            
            for (int originalMeshIndex = 0; originalMeshIndex < meshIndices.Length; originalMeshIndex++) {
                // Read index, map value to proper value
                int mappedMeshIndex = GetMappedIndex(meshIndices[originalMeshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                //Debug.Log($"Mapping originalMeshIndex {meshIndices[originalMeshIndex]} => {mappedMeshIndex}");

                // Not the index we are looking for...
                if (mappedMeshIndex != i) {
                    continue;
                }

                int quadPosition = originalMeshIndex % 4;


                if (quadPosition == 0) {
                    // Bottom left particle

                    // Top-Top Particle
                    // First, find top in the same quad
                    int top_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where top particle is on the bottom


                    for (int top_top_index = 0; top_top_index < meshIndices.Length; top_top_index++) {
                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[top_top_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != top_left_particle_index) {
                            continue;
                        }


                        // Bottom left
                        if (top_top_index % 4 == 0) {
                            int top_top_particle_index = GetMappedIndex(meshIndices[top_top_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(top_top_particle_index)) {
                                particleBendIndices.Add(top_top_particle_index);
                            }
                        }
                        // Bottom right
                        if (top_top_index % 4 == 3) {
                            int top_top_particle_index = GetMappedIndex(meshIndices[top_top_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(top_top_particle_index)) {
                                particleBendIndices.Add(top_top_particle_index);
                            }
                        }
                    }

                    // Right-Right Particle
                    // First, find top in the same quad
                    int bottom_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where right particle is on the left

                    for (int right_right_index = 0; right_right_index < meshIndices.Length; right_right_index++) {

                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[right_right_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != bottom_right_particle_index) {
                            continue;
                        }


                        // Bottom left
                        if (right_right_index % 4 == 0) {
                            int right_right_particle_index = GetMappedIndex(meshIndices[right_right_index + 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(right_right_particle_index)) {
                                particleBendIndices.Add(right_right_particle_index);
                            }
                        }
                        // Top left
                        if (right_right_index % 4 == 1) {
                            int right_right_particle_index = GetMappedIndex(meshIndices[right_right_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(right_right_particle_index)) {
                                particleBendIndices.Add(right_right_particle_index);
                            }
                        }
                    }

                }


                if (quadPosition == 1) {
                    // Top left particle

                    // Bottom-Bottom Particle
                    int bottom_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where bottom particle is on the top

                    for (int bottom_bottom_index = 0; bottom_bottom_index < meshIndices.Length; bottom_bottom_index++) {
                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[bottom_bottom_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != bottom_left_particle_index) {
                            continue;
                        }


                        // Top left
                        if (bottom_bottom_index % 4 == 1) {
                            int bottom_bottom_particle_index = GetMappedIndex(meshIndices[bottom_bottom_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(bottom_bottom_particle_index)) {
                                particleBendIndices.Add(bottom_bottom_particle_index);
                            }
                        }
                        // Top right
                        if (bottom_bottom_index % 4 == 2) {
                            int bottom_bottom_particle_index = GetMappedIndex(meshIndices[bottom_bottom_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(bottom_bottom_particle_index)) {
                                particleBendIndices.Add(bottom_bottom_particle_index);
                            }
                        }
                    }

                    // Right-Right Particle
                    // First, find top in the same quad
                    int top_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where right particle is on the left
                    for (int right_right_index = 0; right_right_index < meshIndices.Length; right_right_index++) {

                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[right_right_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != top_right_particle_index) {
                            continue;
                        }


                        // Bottom left
                        if (right_right_index % 4 == 0) {
                            int right_right_particle_index = GetMappedIndex(meshIndices[right_right_index + 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(right_right_particle_index)) {
                                particleBendIndices.Add(right_right_particle_index);
                            }
                        }
                        // Top left
                        if (right_right_index % 4 == 1) {
                            int right_right_particle_index = GetMappedIndex(meshIndices[right_right_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(right_right_particle_index)) {
                                particleBendIndices.Add(right_right_particle_index);
                            }
                        }
                    }

                }


                if (quadPosition == 2) {
                    // Top right particle

                    // Bottom-Bottom Particle
                    int bottom_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where bottom particle is on the top

                    for (int bottom_bottom_index = 0; bottom_bottom_index < meshIndices.Length; bottom_bottom_index++) {
                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[bottom_bottom_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != bottom_right_particle_index) {
                            continue;
                        }


                        // Top left
                        if (bottom_bottom_index % 4 == 1) {
                            int bottom_bottom_particle_index = GetMappedIndex(meshIndices[bottom_bottom_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(bottom_bottom_particle_index)) {
                                particleBendIndices.Add(bottom_bottom_particle_index);
                            }
                        }
                        // Top right
                        if (bottom_bottom_index % 4 == 2) {
                            int bottom_bottom_particle_index = GetMappedIndex(meshIndices[bottom_bottom_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(bottom_bottom_particle_index)) {
                                particleBendIndices.Add(bottom_bottom_particle_index);
                            }
                        }
                    }

                    // Left-Left Particle
                    // First, find top in the same quad
                    int top_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where left particle is on the right
                    for (int left_left_index = 0; left_left_index < meshIndices.Length; left_left_index++) {

                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[left_left_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != top_left_particle_index) {
                            continue;
                        }


                        // Top Right
                        if (left_left_index % 4 == 2) {
                            int left_left_particle_index = GetMappedIndex(meshIndices[left_left_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(left_left_particle_index)) {
                                particleBendIndices.Add(left_left_particle_index);
                            }
                        }
                        // Bottom Right
                        if (left_left_index % 4 == 3) {
                            int left_left_particle_index = GetMappedIndex(meshIndices[left_left_index - 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(left_left_particle_index)) {
                                particleBendIndices.Add(left_left_particle_index);
                            }
                        }
                    }
                }


                if (quadPosition == 3) {
                    // Bottom right particle


                    // Top-Top Particle
                    // First, find top in the same quad
                    int top_right_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where top particle is on the bottom

                    for (int top_top_index = 0; top_top_index < meshIndices.Length; top_top_index++) {
                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[top_top_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != top_right_particle_index) {
                            continue;
                        }


                        // Bottom left
                        if (top_top_index % 4 == 0) {
                            int top_top_particle_index = GetMappedIndex(meshIndices[top_top_index + 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(top_top_particle_index)) {
                                particleBendIndices.Add(top_top_particle_index);
                            }
                        }
                        // Bottom right
                        if (top_top_index % 4 == 3) {
                            int top_top_particle_index = GetMappedIndex(meshIndices[top_top_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(top_top_particle_index)) {
                                particleBendIndices.Add(top_top_particle_index);
                            }
                        }
                    }

                    // Left-Left Particle
                    // First, find top in the same quad
                    int bottom_left_particle_index = GetMappedIndex(meshIndices[originalMeshIndex - 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    // Search quads where left particle is on the right
                    for (int left_left_index = 0; left_left_index < meshIndices.Length; left_left_index++) {

                        int mappedNeighbourMeshIndex = GetMappedIndex(meshIndices[left_left_index], duplicatedVertexIndicesMap, newVertexIndicesMap);
                        if (mappedNeighbourMeshIndex != bottom_left_particle_index) {
                            continue;
                        }


                        // Top Right
                        if (left_left_index % 4 == 2) {
                            int left_left_particle_index = GetMappedIndex(meshIndices[left_left_index - 1], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(left_left_particle_index)) {
                                particleBendIndices.Add(left_left_particle_index);
                            }
                        }
                        // Bottom Right
                        if (left_left_index % 4 == 3) {
                            int left_left_particle_index = GetMappedIndex(meshIndices[left_left_index - 3], duplicatedVertexIndicesMap, newVertexIndicesMap);
                            if (!particleBendIndices.Contains(left_left_particle_index)) {
                                particleBendIndices.Add(left_left_particle_index);
                            }
                        }
                    }
                } 
            }

            // Finished looping over all possible quads.
            bendIndices[i] = particleBendIndices;
        }

        return bendIndices;
    }



    static int GetMappedIndex(int originalIndex, Dictionary<int, int> duplicatedVertexIndicesMap, Dictionary<int, int> newVertexIndicesMap) {
        if (duplicatedVertexIndicesMap.ContainsKey(originalIndex)) {
            return duplicatedVertexIndicesMap[originalIndex];
        }
        else if (newVertexIndicesMap.ContainsKey(originalIndex)) {
            return newVertexIndicesMap[originalIndex];
        }
        Debug.LogWarning($"Couldn't find key in dictionaries.. Returning original {originalIndex}.");
        return originalIndex;
    }


    static List<ParticleInteraction> GetParticleInteractionList(int nParticles,
                                                                List<int>[] stretchIndices,
                                                                List<int>[] shearIndices,
                                                                List<int>[] bendIndices,
                                                                Vector3[] particlePositions) {


        List<ParticleInteraction> particleInteractions = new List<ParticleInteraction>();
        
        // Add stretch interactions
        for (int particle1Index = 0; particle1Index < nParticles; particle1Index++) {
            foreach (int particle2Index in stretchIndices[particle1Index]) {
                float distance = (particlePositions[particle1Index] - particlePositions[particle2Index]).magnitude;
                ParticleInteraction interaction = new ParticleInteraction(particle1Index, particle2Index, distance, InteractionType.Stretch);
                particleInteractions.Add(interaction);
            }
        }
        
        // Add shear interactions
        for (int particle1Index = 0; particle1Index < nParticles; particle1Index++) {
            foreach (int particle2Index in shearIndices[particle1Index]) {
                float distance = (particlePositions[particle1Index] - particlePositions[particle2Index]).magnitude;
                ParticleInteraction interaction = new ParticleInteraction(particle1Index, particle2Index, distance, InteractionType.Shear);
                particleInteractions.Add(interaction);
            }
        }
        /*
        // Add bend interactions
        for (int particle1Index = 0; particle1Index < nParticles; particle1Index++) {
            foreach (int particle2Index in bendIndices[particle1Index]) {
                float distance = (particlePositions[particle1Index] - particlePositions[particle2Index]).magnitude;
                ParticleInteraction interaction = new ParticleInteraction(particle1Index, particle2Index, distance, InteractionType.Bend);
                particleInteractions.Add(interaction);
            }
        }
        */
        return particleInteractions;

    }
}

public class MeshModelData {

    public int nParticles;
    public Vector3[] particlePositions;

    public List<ParticleInteraction> particleInteractions;
}
