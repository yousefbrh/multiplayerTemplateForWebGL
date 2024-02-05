using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class RemoveMissingMonoBehavioursEditorWindow : EditorWindow
{
    private float _space = 20f;

    [MenuItem("INEX/Remove Missing MonoBehaviours Window")]
    public static void CustomEditorWindow() { GetWindow<RemoveMissingMonoBehavioursEditorWindow>("Remove Missing MonoBehaviours"); }

    private void OnGUI()
    {
        GUILayout.Label("Welcome to INEX custom editor for removing missing MonoBehaviours", EditorStyles.largeLabel);
        GUILayout.Label("This editor allows you to remove all missing references from your project with one click only, happy coding ;)");
        GUILayout.Space(_space);
        GUILayout.Label("Available Methods:", EditorStyles.boldLabel);
        if (GUILayout.Button("Remove From Selected Objects")) { RemoveMissingMonoBehaviours(Selection.gameObjects); }
        if (GUILayout.Button("Remove From All Scene GameObjects")) { RemoveMissingMonoBehaviours(FindObjectsOfType<GameObject>()); }
    }

    private void RemoveMissingMonoBehaviours(GameObject[] objects)
    {
        int missingMonoBehaviorsCount = 0;
        foreach (GameObject go in objects) { missingMonoBehaviorsCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go); }
        Debug.Log("Removed " + missingMonoBehaviorsCount + " Missing MonoBehaviours from " + objects.Length + " GameObjects" );
    }
}
