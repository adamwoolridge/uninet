using UnityEngine;
using System.Collections;

public static class ClientConnection
{    
    public static void ReceivedMessage(NetworkMessage message)
    {
        Debug.Log(message.MessageType);

        switch (message.MessageType)
        {
            case NetworkMessageType.Entity_LocalPlayerCreated:
                {
                    CreateEntity(message.ReadUInt(), message.ReadString(), true);                    
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
            case NetworkMessageType.Chat:
                {
                    OnChat(message);
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
        string path = message.ReadString();
        Vector3 pos = message.ReadVector3();
        Quaternion rot = message.ReadQuaternion();
        
        NetworkEntity ent = EntityManager.Find(id);

        if (ent==null)        
            ent = CreateEntity(id, path, false);        
        
        if (ent.locallyControlled)                
            return;
        
        ent.OnReceiveEntityUpdate(pos, rot);

        message.reader.Close();
        message.stream.Close();
    }
    
    private static NetworkEntity CreateEntity(uint netID, string path, bool local)
    {        
        GameObject obj = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;
        NetworkEntity ent = obj.GetComponent<NetworkEntity>();
        ent.locallyControlled = local;
        EntityManager.Register(ent, netID);
        ent.Init();
        return ent;
    }

    private static void OnChat(NetworkMessage message)
    {
        //uint userID = message.ReadUInt();
        string chatText = message.ReadString();

        Debug.Log(chatText);
    }
}

	