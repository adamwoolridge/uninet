using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Server only
public static class EntityManager
{
    static Dictionary<uint, NetworkEntity> entities = new Dictionary<uint, NetworkEntity>();
    static uint nextID = 0;

    public static NetworkEntity GetNew()
    {
        NetworkEntity newEntity = new NetworkEntity(nextID);        
        entities.Add(nextID, newEntity);
        nextID++;
        return newEntity;
    }
}
