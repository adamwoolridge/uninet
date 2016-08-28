﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EntityManager
{
    public static Dictionary<uint, NetworkEntity> entities = new Dictionary<uint, NetworkEntity>();
    static uint nextID = 0;

    public static uint Register(NetworkEntity netEnt)
    {
        if (NetworkManager.IsServer)
        {
            netEnt.networkable = new Networkable(nextID);            
            nextID++;
        }

        entities.Add(netEnt.networkable.ID, netEnt);

        return netEnt.networkable.ID;        
    }

    public static uint Register(NetworkEntity netEnt, uint id)
    {    
        netEnt.networkable = new Networkable(id);       
        entities.Add(netEnt.networkable.ID, netEnt);

        return netEnt.networkable.ID;
    }

    public static void Unregister(NetworkEntity netEnt)
    {
        entities.Remove(netEnt.networkable.ID);
    }


    public static NetworkEntity Find(uint id)
    {
        NetworkEntity ent;

        if (entities.TryGetValue(id, out ent))        
            return ent;        
        else        
            return null;        
    }
}
