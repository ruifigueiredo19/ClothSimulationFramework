using UnityEngine;
using UnityEditor;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;



[CustomEditor(typeof(TargetManager))]
public class TargetManagerEditor : Editor {
    // Main script
    TargetManager targetManager;

    // Properties to draw
    SerializedProperty scriptProp;
    SerializedProperty targetObjectModeProp;
    SerializedProperty importedObjectProp, matrixSizeProp;

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        serializedObject.Update();

        // Add script at top (not changeable)
        GUI.enabled = false;
        EditorGUILayout.PropertyField(scriptProp);
        GUI.enabled = true;

        // Enum for the mode
        EditorGUILayout.PropertyField(targetObjectModeProp);

        // Only draw the field for the matrix size or for importing the object
        EditorGUI.indentLevel++;
        switch (targetManager.targetObjectMode) {
            case TargetManager.TargetObjectMode.ImportedObject:
                EditorGUILayout.PropertyField(importedObjectProp);
                break;

            case TargetManager.TargetObjectMode.RectangularCloth:
                EditorGUILayout.PropertyField(matrixSizeProp);
                break;
        }
        EditorGUI.indentLevel--;

        // Finish Setting up Property Fields
        serializedObject.ApplyModifiedProperties();

    }

    private void OnEnable() {
        // Setup script and properties
        targetManager = (TargetManager) target;
        scriptProp = serializedObject.FindProperty("m_Script");

        // general
        targetObjectModeProp = serializedObject.FindProperty("targetObjectMode");

        // TargetObjectMode = .ImportedObject
        importedObjectProp = serializedObject.FindProperty("importedObject");

        // TargetObjectMode = .RectangularCloth
        matrixSizeProp = serializedObject.FindProperty("matrixSize");
    }
}


/*
[CustomEditor(typeof(ParticleManager))]
public class ParticleManagerEditor : Editor
{
    
    // Main script
    ParticleManager manager;

    // Properties to draw
    SerializedProperty scriptProp;
    SerializedProperty matrixSizeProp, simulationModeProp;
    SerializedProperty globalEquilibriumDistanceProp, globalForceConstantProp;
    SerializedProperty stretchForceProp, shearForceProp, bendForceProp;

    // "dirty" flags
    bool needToRespawn, needToRedraw, needToCalculateNeighbourIndices,
        simulationModeChanged;

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        
        ResetDirtyFlags();
        serializedObject.Update();

        // Add script at top (not changeable)
        GUI.enabled = false;
        EditorGUILayout.PropertyField(scriptProp);
        GUI.enabled = true;

        // Matrix Size V2Int and Simulation Mode
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(matrixSizeProp);
        if (EditorGUI.EndChangeCheck()) {
            needToRespawn = true;
            needToCalculateNeighbourIndices = true;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(simulationModeProp);
        if (EditorGUI.EndChangeCheck()) {
            simulationModeChanged = true;
        }

        // Global Spring Settings
        EditorGUILayout.PropertyField(globalEquilibriumDistanceProp);
        EditorGUILayout.PropertyField(globalForceConstantProp);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Current Values")) {
            manager.UpdateDefaultSpringValues();
            manager.ResetSpringValues();
        }
        if (GUILayout.Button("Update Default Values")) {
            manager.UpdateDefaultSpringValues();
        }
        GUILayout.EndHorizontal();

        // Stretch Spring Settings
        EditorGUILayout.PropertyField(stretchForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (manager.stretchForce.enabled && stretchForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Stretch Spring Values")) {
                manager.stretchForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }

        // Shear Spring Settings
        EditorGUILayout.PropertyField(shearForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (manager.shearForce.enabled && shearForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Shear Spring Values")) {
                manager.shearForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }

        // Bend Spring Settings
        EditorGUILayout.PropertyField(bendForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (manager.bendForce.enabled && bendForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Bend Spring Values")) {
                manager.bendForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }


        // Finish Setting up Property Fields
        serializedObject.ApplyModifiedProperties();

        // Respawn or Redraw if needed (and if a displayer is attached)
        if (manager.Displayer != null) {
            if (needToRespawn) {
                manager.Displayer.SpawnParticles();
            }
            else if (needToRedraw) {
                manager.Displayer.RedrawParticles();
            }


            if (needToCalculateNeighbourIndices) {
                manager.CreateNeighbourIndices();
            }

            if (simulationModeChanged) {
                //manager.UpdateSimulatorReference();
            }

        }
        

    }

    private void ResetDirtyFlags() {
        needToRespawn = false;
        needToRedraw = false;
        needToCalculateNeighbourIndices = false;

        simulationModeChanged = false;
    }

    private void OnEnable() {
        // Setup script and properties
        manager = (ParticleManager) target;
        scriptProp = serializedObject.FindProperty("m_Script");

        // general
        matrixSizeProp = serializedObject.FindProperty("matrixSize");
        simulationModeProp = serializedObject.FindProperty("simulationMode");

        // global
        globalEquilibriumDistanceProp = serializedObject.FindProperty("globalEquilibriumDistance");
        globalForceConstantProp = serializedObject.FindProperty("globalForceConstant");

        // springs forces
        stretchForceProp = serializedObject.FindProperty("stretchForce");
        shearForceProp = serializedObject.FindProperty("shearForce");
        bendForceProp = serializedObject.FindProperty("bendForce");
    }
 

}
*/