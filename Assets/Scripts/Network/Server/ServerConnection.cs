using UnityEngine;
using System.Collections;

public static class ServerConnection
{
    public static void ReceivedMessage(NetworkMessage message)
    {
		switch (message.MessageType)
        {
            case NetworkMessageType.Entity_UpdateTransform:
                {
                    OnEntityUpdate(message);
                    return;
                }

            case NetworkMessageType.Chat:
                {
                    OnChat(message);
                    return;
                }
        }
    }

    public static NetworkClientID PlayerConnected(ulong connectionID)
    {        
        GameObject obj = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;        
        NetworkEntity ent = obj.GetComponent<NetworkEntity>();
        ent.locallyControlled = false;
        uint netID = EntityManager.Register(ent);
        NetworkClientID netClientID = new NetworkClientID(connectionID, netID);
        ent.clientID = netClientID;
        obj.transform.position = Vector3.zero;        
        NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_LocalPlayerCreated);
        msg.Write(netClientID.NetID);
        msg.Write(Grid.Instance.GetCell(obj.transform.position).Index);
        msg.Write(ent.Path);
        NetworkManager.Instance.Send(netClientID.ConnectionID, msg);

        return netClientID;
    }

	private static void OnEntityUpdate(NetworkMessage message)
    {
        uint id = message.ReadUInt();        
        string path = message.ReadString();
        Vector3 pos = message.ReadVector3();
        Quaternion rot = message.ReadQuaternion();

        NetworkEntity ent = EntityManager.Find(id);

        if (ent==null)
        {
        	Debug.LogWarning("Server received OnEntityUpdate for an invalid entity, ID: " + id);
            return;
        }
       
        ent.OnReceiveEntityUpdate(pos, rot);

        NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
        msg.Write(1);
        msg.Write(id);
        msg.Write(ent.gridCell.Index);
        msg.Write(path);
        msg.Write(pos);
        msg.Write(rot);

        NetworkManager.Instance.SendToCell(ent.gridCell, msg, ent.clientID);
    }

    private static void OnChat(NetworkMessage message)
    {
        NetworkManager.Instance.SendToClients(message);
    }
}

	