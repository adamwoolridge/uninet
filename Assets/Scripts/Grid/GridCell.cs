using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell
{
    public Bounds BoundingBox;
    public HashSet<NetworkEntity> Entities;
    public List<NetworkEntity> Listeners;

    public GridCell(Vector3 center, float size)
    {
        BoundingBox = new Bounds(center, new Vector3(size,0f, size));
        Entities = new HashSet<NetworkEntity>();
        Listeners = new List<NetworkEntity>();
    }

    public void OnEntityEnter(NetworkEntity entity)
    {
        if (Entities.Contains(entity)) return;

        Entities.Add(entity);
        

        // Tell the listening entities this entity has entered the grid cell
        foreach (NetworkEntity ent in Listeners)
        {            
            NetworkManager.Instance.SendEntity(ent.clientID, entity);
        }

        // HACK, means a player for now.
        if (!entity.locallyControlled) 
        {
            // Tell the new entity about all the other entities in the cell
            NetworkManager.Instance.SendEntities(entity.clientID, Entities);
            if (!Listeners.Contains(entity))
                Listeners.Add(entity);
        }                                
    }

    public void OnEntityExit(NetworkEntity entity)
    {
        if (!Entities.Contains(entity)) return;

        Entities.Remove(entity);

        // HACK, means a player for now.
        if (!entity.locallyControlled)
        {
            Listeners.Remove(entity);
        }
              
        foreach (NetworkEntity ent in Listeners)
        {            
            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_Destroy);
            msg.Write(entity.networkable.ID);
            NetworkManager.Instance.Send(ent.clientID.ConnectionID, msg);            
        }
    }
}
