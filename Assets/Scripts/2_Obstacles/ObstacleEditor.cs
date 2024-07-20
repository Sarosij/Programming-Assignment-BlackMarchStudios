using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// <summary>
// Custom editor window for creating and editing obstacle data on a 10x10 grid.
// </summary>
public class ObstacleEditor : EditorWindow
{
    private ObstacleData obstacleData;

    // Adds the "Obstacle Editor" option to the Unity Tools menu.
    [MenuItem("Tools/Obstacle Editor")]
    public static void ShowWindow()
    {
        // Displays the custom editor window
        GetWindow<ObstacleEditor>("Obstacle Editor");
    }

    // Draws the custom editor GUI.
    void OnGUI()
    {
        if (obstacleData == null)
        {
            if (GUILayout.Button("Create Obstacle Data"))
            {
                // Creates a new ObstacleData instance and saves it as an asset
                obstacleData = CreateInstance<ObstacleData>();
                AssetDatabase.CreateAsset(obstacleData, "Assets/ObstacleData.asset");
                AssetDatabase.SaveAssets();
            }


            // Allow the user to assign an existing ObstacleData asset
            obstacleData = (ObstacleData)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleData), false);
        }

         // If obstacle data is assigned, draw the grid of toggle buttons
        if (obstacleData != null)
        {
            // Creates a serialized object for the obstacle data to handle undo and redo operations
            SerializedObject serializedObject = new SerializedObject(obstacleData);
            SerializedProperty obstaclesProperty = serializedObject.FindProperty("obstacles");

            // Draws a 10x10 grid of toggle buttons to represent the obstacles
            for (int y = 0; y < 10; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < 10; x++) 
                {
                    int index = x * 10 + y;
                    obstaclesProperty.GetArrayElementAtIndex(index).boolValue = EditorGUILayout.Toggle(obstaclesProperty.GetArrayElementAtIndex(index).boolValue, GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();
            }

            // Save the obstacle data asset when the save button is clicked
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Save Obstacle Data"))
            {
                EditorUtility.SetDirty(obstacleData);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
