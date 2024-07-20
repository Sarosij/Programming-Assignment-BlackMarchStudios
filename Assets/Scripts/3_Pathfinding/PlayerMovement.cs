using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// <summary>
// This script handles the movement of the player on a grid.
// The player can click on any tile to move to it,
// provided that the tile is not blocked by an obstacle.
// It uses the Breadth-First Search (BFS) algorith for pathfinding
// </summary>

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject obstacleManager;

    private Vector3 targetPosition; // Target position for player to Moveto
    private bool isMoving = false;
    private ObstacleManager obstacleManagerScript;

    void Start()
    {
        obstacleManagerScript = obstacleManager.GetComponent<ObstacleManager>();
        targetPosition = transform.position; // initial target position to the player's starting position
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && !IsTileBlocked(new Vector2Int(tile.x, tile.y)))
                {
                    List<Vector3> path = FindPath(transform.position, hit.collider.transform.position);
                    if (path != null)
                    {
                        StartCoroutine(MoveAlongPath(path));
                    }
                }
            }
        }
    }

    // Checks if a tile at the given grid position is blocked by an obstacle.
    // Returns true if the tile is blocked, false otherwise.
    bool IsTileBlocked(Vector2Int gridPosition)
    {
        int index = gridPosition.y * 10 + gridPosition.x;
        return obstacleManagerScript.obstacleData.obstacles[index];
    }


    // Finding a path from the start position to the target position using the Breadth-First Search (BFS) algorithm.

    List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Converting world positions to grid positions
        Vector2Int startGridPos = new Vector2Int((int)(startPos.x / 1.01f), (int)(startPos.z / 1.01f));
        Vector2Int targetGridPos = new Vector2Int((int)(targetPos.x / 1.01f), (int)(targetPos.z / 1.01f));

         // Initializing the queue for BFS and add the starting grid position
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startGridPos);

        // Dictionary to track the path
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[startGridPos] = startGridPos;

        // Performing BFS to find the shortest path
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // If the target position is reached, reconstruct the path
            if (current == targetGridPos)
            {
                List<Vector3> path = new List<Vector3>();
                while (current != startGridPos)
                {
                    path.Add(new Vector3(current.x * 1.1f, 0, current.y * 1.1f));
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

             // Explore the neighbors of the current position
            foreach (Vector2Int next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next) && !IsTileBlocked(next))
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }
        return null; // No path found
    }


    // Returns the valid neighbors (up, down, left, right) of a given grid position.
    //An IEnumerable of Vector2Int representing the neighbors.

    IEnumerable<Vector2Int> GetNeighbors(Vector2Int gridPos)
    {
        // Possible directions to move in the grid
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        // Iterate through the directions and yield valid neighbors
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = gridPos + dir;
            if (neighbor.x >= 0 && neighbor.x < 10 && neighbor.y >= 0 && neighbor.y < 10)
            {
                yield return neighbor;
            }
        }
    }

    
    // Coroutine to move the player along a given path.
    // Returns ann IEnumerator for the coroutine.
    IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;

        // Iterate through the waypoints in the path
        foreach (Vector3 waypoint in path)
        {
            // Move towards each waypoint until the position is reached
            while (transform.position != waypoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
    }
}
