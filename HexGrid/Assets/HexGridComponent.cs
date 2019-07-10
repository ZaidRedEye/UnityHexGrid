using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class HexGridComponent : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Vector3 cellSizeRescale = Vector3.one;
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private GameObject hexObjectTemplate;
    private void OnValidate()
    {
        if (!grid)
            grid = GetComponent<Grid>();

        if (grid)
        {
            if (grid.cellLayout != GridLayout.CellLayout.Hexagon)
                grid.cellLayout = GridLayout.CellLayout.Hexagon;

            switch (hexGrid.gridOrientation)
            {
                case HexGridOrientation.Horizontal:
                    break;
                case HexGridOrientation.Vertical:
                    switch (hexGrid.cellOrientation)
                    {
                        case HexCellOrientation.PointyOdd:
                            if (grid.cellSwizzle != GridLayout.CellSwizzle.XYZ)
                                grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
                            break;
                        case HexCellOrientation.FlatOdd:
                            if (grid.cellSwizzle != GridLayout.CellSwizzle.YXZ)
                                grid.cellSwizzle = GridLayout.CellSwizzle.YXZ;
                            break;
                    }
                    break;
            }
            
            var rescaledSize = cellSizeRescale * hexGrid.hexRadius;
            if (grid.cellSize != rescaledSize)
                grid.cellSize = rescaledSize;
        }
    }
    [ContextMenu("GenerateGrid")]
    private void GenerateGrid()
    {
        hexGrid.GenerateGrid();

        foreach (var cell in hexGrid.grid.Values)
        {
            cell.coordinates.GetOffsetCoordinates(hexGrid.cellOrientation, out var offset);
            var pos = grid.GetCellCenterWorld(new Vector3Int(offset.col, -offset.row, 0));
            var spawned = Instantiate(hexObjectTemplate, pos, Quaternion.identity);
            spawned.gameObject.SetActive(true);
            spawned.name = cell.coordinates.ToString();
            var label = spawned.GetComponentInChildren<TextMeshPro>();
            if(label) label.text = spawned.name;
        }
    }
}
