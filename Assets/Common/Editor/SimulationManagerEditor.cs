using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SimulationManager))]
public class SimulationManagerEditor : Editor {
    
    // Main script
    SimulationManager simulationManager;

    SerializedProperty scriptProp;
    SerializedProperty simulationModeProp, particleMassProp;
    SerializedProperty verletConstantsProp, eulerConstantsProp;

    SerializedProperty globalEquilibriumDistanceProp, globalForceConstantProp;
    SerializedProperty stretchForceProp, shearForceProp, bendForceProp;

    SerializedProperty gravityForceProp, windForceProp;



    public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        serializedObject.Update();

        // Add script at top (not changeable)
        GUI.enabled = false;
        EditorGUILayout.PropertyField(scriptProp);
        GUI.enabled = true;

        // General settings
        EditorGUILayout.PropertyField(simulationModeProp);
        EditorGUILayout.PropertyField(particleMassProp);

        switch (simulationManager.simulationMode) {
            case SimulationManager.SimulationMode.Verlet:
                EditorGUILayout.PropertyField(verletConstantsProp);
                break;
            case SimulationManager.SimulationMode.Euler:
                EditorGUILayout.PropertyField(eulerConstantsProp);
                break;
        }

        // Global Spring Settings
        EditorGUILayout.PropertyField(globalEquilibriumDistanceProp);
        EditorGUILayout.PropertyField(globalForceConstantProp);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Current Values")) {
            simulationManager.UpdateDefaultSpringValues();
            simulationManager.ResetSpringValues();
        }
        if (GUILayout.Button("Update Default Values")) {
            simulationManager.UpdateDefaultSpringValues();
        }
        GUILayout.EndHorizontal();

        // Stretch Spring Settings
        EditorGUILayout.PropertyField(stretchForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (simulationManager.stretchForce.enabled && stretchForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Stretch Spring Values")) {
                simulationManager.stretchForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }

        // Shear Spring Settings
        EditorGUILayout.PropertyField(shearForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (simulationManager.shearForce.enabled && shearForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Shear Spring Values")) {
                simulationManager.shearForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }

        // Bend Spring Settings
        EditorGUILayout.PropertyField(bendForceProp);
        // We have to add button manually due to limitations of the custom property drawer
        if (simulationManager.bendForce.enabled && bendForceProp.isExpanded) {
            EditorGUI.indentLevel++;
            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Reset Bend Spring Values")) {
                simulationManager.bendForce.ResetDefaultValues();
            }
            EditorGUI.indentLevel--;
        }

        // External forces
        EditorGUILayout.PropertyField(gravityForceProp);
        EditorGUILayout.PropertyField(windForceProp);

        // Finish Setting up Property Fields
        serializedObject.ApplyModifiedProperties();

    }

    private void OnEnable() {
        // Setup script and properties
        simulationManager = (SimulationManager) target;
        scriptProp = serializedObject.FindProperty("m_Script");

        simulationModeProp = serializedObject.FindProperty("simulationMode");
        particleMassProp = serializedObject.FindProperty("particleMass");

        verletConstantsProp = serializedObject.FindProperty("verletConstants");
        eulerConstantsProp = serializedObject.FindProperty("eulerConstants");

        globalEquilibriumDistanceProp = serializedObject.FindProperty("globalEquilibriumDistance");
        globalForceConstantProp = serializedObject.FindProperty("globalForceConstant");

        stretchForceProp = serializedObject.FindProperty("stretchForce");
        shearForceProp = serializedObject.FindProperty("shearForce");
        bendForceProp = serializedObject.FindProperty("bendForce");

        gravityForceProp = serializedObject.FindProperty("gravityForce");
        windForceProp = serializedObject.FindProperty("windForce");

    }



}


//*

// Custom Class for Drawer with foldout
[CustomPropertyDrawer(typeof(SpringForceSettings))]
public class SpringForceSettingsDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Info about enabled state to check foldout state
        SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
        bool enabledValue = enabledProp.boolValue;


        // Foldout + Label + Enabled bool
        if (enabledValue) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.PropertyField(position, enabledProp, GUIContent.none);  // Draw the enabled bool without the label

        // Draw remaining variables if expanded
        if (property.isExpanded && enabledValue) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("equilibriumDistance"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("forceConstant"));
            // We can't create a button here, due to limitations of the custom drawers (reaching the target instance)
            EditorGUI.indentLevel--;
        }
    }
}



// Custom Class for Drawer with foldout
[CustomPropertyDrawer(typeof(WindExternalForce))]
public class WindForceDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Info about enabled state to check foldout state
        SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
        bool enabledValue = enabledProp.boolValue;


        // Foldout + Label + Enabled bool
        if (enabledValue) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.PropertyField(position, enabledProp, GUIContent.none);  // Draw the enabled bool without the label

        // Draw remaining variables if expanded
        if (property.isExpanded && enabledValue) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("strength"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("frequency"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("direction"));
            // We can't create a button here, due to limitations of the custom drawers (reaching the target instance)
            EditorGUI.indentLevel--;
        }
    }
}


// Custom Class for Drawer with foldout
[CustomPropertyDrawer(typeof(GravityExternalForce))]
public class GravityForceDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Info about enabled state to check foldout state
        SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
        bool enabledValue = enabledProp.boolValue;


        // Foldout + Label + Enabled bool
        if (enabledValue) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.PropertyField(position, enabledProp, GUIContent.none);  // Draw the enabled bool without the label

        // Draw remaining variables if expanded
        if (property.isExpanded && enabledValue) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("gravity"));
            // We can't create a button here, due to limitations of the custom drawers (reaching the target instance)
            EditorGUI.indentLevel--;
        }
    }
}

// */