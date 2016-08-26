using UnityEngine;
using System.Collections;

public static class ServerConnection
{
    public static void ReceivedMessage(NetworkMessage message)
    {
        Debug.Log("Server received message of type: " + message.MessageType);
    }
}

	