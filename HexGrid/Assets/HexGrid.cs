using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Serialization;

[Serializable]
public class HexGrid
{
	public HexGridOrientation gridOrientation;
	//Map settings
	public HexGridShape hexGridShape = HexGridShape.Rectangle;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public HexCellOrientation cellOrientation = HexCellOrientation.FlatOdd;
	public float hexRadius = 1;
	
	//Internal variables
	public Dictionary<string, HexCell> grid = new Dictionary<string, HexCell>();
	
	private readonly HexCubeCoordinate[] _directions = 
		{                                      // Pointy 		// Flat
			new HexCubeCoordinate(1, -1, 0), // right 			// bottom right
			new HexCubeCoordinate(1, 0, -1), // top right 		// top right
			new HexCubeCoordinate(0, 1, -1), // top left 		// top
			new HexCubeCoordinate(-1, 1, 0), // left 			// top left
			new HexCubeCoordinate(-1, 0, 1), // bottom left 	// bottom left
			new HexCubeCoordinate(0, -1, 1) // bottom right 	// bottom
		}; 
	
	#region Public Methods
	public void GenerateGrid() {
		
		grid.Clear();
		
		//Generate the grid shape
		switch(hexGridShape) 
		{
		case HexGridShape.Hexagon:
			GenHexShape();
			break;

		case HexGridShape.Rectangle:
			GenRectShape();
			break;

		case HexGridShape.Parrallelogram:
			GenParrallShape();
			break;

		case HexGridShape.Triangle:
			GenTriShape();
			break;
		}
	}

	public void CellAt(ref HexCubeCoordinate coordinate, out HexCell cell)
	{
		grid.TryGetValue(coordinate.ToString(), out cell);
	}

	public void CellAt(int x, int y, int z, out HexCell cell)
	{
		var c = new HexCubeCoordinate(x, y, z);
		CellAt(ref c, out cell);
	}

	public void CellAt(int x, int z, out HexCell cell)
	{
		var c = new HexCubeCoordinate(x, z);
		CellAt(ref c, out cell);
	}

	public void Neighbours(ref HexCell hexCell, List<HexCell> neighbours) 
	{
		neighbours.Clear();
		
		for(var i = 0; i < 6; i++)
		{
			var o = hexCell.coordinates + _directions[i];
			if(grid.ContainsKey(o.ToString()))
				neighbours.Add(grid[o.ToString()]);
		}
	}

	public void Neighbours(ref HexCubeCoordinate coordinate, List<HexCell> neighbours)
	{
		CellAt(ref coordinate, out var cell);
		Neighbours(ref cell, neighbours);
	}

	public void Neighbours(int x, int y, int z, List<HexCell> neighbours)
	{
		CellAt(x, y, z, out var cell);
		Neighbours(ref cell, neighbours);
	}

	public void Neighbours(int x, int z, List<HexCell> neighbours)
	{
		CellAt(x, z, out var cell);
		Neighbours(ref cell, neighbours);
	}

	public void CellsInRange(ref HexCell center, int range, List<HexCell> neighbours)
	{
		//Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range

		for(int dx = -range; dx <= range; dx++){
			for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++)
			{
				var o = new HexCubeCoordinate(dx, dy, -dx-dy) + center.coordinates;
				if(grid.TryGetValue(o.ToString(), out var cell))
					neighbours.Add(cell);
			}
		}
	}

	public void CellsInRange(ref HexCubeCoordinate coordinate, int range, List<HexCell> neighbours)
	{
		CellAt(ref coordinate, out var cell);
		CellsInRange(ref cell, range, neighbours);
	}

	public void CellsInRange(int x, int y, int z, int range, List<HexCell> neighbours)
	{
		CellAt(x, y, z, out var cell);
		CellsInRange(ref cell, range, neighbours);
	}

	public void CellsInRange(int x, int z, int range, List<HexCell> neighbours)
	{
		CellAt(x, z, out var cell);
		CellsInRange(ref cell, range, neighbours);
	}

	public int Distance(HexCubeCoordinate a, HexCubeCoordinate b){
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
	}

	public int Distance(HexCell a, HexCell b){
		return Distance(a.coordinates, b.coordinates);
	}
	#endregion

	#region Private Methods


	private void GenHexShape() {
		
		int mapSize = Mathf.Max(mapWidth, mapHeight);
		
		for (int q = -mapSize; q <= mapSize; q++)
		{
			int r1 = Mathf.Max(-mapSize, -q-mapSize);
			int r2 = Mathf.Min(mapSize, -q+mapSize);
			for(int r = r1; r <= r2; r++)
			{
				var cell = new HexCell {coordinates = new HexCubeCoordinate(q,r) };
				grid.Add(cell.coordinates.ToString(), cell);
			}
		}
	}

	private void GenRectShape() {
		Debug.Log ("Generating rectangular shaped grid...");
		
		switch(cellOrientation){
		case HexCellOrientation.FlatOdd:
			for(int q = 0; q < mapWidth; q++)
			{
				int qOff = q>>1;
				for (int r = -qOff; r < mapHeight - qOff; r++)
				{
					var cell = new HexCell {coordinates = new HexCubeCoordinate(q,r) };
					grid.Add(cell.coordinates.ToString(), cell);
				}
			}
			break;
			
		case HexCellOrientation.PointyOdd:
			for(int r = 0; r < mapHeight; r++)
			{
				int rOff = r>>1;
				for (int q = -rOff; q < mapWidth - rOff; q++)
				{
					var cell = new HexCell {coordinates = new HexCubeCoordinate(q,r) };
					grid.Add(cell.coordinates.ToString(), cell);
				}
			}
			break;
		}
	}

	private void GenParrallShape() {
		Debug.Log ("Generating parrellelogram shaped grid...");

		for (int q = 0; q <= mapWidth; q++)
		{
			for(int r = 0; r <= mapHeight; r++)
			{
				var cell = new HexCell {coordinates = new HexCubeCoordinate(q,r) };
				grid.Add(cell.coordinates.ToString(), cell);
			}
		}
	}

	private void GenTriShape() {
		Debug.Log ("Generating triangular shaped grid...");
		
		int mapSize = Mathf.Max(mapWidth, mapHeight);

		for (int q = 0; q <= mapSize; q++)
		{
			for(int r = 0; r <= mapSize - q; r++)
			{
				var cell = new HexCell {coordinates = new HexCubeCoordinate(q,r) };
				grid.Add(cell.coordinates.ToString(), cell);
			}
		}
	}
	
	#endregion
}

[System.Serializable]
public enum HexGridShape {
	Rectangle,
	Hexagon,
	Parrallelogram,
	Triangle
}

public enum HexCellOrientation {
	PointyOdd,
	PointyEven,
	FlatOdd,
	FlatEven
}

public enum HexGridOrientation
{
	Horizontal,
	Vertical
}
