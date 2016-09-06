using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public static void DestroyCell(uint id)
    {
        if (NetworkManager.IsClient)
        {
            List<NetworkEntity> toDelete = entities.Values.ToList();

            foreach (NetworkEntity ne in toDelete)
            {
                if (ne.locallyControlled) continue; // Don't delete the local player 
                if (ne.cellID == id)
                    ne.Destroy();
            }
        }
    }
}