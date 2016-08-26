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

    }
}

	