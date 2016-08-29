using UnityEngine;
using System.Collections;

public static class ClientConnection
{    
    public static void ReceivedMessage(NetworkMessage message)
    {        
        switch (message.MessageType)
        {
            case NetworkMessageType.Entity_ClientCreated:
                {
                    NetworkEntity clientEnt = CreateEntity(message.ReadUInt());
                    clientEnt.locallyControlled = true;
                    return;
                }
            case NetworkMessageType.Entity_UpdateTransform:
                {
                    uint count = message.ReadUInt();
                    for (int i=0; i< count; i++)
                    {
                        OnEntityUpdate(message);
                    }                    
                    return;
                }
            case NetworkMessageType.Entity_Destroy:
                {
                    OnEntityDestroyed(message);                    
                    return;                  
                }                
        }
    }

    private static void OnEntityDestroyed(NetworkMessage message)
    {
        uint id = message.ReadUInt();
        NetworkEntity ent = EntityManager.Find(id);
        ent.Destroy();
    }

    private static void OnEntityUpdate(NetworkMessage message)
    {
        uint id = message.ReadUInt();
        
        NetworkEntity ent = EntityManager.Find(id);

        if (ent==null)
        {
            CreateEntity(id);
        }

        if (ent.locallyControlled) return;

        ent.OnReceiveEntityUpdate(message.ReadVector3(), message.ReadQuaternion());        
    }

    private static NetworkEntity CreateEntity(uint netID)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        NetworkEntity ent = box.AddComponent<NetworkEntity>();
        EntityManager.Register(ent, netID);
        ent.Init();
        return ent;
    }
}

	