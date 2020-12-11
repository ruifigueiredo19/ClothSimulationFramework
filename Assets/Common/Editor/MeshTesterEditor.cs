using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshTester))]
public class MeshTesterEditor : Editor
{

    MeshTester meshTester;

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Get Stats")) {
            meshTester.GetStats();
        }
        if (GUILayout.Button("Print Indices")) {
            meshTester.PrintIndices();
        }
        GUILayout.EndHorizontal();
    }



    private void OnEnable() {
        // Setup script and properties
        meshTester = (MeshTester) target;
    }
}
