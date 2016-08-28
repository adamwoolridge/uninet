using UnityEngine;
using System.Collections;

public static class ServerConnection
{
    public static void ReceivedMessage(NetworkMessage message)
    {
        Debug.Log("Server received message of type: " + message.MessageType);
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
}

	