using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridTest : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject childTemplate;
    [SerializeField] private Vector2Int gridSize;
    private void OnValidate()
    {
        if (!grid)
            grid = GetComponent<Grid>();

        if (!childTemplate)
            childTemplate = transform.GetChild(0)?.gameObject;
    }

    [ContextMenu("BuildGrid")]
    private void BuildGrid()
    {
        if(!grid) return;
        if(!childTemplate) return;
        
        childTemplate.SetActive(false);

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            
            if(child == childTemplate.transform) continue;
            
            DestroyImmediate(child.gameObject);
        }
      
        for (var y = 0; y < gridSize.y; y++)
        {
            var xMax = y % 2 == 1 ? gridSize.x - 1: gridSize.x; 
            for (var x = 0; x < xMax; x++)
            {
                var childClone = Instantiate(childTemplate, transform);
                childClone.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
                childClone.gameObject.SetActive(true);
                var label = childClone.GetComponentInChildren<TextMeshPro>();
                if(!label) continue;
                label.text = $@"{x}, {y}";
         
            }
        }
        
        
    }
    
}
