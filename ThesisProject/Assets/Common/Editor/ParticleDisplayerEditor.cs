using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/*
[CustomEditor(typeof(ParticleDisplayer))]
public class ParticleDisplayerEditor : Editor
{
    // Main script
    ParticleDisplayer displayer;

    // Properties to draw
    SerializedProperty scriptProp;
    SerializedProperty sizeProp, drawSpringInteractionsProp;


    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        
        bool needToRespawn = false;
        bool needToRedraw = false;

        serializedObject.Update();

        // Add script at top (not changeable)
        GUI.enabled = false;
        EditorGUILayout.PropertyField(scriptProp);
        GUI.enabled = true;

        // Size Slider
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(sizeProp);
        if (EditorGUI.EndChangeCheck()) {
            needToRedraw = true;
        }

        // Draw Default Interactions Bool
        EditorGUILayout.PropertyField(drawSpringInteractionsProp);


        // Add buttons at the end
        if (GUILayout.Button("Reset Particles")) {
            needToRespawn = true;
        }


        // Finish Setting up Property Fields
        serializedObject.ApplyModifiedProperties();


        // Respawn or Redraw if needed
        if (needToRespawn) {
            displayer.SpawnParticles();
        }
        else if (needToRedraw) {
            displayer.RedrawParticles();
        }
        
    }


    private void OnEnable() {
        // Setup script and properties
        displayer = (ParticleDisplayer) target;

        scriptProp = serializedObject.FindProperty("script");
        sizeProp = serializedObject.FindProperty("size");
        drawSpringInteractionsProp = serializedObject.FindProperty("drawSpringInteractions");
    }

}
*/