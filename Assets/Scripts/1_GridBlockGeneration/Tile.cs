using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    // This method initializes the tile with its position in the grid.
    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
