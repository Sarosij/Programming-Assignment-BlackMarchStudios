using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Manages the generation and initialization of a grid of tiles.
// </summary>

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float spacing = 1.1f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculating the position with spacing
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                
                // Instantiating a new tile at the calculated position
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                
                // Initializing the tile with its position
                tile.GetComponent<Tile>().Initialize(x, y);

                // Seting the name of the tile for easy identification in the hierarchy
                tile.name = $"Tile_{x}_{y}";
            }
        }
    }
}
