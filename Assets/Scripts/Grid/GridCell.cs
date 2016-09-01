using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell
{
    public Bounds BoundingBox;
    public HashSet<NetworkEntity> Entities;
    
    public GridCell(Vector3 center, float size)
    {
        BoundingBox = new Bounds(center, new Vector3(size,0f, size));
        Entities = new HashSet<NetworkEntity>();
    }

    public void OnEntityEnter(NetworkEntity entity)
    {
        if (Entities.Contains(entity)) return;

        Entities.Add(entity);
        
        // Now notify shit

    }

    public void OnEntityExit(NetworkEntity entity)
    {
        if (!Entities.Contains(entity)) return;

        Entities.Remove(entity);

        // Now notify shit
    }
}
