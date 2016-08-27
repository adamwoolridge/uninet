using UnityEngine;
using System.Collections;

public static class ClientConnection
{
    public static void ReceivedMessage(NetworkMessage message)
    {
        Debug.Log("Client received message of type: " + message.MessageType);

        switch (message.MessageType)
        {
            case NetworkMessageType.Entity_UpdateTransform:
                {
                    OnEntityUpdate(message);
                    return;
                }
        }
    }

    private static void OnEntityUpdate(NetworkMessage message)
    {
        uint id = message.ReadUInt();
        NetworkEntity ent = EntityManager.Find(id);

        if (ent==null)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ent = box.AddComponent<NetworkEntity>();            
            EntityManager.Register(ent, id);
        }
        ent.transform.position = message.ReadVector3();        
        ent.transform.rotation = message.ReadQuaternion();
    }
}

	