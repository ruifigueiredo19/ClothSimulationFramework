using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshTester : MonoBehaviour {
    public bool gizmoEnabled = true;

    Mesh mesh;
    public int index = 0;
    Color[] colours = { Color.blue, Color.cyan, Color.green, Color.yellow };

    public void GetStats() {
        if (mesh == null) {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }


        Debug.Log($"Mesh Topology: {mesh.GetTopology(0)}");
        Debug.Log($"Mesh Indices: {Utils.ArrayToString(mesh.GetIndices(0))}");
        Debug.Log($"Mesh Indices Length: {mesh.GetIndices(0).Length}");

        Debug.Log($"Mesh Vertices: {mesh.vertexCount}");

    }



    public void OnDrawGizmos() {
        if (!gizmoEnabled) {
            return;
        }
        if (mesh == null) {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }
        int[] indices = mesh.GetIndices(0);

        for (int i = 0; i < 4; i++) {
            /*
            Gizmos.color = colours[i];
            Gizmos.DrawLine(mesh.vertices[mesh.triangles[index + i]], mesh.vertices[mesh.triangles[index + i + 1]]);
            */
            Vector3 startPos = mesh.vertices[indices[4 * index + i]];
            Vector3 endPos = mesh.vertices[indices[4 * index + (i + 1)%4]];
            Vector3 deltaPos = Vector3.zero; // Vector3.up * ((i-1) * 0.05f);


            Handles.color = colours[i];
            Handles.DrawLine(startPos + deltaPos, endPos + deltaPos, 10);
            Handles.Label(startPos + Vector3.up * (startPos.y > 0f ? 0.2f : -0.1f), $"{i}/{indices[4 * index + i]}");
        }
    }

    public void PrintIndices() {
        if (mesh == null) {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }

        int[] indices = mesh.GetIndices(0);
        Debug.Log($"Current face indices: {indices[4 * index]} {indices[4 * index + 1]} {indices[4 * index + 2]} {indices[4 * index + 3]}");

    }

    public void OnValidate() {
        if (mesh == null) {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }
        int max_index_value = (mesh.GetIndices(0).Length - 1) / 4;


        if (index < 0 || index > max_index_value) {
            index = (index % max_index_value + max_index_value) % max_index_value;
        }
    }
}
