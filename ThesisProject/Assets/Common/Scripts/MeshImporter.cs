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
        HashSet<int>[] stretchIndices = GetStretchIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);
        HashSet<int>[] shearIndices = GetShearIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);
        HashSet<int>[] bendIndices = GetBendIndices(mesh, nParticles, duplicatedVertexIndicesMap, newVertexIndicesMap);

        List<ParticleInteraction> particleInteractions = GetParticleInteractionList(nParticles, stretchIndices, shearIndices, bendIndices, particlePositions);

        MeshModelData meshModelData = new MeshModelData {
            nParticles = nParticles,
            particlePositions = particlePositions,
            particleInteractions = particleInteractions
        };
        return meshModelData;
    }

    static HashSet<int>[] GetStretchIndices(Mesh mesh,
                                            int nParticles,
                                            Dictionary<int, int> duplicatedVertexIndicesMap,
                                            Dictionary<int, int> newVertexIndicesMap) {

        HashSet<int>[] stretchIndices = new HashSet<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);


        for (int particleIndex = 0; particleIndex < nParticles; particleIndex++) {
            HashSet<int> particleStretchIndices = new HashSet<int>();

            // Find faces that contain that particle
            for (int meshIndex = 0; meshIndex < meshIndices.Length; meshIndex++) {

                // To compare if the same, map mesh index to a particle index
                int mappedMeshParticleIndex = GetMappedIndex(meshIndices[meshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                if (mappedMeshParticleIndex != particleIndex) {
                    continue;
                }

                // Found a face with that particle, save face index and offset
                int startIndexOfFace = meshIndex / 4 * 4;
                int offsetIndexInsideFace = meshIndex - startIndexOfFace;

                // Add interaction with adjacent vertices
                particleStretchIndices.Add(GetMappedIndex(meshIndices[startIndexOfFace + (offsetIndexInsideFace + 1) % 4], duplicatedVertexIndicesMap, newVertexIndicesMap));
                particleStretchIndices.Add(GetMappedIndex(meshIndices[startIndexOfFace + (offsetIndexInsideFace + 3) % 4], duplicatedVertexIndicesMap, newVertexIndicesMap));
            }

            stretchIndices[particleIndex] = particleStretchIndices;
        }

        return stretchIndices;
    }


    static HashSet<int>[] GetShearIndices(Mesh mesh,
                                          int nParticles,
                                          Dictionary<int, int> duplicatedVertexIndicesMap,
                                          Dictionary<int, int> newVertexIndicesMap) {



        HashSet<int>[] shearIndices = new HashSet<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);


        for (int particleIndex = 0; particleIndex < nParticles; particleIndex++) {
            HashSet<int> particleShearIndices = new HashSet<int>();

            // Find faces that contain that particle
            for (int meshIndex = 0; meshIndex < meshIndices.Length; meshIndex++) {

                // To compare if the same, map mesh index to a particle index
                int mappedMeshParticleIndex = GetMappedIndex(meshIndices[meshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                if (mappedMeshParticleIndex != particleIndex) {
                    continue;
                }

                // Found a face with that particle, save face index and offset
                int startIndexOfFace = meshIndex / 4 * 4;
                int offsetIndexInsideFace = meshIndex - startIndexOfFace;

                // Add interaction with adjacent vertices
                particleShearIndices.Add(GetMappedIndex(meshIndices[startIndexOfFace + (offsetIndexInsideFace + 2) % 4], duplicatedVertexIndicesMap, newVertexIndicesMap));
            }

            shearIndices[particleIndex] = particleShearIndices;
        }

        return shearIndices;
    }



    static HashSet<int>[] GetBendIndices(Mesh mesh,
                                         int nParticles,
                                         Dictionary<int, int> duplicatedVertexIndicesMap,
                                         Dictionary<int, int> newVertexIndicesMap) {



        HashSet<int>[] bendIndices = new HashSet<int>[nParticles];
        int[] meshIndices = mesh.GetIndices(0);


        for (int particleIndex = 0; particleIndex < nParticles; particleIndex++) {
            HashSet<int> particleBendIndices = new HashSet<int>();

            // Search for the faces with that particle
            List<int> startIndicesOfFaces = new List<int>();
            //List<int> offsetIndicesInsideFaces = new List<int>();
            HashSet<int> adjacentsOfAdjacentIndices = new HashSet<int>();

            // Find ALL faces that contain that particle
            for (int meshIndex = 0; meshIndex < meshIndices.Length; meshIndex++) {

                // To compare if the same, map mesh index to a particle index
                int mappedMeshParticleIndex = GetMappedIndex(meshIndices[meshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                if (mappedMeshParticleIndex != particleIndex) {
                    continue;
                }

                // Found a face with that particle, save face index and offset
                int startIndexOfFace = meshIndex / 4 * 4;
                int offsetIndexInsideFace = meshIndex - startIndexOfFace;
                if (!startIndicesOfFaces.Contains(startIndexOfFace)) {
                    startIndicesOfFaces.Add(startIndexOfFace);
                    //offsetIndicesInsideFaces.Add(offsetIndexInsideFace);
                }


                // Get adjacent
                int adjacentParticleIndex1 = GetMappedIndex(meshIndices[startIndexOfFace + (offsetIndexInsideFace + 1) % 4], duplicatedVertexIndicesMap, newVertexIndicesMap);
                int adjacentParticleIndex2 = GetMappedIndex(meshIndices[startIndexOfFace + (offsetIndexInsideFace + 3) % 4], duplicatedVertexIndicesMap, newVertexIndicesMap);

                // Find ALL faces that contain that adjacent particles
                for (int adjacentMeshIndex = 0; adjacentMeshIndex < meshIndices.Length; adjacentMeshIndex++) {

                    // To compare if the same, map adjacent mesh index to a particle index
                    int mappedAdjacentParticleIndex = GetMappedIndex(meshIndices[adjacentMeshIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);
                    if (mappedAdjacentParticleIndex != adjacentParticleIndex1 && mappedAdjacentParticleIndex != adjacentParticleIndex2) {
                        continue;
                    }


                    // Found a face with that adjacent, save adjacent of adjacent
                    int startIndexOfAdjacentFace = adjacentMeshIndex / 4 * 4;
                    int offsetIndexInsideAdjacentFace = adjacentMeshIndex - startIndexOfAdjacentFace;
                    adjacentsOfAdjacentIndices.Add(startIndexOfAdjacentFace + (offsetIndexInsideAdjacentFace + 1) % 4);
                    adjacentsOfAdjacentIndices.Add(startIndexOfAdjacentFace + (offsetIndexInsideAdjacentFace + 3) % 4);
                }
            }

            // Make sure the adjacent of adjacent aren't in the original faces
            foreach (int adjacentOfAdjacentIndex in adjacentsOfAdjacentIndices) {
                bool inOriginalFace = false;
                int mappedAdjacentOfAdjacentParticleIndex = GetMappedIndex(meshIndices[adjacentOfAdjacentIndex], duplicatedVertexIndicesMap, newVertexIndicesMap);

                foreach (int startFaceIndex in startIndicesOfFaces) {
                    for (int offset = 0; offset < 4; offset++) {
                        if (mappedAdjacentOfAdjacentParticleIndex == GetMappedIndex(meshIndices[startFaceIndex + offset], duplicatedVertexIndicesMap, newVertexIndicesMap)) {
                            inOriginalFace = true;
                            break;
                        }
                    }

                    if (inOriginalFace) {
                        break;
                    }
                }

                // Add it to particleBendIndices
                if (!inOriginalFace) {
                    particleBendIndices.Add(mappedAdjacentOfAdjacentParticleIndex);
                }
            }

            bendIndices[particleIndex] = particleBendIndices;
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
                                                                HashSet<int>[] stretchIndices,
                                                                HashSet<int>[] shearIndices,
                                                                HashSet<int>[] bendIndices,
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
        
        // Add bend interactions
        for (int particle1Index = 0; particle1Index < nParticles; particle1Index++) {
            foreach (int particle2Index in bendIndices[particle1Index]) {
                float distance = (particlePositions[particle1Index] - particlePositions[particle2Index]).magnitude;
                ParticleInteraction interaction = new ParticleInteraction(particle1Index, particle2Index, distance, InteractionType.Bend);
                particleInteractions.Add(interaction);
            }
        }

        return particleInteractions;
    }
}

public class MeshModelData {

    public int nParticles;
    public Vector3[] particlePositions;

    public List<ParticleInteraction> particleInteractions;
}
