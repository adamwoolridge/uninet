using UnityEngine;
using System.Collections;

public static class ClientConnection
{    
    public static void ReceivedMessage(NetworkMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageType.Entity_LocalPlayerCreated:
                {
                    NetworkEntity clientEnt = CreateEntity(message.ReadUInt(), message.ReadUInt(), message.ReadString(), true);
                    clientEnt.transform.FindChild("Camera").gameObject.SetActive(true);
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
            case NetworkMessageType.Cell_Destroy:
                {
                    OnCellDestryoyed(message);
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

    private static void OnCellDestryoyed(NetworkMessage message)
    {
        EntityManager.DestroyCell(message.ReadUInt());
    }

    private static void OnEntityUpdate(NetworkMessage message)
    {
        uint id = message.ReadUInt();
        uint cellID = message.ReadUInt();
        string path = message.ReadString();
        Vector3 pos = message.ReadVector3();
        Quaternion rot = message.ReadQuaternion();
        
        NetworkEntity ent = EntityManager.Find(id);

        bool justCreated = false;

        if (ent==null)   
        {     
            ent = CreateEntity(id, cellID, path, false);  
            justCreated = true;      
        }

        ent.cellID = cellID;

        if (ent.locallyControlled)                
            return;
        
        ent.OnReceiveEntityUpdate(pos, rot, !justCreated);

        message.reader.Close();
        message.stream.Close();
    }
    
    private static NetworkEntity CreateEntity(uint netID, uint cellID, string path, bool local)
    {        
        GameObject obj = GameObject.Instantiate(Resources.Load(path)) as GameObject;
        NetworkEntity ent = obj.GetComponent<NetworkEntity>();
        ent.cellID = cellID;
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

	