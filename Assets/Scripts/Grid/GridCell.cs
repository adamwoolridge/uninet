using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell
{
    public Bounds BoundingBox;
    public HashSet<NetworkEntity> Entities;
    public List<NetworkEntity> Listeners;
    public uint Index;    

    public GridCell(Vector3 center, float size, uint index)
    {
        BoundingBox = new Bounds(center, new Vector3(size,0f, size));
        Entities = new HashSet<NetworkEntity>();
        Listeners = new List<NetworkEntity>();
        Index = index;        
    }

    public void OnEntityEnter(NetworkEntity entity)
    {
        if (Entities.Contains(entity)) return;

        Entities.Add(entity);      
    }

    public void OnEntityExit(NetworkEntity entity)
    {
        if (!Entities.Contains(entity)) return;

        Entities.Remove(entity);
    }
}
