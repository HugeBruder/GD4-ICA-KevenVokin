using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public bool blocked;
    
    public Vector2Int cords;

    private GridScript gridScript;

    private void Start()
    {
        SetCords();

        if (blocked)
        {
            gridScript.BlockNode(cords);
        }
    }

    private void SetCords()
    {
        gridScript = FindObjectOfType<GridScript>();
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        cords = new Vector2Int(x / gridScript.UnityGridSize, z / gridScript.UnityGridSize);
    }
}
