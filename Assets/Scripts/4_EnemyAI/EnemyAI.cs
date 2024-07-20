using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// <summary>
// This script handles the enemy AI, making the enemy move towards the player while avoiding obstacles.
// </summary>

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject obstacleManager;
    public GameObject player;

    private Vector3 targetPosition; // Target position for the enemy to move to
    private bool isMoving = false;
    private ObstacleManager obstacleManagerScript;

    void Start()
    {
        // Initializing the reference to the ObstacleManager script
        obstacleManagerScript = obstacleManager.GetComponent<ObstacleManager>();
        // Set initial target position to the starting position of the enemy
        targetPosition = transform.position; 
    }

    void Update()
    {
        // Checking if the enemy is not moving then moving towards player's position
        if (!isMoving)
        {
            MoveTowardsPlayer(player.transform.position);
        }
    }


    // Moves the enemy towards the player while avoiding obstacles.
    public void MoveTowardsPlayer(Vector3 playerPosition)
    {
        // Converting player's world position to grid position
        Vector2Int playerGridPos = new Vector2Int((int)(playerPosition.x / 1.1f), (int)(playerPosition.z / 1.1f));
        // Getting the valid neighboring tiles of the player
        List<Vector2Int> neighbors = GetNeighbors(playerGridPos).ToList();

        Vector2Int closestTile = playerGridPos;
        float closestDistance = float.MaxValue;

        // Finding the closest neighboring tile to move to
        foreach (Vector2Int neighbor in neighbors)
        {
            if (!IsTileBlocked(neighbor))
            {
                float distance = Vector3.Distance(transform.position, new Vector3(neighbor.x * 1.1f, 0, neighbor.y * 1.1f));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTile = neighbor;
                }
            }
        }

        // Finding the path to the closest tile
        List<Vector3> path = FindPath(transform.position, new Vector3(closestTile.x * 1.1f, 0, closestTile.y * 1.1f));
        if (path != null)
        {
            // Starting the movement coroutine if a path is found
            StartCoroutine(MoveAlongPath(path));
        }
    }


    // Checks if a tile at the given grid position is blocked by an obstacle.
    // Returns true if the tile is blocked, false otherwise.
    bool IsTileBlocked(Vector2Int gridPosition)
    {
        int index = gridPosition.y * 10 + gridPosition.x;
        return obstacleManagerScript.obstacleData.obstacles[index];
    }



    // Finds a path from the start position to the target position using the Breadth-First Search (BFS) algorithm.
    // Returns a list of Vector3 positions representing the path, or null if no path is found.
    List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
         // Convert world positions to grid positions
        Vector2Int startGridPos = new Vector2Int((int)(startPos.x / 1.1f), (int)(startPos.z / 1.1f));
        Vector2Int targetGridPos = new Vector2Int((int)(targetPos.x / 1.1f), (int)(targetPos.z / 1.1f));

        // Initialize the queue for BFS and add the starting grid position
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startGridPos);

        // Dictionary to track the path
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[startGridPos] = startGridPos;


        // Performing BFS to find the shortest path
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

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
    // Returns an IEnumerable of Vector2Int representing the neighbors.
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


    // Coroutine to move the enemy along a given path.
    // Returns an IEnumerator for the coroutine.
    IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;

        // Iterate through the waypoints in the path
        foreach (Vector3 waypoint in path)
        {
            while (transform.position != waypoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false; //// Set the isMoving flag to false once the path is completed
    }
}
