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
        if (Instance != null)
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
        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridCell cell = new GridCell( new Vector3((x*cellSize)+(cellSize/2f), 0f,(y*cellSize)+(cellSize/2f)), cellSize, index);
                Cells.Add(cell);
                index++;
            }
        }
    }

    public GridCell GetCell(int xIndex, int yIndex)
    {
        return Cells[(yIndex * yCells) + xIndex];
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

        return GetCell(x, y);
    }

    public List<GridCell> GetNeighbours(GridCell cell, int depth)
    {
        int yIndex = (int)cell.Index/xCells;
        int xIndex = (int)cell.Index - (yIndex*yCells);
        List<GridCell> neighbours = new List<GridCell>();

        for (int y = yIndex - depth; y < yIndex + depth + 1; y++)
        {
            for (int x = xIndex - depth; x < xIndex + depth + 1; x++)
            {
                if (x >= 0 && x < xCells && y >= 0 && y < yCells)
                {
                    neighbours.Add(GetCell(x, y));
                }
            }
        }
        return neighbours;
    }

    public void TransferEntity(NetworkEntity ent, GridCell oldCell, GridCell newCell)
    {
        if (ent == null) return;
        ent.gridCell = newCell;

        if (oldCell != null)
            oldCell.OnEntityExit(ent);

        if (newCell != null)
            newCell.OnEntityEnter(ent);

        UpdateCellEntities(ent, oldCell, newCell);
        NotifyCellListeners(ent, oldCell, newCell);
    }

    // Figure out which entities we need to send and delete based on the new grid cells this entity can see
    private void UpdateCellEntities(NetworkEntity entity, GridCell oldCell, GridCell newCell)
    {
        List<GridCell> oldVisCells = oldCell != null ? GetNeighbours(oldCell, 2) : null;
        List<GridCell> newVisCells = newCell != null ? GetNeighbours(newCell, 2) : null;
        List<GridCell> leaveCells;
        List<GridCell> enterCells;
        List<GridCell> remainCells;
        Utils.CategorizeDifferences<GridCell>(oldVisCells, newVisCells, out enterCells, out leaveCells, out remainCells);

        if (!entity.locallyControlled)
        {
            // Send any entities from any new cells this entity can now see
            if (enterCells != null)
            {
                foreach (GridCell cell in enterCells)
                {
                    NetworkManager.Instance.SendEntities(entity.clientID, cell.Entities);

                    if (!cell.Listeners.Contains(entity))
                        cell.Listeners.Add(entity);
                }
            }

            // Destroy any entities from any old cells this entity can no longer see
            if (leaveCells != null)
            {
                foreach (GridCell cell in leaveCells)
                {
                    NetworkMessage delCellMsg = new NetworkMessage(NetworkMessageType.Cell_Destroy);
                    delCellMsg.Write(cell.Index);
                    NetworkManager.Instance.Send(entity.clientID.ConnectionID, delCellMsg);

                    cell.Listeners.Remove(entity);
                }
            }
        }
    }

    // Figure out which listeners for the old and new cells need to be told, and how, about the change
    private void NotifyCellListeners(NetworkEntity entity, GridCell oldCell, GridCell newCell)
    {
        List<NetworkEntity> oldCellListeners = oldCell != null ? oldCell.Listeners : null;
        List<NetworkEntity> newCellListeners = newCell != null ? newCell.Listeners : null;
        List<NetworkEntity> destroy;
        List<NetworkEntity> update;
        List<NetworkEntity> noChange;

        Utils.CategorizeDifferences<NetworkEntity>(oldCellListeners, newCellListeners, out update, out destroy, out noChange);

        if (update != null)
        {
            // These guys never knew about this entity. Tell them.
            foreach (NetworkEntity listenEnt in update)
            {
                if (listenEnt == entity) continue;
                NetworkManager.Instance.SendEntity(listenEnt.clientID, entity);
            }
        }

        if (destroy != null)
        {
            // These guys knew about this entity but now don't need to. Tell them to destroy it.
            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_Destroy);
            msg.Write(entity.networkable.ID);
            foreach (NetworkEntity listenEnt in destroy)
            {
                if (listenEnt == entity) continue;
                NetworkManager.Instance.Send(listenEnt.clientID.ConnectionID, msg);
            }
        }
    }
}
