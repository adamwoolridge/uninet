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
        }
    }

    public static NetworkClientID PlayerConnected(int connectionID)
    {
        Debug.Log("Client connected with ID: " + connectionID);
                
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.position = Vector3.zero;

        NetworkEntity ent = box.AddComponent<NetworkEntity>();
        uint netID = EntityManager.Register(ent);
        NetworkClientID netClientID = new NetworkClientID(connectionID, netID);

        NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_ClientCreated);
        msg.Write(netClientID.NetID);
        NetworkManager.Instance.Send(netClientID.ConnectionID, msg);

        return netClientID;
    }

	private static void OnEntityUpdate(NetworkMessage message)
    {
        uint id = message.ReadUInt();
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
        msg.Write(pos);
        msg.Write(rot);
        NetworkManager.Instance.SendToClients(msg);
    }
}

	