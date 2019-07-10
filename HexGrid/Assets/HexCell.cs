using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCell
{
    public HexCubeCoordinate coordinates;
    
    public static Vector3 Corner(Vector3 origin, float radius, int corner, HexCellOrientation cellOrientation, HexGridOrientation gridOrientation){
        float angle = 60 * corner;
        if(cellOrientation == HexCellOrientation.PointyOdd)
            angle += 30;
        angle *= Mathf.PI / 180;
		
        var cornerPos = new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));

        if (gridOrientation != HexGridOrientation.Vertical) return cornerPos;
		
        cornerPos.y = origin.y + radius * Mathf.Sin(angle);
        cornerPos.z = 0;

        return cornerPos;
    }
   
}

[System.Serializable]
public struct OffsetCoordinate 
{
    public int row;
    public int col;

    public OffsetCoordinate(int row, int col){
        this.row = row; this.col = col;
    }

    public void GetCubeCoordinates(HexCellOrientation orientation, out HexCubeCoordinate c)
    {
        OffsetToCube(ref this, orientation, out c);
    }
    
    public override string ToString ()
    {
        return $"{col}:{row}";
    }

    public static void OffsetToCube(ref OffsetCoordinate o, HexCellOrientation orientation, out HexCubeCoordinate c)
    {
        switch (orientation)
        {
            case HexCellOrientation.PointyOdd:
                PointyOddToCube(ref o, out c);
                break;
            case HexCellOrientation.PointyEven:
                PointyEvenToCube(ref o, out c);
                break;
            case HexCellOrientation.FlatOdd:
                FlatOddToCube(ref o, out c);
                break;
            case HexCellOrientation.FlatEven:
                FlatEvenToCube(ref o, out c);
                break;
            default:
                PointyOddToCube(ref o, out c);
                break;
        }
    }
    
    private static void PointyOddToCube(ref OffsetCoordinate o, out HexCubeCoordinate c)
    {
        c.x = o.col - (o.row - (o.row & 1)) / 2;
        c.z = o.row;
        c.y = -c.x - c.z;
    }
    
    private static void PointyEvenToCube(ref OffsetCoordinate o, out HexCubeCoordinate c)
    {
        c.x = o.col - (o.row + (o.row & 1)) / 2;
        c.z = o.row;
        c.y = -c.x - c.z;
    }
    
    private static void FlatOddToCube(ref OffsetCoordinate o, out HexCubeCoordinate c)
    {
        c.x = o.col;
        c.z = o.row - (o.col - (o.col&1)) / 2;
        c.y = -c.x - c.z;
    }
    
    private static void FlatEvenToCube(ref OffsetCoordinate o, out HexCubeCoordinate c)
    {
        c.x = o.col;
        c.z = o.row - (o.col + (o.col&1)) / 2;
        c.y = -c.x - c.z;
    }
}

[System.Serializable]
public struct HexCubeCoordinate {
    public int x;
    public int y;
    public int z;

    public HexCubeCoordinate(int x, int y, int z){
        this.x = x; this.y = y; this.z = z;
    }

    public HexCubeCoordinate(int x, int z) {
        this.x = x; this.z = z; this.y = -x-z;
    }

    public static HexCubeCoordinate operator+ (HexCubeCoordinate one, HexCubeCoordinate two){
        return new HexCubeCoordinate(one.x + two.x, one.y + two.y, one.z + two.z);
    }

    public override bool Equals (object obj) {
        if(obj == null)
            return false;
        var o = (HexCubeCoordinate)obj;
        return((x == o.x) && (y == o.y) && (z == o.z));
    }

    public override int GetHashCode () {
        return(x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5))/2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
    }

    public override string ToString ()
    {
        return $"{x}:{y}:{z}";
    }

    public void GetOffsetCoordinates(HexCellOrientation orientation, out OffsetCoordinate o)
    {
        CubeToOffset(ref this, orientation, out o);
    }

    public static void CubeToOffset(ref HexCubeCoordinate c, HexCellOrientation orientation, out OffsetCoordinate o)
    {
        switch (orientation)
        {
            case HexCellOrientation.PointyOdd:
                CubeToPointyOdd(ref c, out o);
                break;
            case HexCellOrientation.PointyEven:
                CubeToPointyEven(ref c, out o);
                break;
            case HexCellOrientation.FlatOdd:
                CubeToFlatOdd(ref c, out o);
                break;
            case HexCellOrientation.FlatEven:
                CubeToFlatEven(ref c, out o);
                break;
            default:
                CubeToPointyOdd(ref c, out o);
                break;
        }
    }
    
    private static void CubeToPointyOdd(ref HexCubeCoordinate c, out OffsetCoordinate o) 
    {
        o.row = c.z;
        o.col = c.x + (c.z - (c.z&1)) / 2;
    }
    
    private static void CubeToPointyEven(ref HexCubeCoordinate c, out OffsetCoordinate o)
    {
        o.row = c.z;
        o.col = c.x + (c.z + (c.z&1)) / 2;
    }
    
    private static void CubeToFlatOdd(ref HexCubeCoordinate c, out OffsetCoordinate o)
    {
        o.col = c.x;
        o.row = c.z + (c.x - (c.x&1)) / 2;
    }
    
    private static void CubeToFlatEven(ref HexCubeCoordinate c, out OffsetCoordinate o)
    {
        o.row = c.x;
        o.col = c.z + (c.x + (c.x&1)) / 2;
    }
}
