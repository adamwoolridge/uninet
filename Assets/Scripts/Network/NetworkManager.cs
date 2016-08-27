﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class NetworkManager : MonoBehaviour
{
    int channelID;
    int socketID;
    int socketPort = 8888;
    int clientConnectionID;

    private Dictionary<int, NetworkClientID> clients = new Dictionary<int, NetworkClientID>();

    // Have both just to make the code nicer to read
    public static bool IsServer = false;
    public static bool IsClient = true;

    public Text TextLog;

    public static NetworkManager Instance;

    private bool connected = false;
    
    // Use this for initialization
    void Awake()
    {
        Instance = this;
        NetworkTransport.Init();        
    }
    
    public void Host()
    {
        ConnectionConfig config = new ConnectionConfig();
        channelID = config.AddChannel(QosType.Reliable);    
        HostTopology topology = new HostTopology(config, 128);
        socketID = NetworkTransport.AddHost(topology, socketPort);
        connected = true;
        IsServer = true;
        IsClient = false;
        Debug.Log("Server up!");

        InvokeRepeating("SendEntities", 0.1f, 0.1f);
    }

    public void Connect()
    {
        ConnectionConfig config = new ConnectionConfig();
        channelID = config.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(config, 1);
        socketID = NetworkTransport.AddHost(topology);
        connected = true;
        IsServer = false;
        IsClient = true;
        byte error;
        clientConnectionID = NetworkTransport.Connect(socketID, "127.0.0.1", socketPort, 0, out error);        

        Debug.Log("Connected to server!");
    }

    private void ClientConnected(int id)
    {
        if (IsServer)
        {
            clients.Add(id, new NetworkClientID(id));
        }        
        
        Debug.Log("Client connected with ID: " + id);
    }

    private void ClientDisconnected(int id)
    {
        if (IsServer)
        {
            clients.Remove(id);
        }

        Debug.Log("Client disconnected, removing ID: " + id);
    }

    // Update is called once per frame
    void Update()
    {        
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                ClientConnected(recConnectionId);
                break;
            case NetworkEventType.DataEvent:
                NetworkMessage message = new NetworkMessage(recBuffer);                
                if (IsServer)                
                    ServerConnection.ReceivedMessage(message);                
                if (IsClient)                
                    ClientConnection.ReceivedMessage(message);                                                                                                
                break;
            case NetworkEventType.DisconnectEvent:
                ClientDisconnected(recConnectionId);
                break;
        }
    }

    void SendEntities()
    {
        foreach (KeyValuePair<uint, NetworkEntity> ent in EntityManager.entities)
        {
            SendToClients(ent.Value.GetUpdateMessage());
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "Host"))
            Host();

        if (GUI.Button(new Rect(10, 200, 100, 100), "Client"))
            Connect();

        if (GUI.Button(new Rect(10, 400, 100, 100), "Send to Server"))
        {
            NetworkMessage message = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
            message.Write(new Vector3(1f, 2f, 3f));
            message.Write(new Quaternion(4f, 5f, 6f, 7f));
            Send(clientConnectionID, message);
        }

        if (GUI.Button(new Rect(10, 600, 100, 100), "Send to clients"))
            SendClients();
    }


    public void SendToClients(NetworkMessage netMsg)
    {
        foreach (KeyValuePair<int, NetworkClientID> client in clients)
        {
            Send(client.Key, netMsg);
        }
    }

    public void SendClients()
    {
        NetworkMessage message = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
        message.Write(new Vector3(1f, 2f, 3f));
        message.Write(new Quaternion(4f, 5f, 6f, 7f));

        SendToClients(message);
    }
    
    public void Send(int targetId, NetworkMessage message)
    {
        if (!connected) return;

        byte error;
              
        Debug.Log(NetworkTransport.Send(socketID, targetId, channelID, message.GetData(), message.GetData().Length, out error));
        Debug.Log(error);
    }

    private void DebugLog(string text)
    {
        TextLog.text += (text + "\n");
    }
}