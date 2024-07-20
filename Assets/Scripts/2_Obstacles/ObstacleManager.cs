using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// <summary>
// Manages the generation of obstacles on the grid based on the obstacle data.
// </summary>

public class ObstacleManager : MonoBehaviour
{
    public ObstacleData obstacleData;
    public GameObject obstaclePrefab;

    void Start()
    {
        GenerateObstacles(); // Generate obstacles on the grid at the start
    }

    // Generates obstacles on the grid based on the data in the ObstacleData ScriptableObject.
    void GenerateObstacles()
    {
        for (int i = 0; i < obstacleData.obstacles.Length; i++)
        {
            if (obstacleData.obstacles[i]) 
            {
                int x = i % 10; // Calculate the x coordinate in the grid
                int y = i / 10; // Calculate the y coordinate in the grid
                Vector3 position = new Vector3(x * 1.1f, 0.5f, y * 1.1f); // Calculate the position in the world space
                Instantiate(obstaclePrefab, position, Quaternion.identity);
            }
        }
    }
}
