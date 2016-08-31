using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Network;

public class NetworkManager : MonoBehaviour
{
    public static Server sv = new Facepunch.Network.Raknet.Server();
    public Client cl = new Facepunch.Network.Raknet.Client();

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

    public static bool connected = false;

    int receives = 0;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
     //   NetworkTransport.Init();        
        
    }

    void OnNetworkMessage(Message packet)
    {
        Debug.Log("network message!");
    }

    void OnDisconnected(string strReason, Network.Connection connection)
    {
        Debug.Log("Client disconnected");
    }

    public void Host()
    {
        sv.ip = "localhost";
        sv.port = 8888;         
        sv.onMessage = OnNetworkMessage;
        //sv.onUnconnectedMessage = OnUnconnectedMessage;
        sv.onDisconnected = OnDisconnected;

        if (!sv.Start())
        {
            Debug.LogWarning("Couldn't Start Server.");
            return;
        }

        //ConnectionConfig config = new ConnectionConfig();
        //channelID = config.AddChannel(QosType.Reliable);
        //config.MaxSentMessageQueueSize = 1000;    

        //HostTopology topology = new HostTopology(config, 128);
        //socketID = NetworkTransport.AddHost(topology, socketPort);
        //connected = true;
        //IsServer = true;
        //IsClient = false;
        //Debug.Log("Server up!");

        //InvokeRepeating("SendEntities", 0.1f, 0.1f);
    }

    void OnClNetworkMessage(Message packet)
    {
        Debug.Log("Message!");
    }

    public void Connect()
    {

        if (!cl.Connect("127.0.0.1", 8888))
        {
            Debug.Log("Connect failed!");
            return;
        }
        else
        {
            Debug.Log("Connected?");
        }

       cl.onMessage = OnClNetworkMessage;

        //ConnectionConfig config = new ConnectionConfig();
        //channelID = config.AddChannel(QosType.Reliable);
        //config.MaxSentMessageQueueSize = 1000;
        //HostTopology topology = new HostTopology(config, 1);
        //socketID = NetworkTransport.AddHost(topology);
        //connected = true;
        //IsServer = false;
        //IsClient = true;
        //byte error;
        ////clientConnectionID = NetworkTransport.Connect(socketID, "86.179.63.182", socketPort, 0, out error);                
        //clientConnectionID = NetworkTransport.Connect(socketID, "127.0.0.1", socketPort, 0, out error);
    }

    private void ClientConnected(int id)
    {
        if (IsServer)
        {
            NetworkClientID clientID = ServerConnection.PlayerConnected(id);
            clients.Add(clientID.ConnectionID, clientID);            
        }        
        
        if (IsClient)
        {
            DebugLog("Connected to server.");
        }        
    }

    private void ClientDisconnected(int id)
    {
        if (IsServer)
        {            
            Debug.Log("Client disconnected, removing ID: " + id);

            NetworkEntity ent = EntityManager.Find(clients[id].NetID);
            if (ent!=null)
                ent.Destroy();

            clients.Remove(id);                        
        }        
    }

    // Update is called once per frame
    void Update()
    {
        return;
          
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

                if (IsServer)
                {
                    NetworkMessage message = new NetworkMessage(recBuffer);
                    ServerConnection.ReceivedMessage(message);
                }
                if (IsClient)
                {
                    //ClientConnection.ReceivedMessage(message);
                    receives++;
                }
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(error);
                ClientDisconnected(recConnectionId);
                break;
        }
    }

    void SendEntities()
    {
        if (EntityManager.entities.Count == 0) return;
        
        int remaining = EntityManager.entities.Count;
              
        //NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
        //msg.Write((uint)Mathf.Min(20,remaining));

        //int batchItemCount = 0;

        foreach (KeyValuePair<uint, NetworkEntity> ent in EntityManager.entities)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
            msg.Write(1);
            msg.Write(ent.Value.networkable.ID);
            msg.Write(ent.Value.Path);
            msg.Write(ent.Value.transform.position);
            msg.Write(ent.Value.transform.rotation);
            SendToClients(msg);

            //remaining--;
            //batchItemCount++;
            //if (batchItemCount == 20 || remaining==0)
            //{                
            //    batchItemCount = 0;
               
            //    if (remaining > 0)
            //    {
            //        msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
            //        msg.Write((uint)Mathf.Min(20, remaining));
            //    }
            //}
        }                   
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(500, 10, 100, 100), receives.ToString());

        if (GUI.Button(new Rect(10, 10, 100, 100), "Host"))
            Host();

        if (GUI.Button(new Rect(10, 200, 100, 100), "Client"))
            Connect();

        if (GUI.Button(new Rect(10, 300, 100, 100), "Spawn test"))
        {
            TestMassSpawn.Spawn("Cube", 100);
            SendEntities();
        }
    }


    public void SendToServer(NetworkMessage netMsg)
    {
        Send(clientConnectionID, netMsg);
    }

    public void SendToClients(NetworkMessage netMsg)
    {
        foreach (KeyValuePair<int, NetworkClientID> client in clients)
        {
            Send(client.Key, netMsg);
        }
    }
       
    public void Send(int targetId, NetworkMessage message)
    {
        if (!connected) return;
        byte error;
        NetworkTransport.Send(socketID, targetId, channelID, message.GetData(), message.GetData().Length, out error);
        
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("Message send error: " + (NetworkError)error);
        }
    }

    private void DebugLog(string text)
    {
        TextLog.text += (text + "\n");
    }
}