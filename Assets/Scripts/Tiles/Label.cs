using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Label : MonoBehaviour
{
    private TextMeshPro label;
    public Vector2Int cords;
    private GridScript gridScript;

    private void Awake()
    {
        gridScript = FindObjectOfType<GridScript>();
        label = GetComponentInChildren<TextMeshPro>();
        label.enabled = false;
        
        if (!gridScript)
        {
            return;
        }
        
        DisplayCords();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            label.enabled = true;
        }
        
        DisplayCords();
        transform.name = cords.ToString();
    }

    private void DisplayCords()
    {
        cords.x = Mathf.RoundToInt(transform.position.x / gridScript.UnityGridSize);
        cords.y = Mathf.RoundToInt(transform.position.z / gridScript.UnityGridSize);

        label.text = $"{cords.x},{cords.y}";
    }
}
