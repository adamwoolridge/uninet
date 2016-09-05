﻿using UnityEngine;
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

        uint index = 0;
        for (int y=0; y<depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridCell cell = new GridCell( new Vector3((x*cellSize)+(cellSize/2f), 0f,(y*cellSize)+(cellSize/2f)), cellSize, index);
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


        // TODO: make this all work with multiple groups for propper ranged visibility

        // Tell old cell listeners to destroy this entity        
        if (oldCell != null)
        {
            oldCell.Listeners.Remove(ent);

            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_Destroy);
            msg.Write(ent.networkable.ID);
            foreach (NetworkEntity e in oldCell.Listeners)
            {
                NetworkManager.Instance.Send(e.clientID.ConnectionID, msg);
            }

            // Tell the entity to delete all entities in the old cell
            NetworkMessage delCellMsg = new NetworkMessage(NetworkMessageType.Cell_Destroy);
            delCellMsg.Write(oldCell.Index);
            NetworkManager.Instance.Send(ent.clientID.ConnectionID, delCellMsg);
        }

        // Tell the new cell listeners about this entity
        if (newCell != null)
        {         
            foreach (NetworkEntity e in newCell.Listeners)
            {                
                NetworkManager.Instance.SendEntity(e.clientID, ent);
            }

            //// HACK, means a player for now.
            if (!ent.locallyControlled)
            {
                // Add new entity to listeners
                newCell.Listeners.Add(ent);

                // Tell the new entity about all the other entities in the cell
                NetworkManager.Instance.SendEntities(ent.clientID, newCell.Entities);
            }
        }
    }
}
