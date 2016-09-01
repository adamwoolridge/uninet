using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public static Grid Instance;
    public List<GridCell> Cells;

    private float cellSize;
    private int xCells;
    private int yCells;    
    
    public Grid(int width, int depth, float size)
    {
        if (Instance!=null)
        {
            Debug.LogError("Trying to initialise another Grid!");
            return;
        }

        Instance = this;

        xCells = width;
        yCells = depth;
        cellSize = size;

        Cells = new List<GridCell>();

        for (int y=0; y<depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridCell cell = new GridCell( new Vector3((x*cellSize)+(cellSize/2f), 0f,(y*cellSize)+(cellSize/2f)), cellSize );
                Cells.Add(cell);
            }
        }
    }

    public GridCell GetCell(Vector3 pos)
    {
        int x = (int)(pos.x/cellSize);
        int y = (int)(pos.z/cellSize);

        if (x < 0 || x >= xCells || y < 0 || y >= yCells)
        {
            Debug.LogWarning("Grid::GetCell trying to access an out of bounds position.");
            return null;
        }

        return Cells[(y * yCells) + x];
    }

    public void TransferEntity(NetworkEntity ent, GridCell oldCell, GridCell newCell)
    {
        if (ent == null) return;
        ent.gridCell = newCell;

        if (oldCell!=null)        
            oldCell.OnEntityExit(ent);        

        if (newCell!=null)        
            newCell.OnEntityEnter(ent);                                   
    }
}
