using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using Network;
using Networking;
using Networking.Raknet;

unsafe public class NetworkManager : MonoBehaviour
{
   // public static Server sv = new Facepunch.Network.Raknet.Server();
  //  public Client cl = new Facepunch.Network.Raknet.Client();

    int channelID;
    int socketID;
    int socketPort = 8888;
    ulong clientConnectionID;

    private Dictionary<ulong, NetworkClientID> clients = new Dictionary<ulong, NetworkClientID>();

    // Have both just to make the code nicer to read
    public static bool IsServer = false;
    public static bool IsClient = true;

    public Text TextLog;

    public static NetworkManager Instance;

    public static bool connected = false;

    int receives = 0;

    private System.IntPtr ptr;

    private void ShutDown()
    {
        if (ptr != System.IntPtr.Zero)
        {
            Native.NET_Close(ptr);
            ptr = System.IntPtr.Zero;
        }
    }

    void OnDestroy()
    {
        ShutDown();
    }

    // Use this for initialization
    void Awake()
    {
     

        Instance = this;
        //NetworkTransport.Init();                

        //sv.ip = "127.0.0.1";
        //sv.port = 8888;
        //sv.Start();


    }

    //void OnNetworkMessage(Message packet)
    //{
    //    Debug.Log("network message!");
    //}

    //void OnDisconnected(string strReason, Network.Connection connection)
    //{
    //    Debug.Log("Client disconnected");
    //}

    public void Host()
    {
        ptr = Native.NET_Create();

        if (Native.NET_StartServer(ptr, "", 8888, 128) == 0)
        {
            IsServer = true;
            IsClient = false;
            connected = true;
            Debug.Log("Hosting!");
        }
        else
        {
            connected = false;
            IsServer = false;
            IsClient = false;
            Debug.Log("Cannot start server");
            ShutDown();
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
 
    public void Connect()
    {
        ptr = Native.NET_Create();

        if (Native.NET_StartClient(ptr, "127.0.01", 8888, 10, 500, 0) == 0)
        {
            IsServer = false;
            IsClient = true;
            connected = true;
            // connectionState = ClientConnectState.Connecting;
            // connection = new NetworkConnection();
            //connection.ipaddress = ip;

            //Create new Raknet Connection
            // Debug.Log(ptr + " Client connecting to " + ip + " " + port);
            Debug.Log("Connected?");
            return;
        }
        else
        {
            IsServer = false;
            IsClient = false;
            connected = false;
            Debug.Log("Failed to connect");
            ShutDown();
        }
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

    private void ClientConnected(ulong id)
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

    private void ClientDisconnected(ulong id)
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

    bool RaknetPacket(byte type)
    {
        ulong guid = Native.NETRCV_GUID(ptr);

        switch (type)
        {
            case PacketType.NEW_INCOMING_CONNECTION:
                {
                    ClientConnected(Native.NETRCV_GUID(ptr));
                    Debug.Log("New client connection.");
                    return true;
                }

            case PacketType.CONNECTION_REQUEST_ACCEPTED:
                {
                    Debug.Log("Connection request accepted");

                    clientConnectionID = guid;                    
                    return true;
                }

        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (connected)
        {
            while (Native.NET_Receive(ptr))
            {
                byte type = ReadBytes(1)[0];
                Debug.Log(type);
                
                if (RaknetPacket(type))
                {
                    
                }
                else
                {
                    Debug.Log("Non raknet packet received!");
                    int length = System.BitConverter.ToInt32(ReadBytes(4), 0);
                    Debug.Log("Length: " + length);
                    NetworkMessage message = new NetworkMessage(ReadBytes(length));

                    if (IsServer)
                    {
                        ServerConnection.ReceivedMessage(message);
                    }
                    if (IsClient)
                    {
                        ClientConnection.ReceivedMessage(message);
                        receives++;
                    }
                }
            }
        }
        //int recHostId;
        //int recConnectionId;
        //int recChannelId;
        //byte[] recBuffer = new byte[1024];
        //int bufferSize = 1024;
        //int dataSize;
        //byte error;
        //NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

        //switch (recNetworkEvent)
        //{
        //    case NetworkEventType.Nothing:
        //        break;
        //    case NetworkEventType.ConnectEvent:
        //        ClientConnected(recConnectionId);
        //        break;
        //    case NetworkEventType.DataEvent:
        //        NetworkMessage message = new NetworkMessage(recBuffer);
        //        if (IsServer)
        //        {                 
        //            ServerConnection.ReceivedMessage(message);
        //        }
        //        if (IsClient)
        //        {
        //            ClientConnection.ReceivedMessage(message);
        //            receives++;
        //        }
        //        break;
        //    case NetworkEventType.DisconnectEvent:
        //        Debug.Log(error);
        //        ClientDisconnected(recConnectionId);
        //        break;
       // }
    }

    byte[] ReadBytes(int length)
    {
        var data = new byte[length];

        fixed (byte* dataPtr = data)
        {
            if (!Native.NETRCV_ReadBytes(ptr, dataPtr, length))
            {
                Debug.LogError("NETRCV_ReadBytes returned false");
                return null;
            }
        }

        return data;
    }

    void WriteBytes(byte[] bytes)
    {
        fixed (byte* valPtr = bytes)
        {
            Native.NETSND_WriteBytes(ptr, valPtr, bytes.Length);
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


        if (GUI.Button(new Rect(10, 300, 100, 100), "ToServer"))
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_LocalPlayerCreated);
            SendToServer(msg);
        }

        //if (GUI.Button(new Rect(10, 300, 100, 100), "Spawn test"))
        //{
        //    TestMassSpawn.Spawn("Cube", 50);
        //    SendEntities();
        //}
    }


    public void SendToServer(NetworkMessage netMsg)
    {
        Debug.LogWarning("SendToServer is commented out!");
        Send(clientConnectionID, netMsg);
    }

    public void SendToClients(NetworkMessage netMsg)
    {
        foreach (KeyValuePair<ulong, NetworkClientID> client in clients)
        {
            Send(client.Key, netMsg);
        }
    }
       
    public void Send(ulong targetId, NetworkMessage message)
    {
        if (!connected) return;

        byte[] bytes = message.GetData();

        Native.NETSND_Start(ptr);        
        WriteBytes(System.BitConverter.GetBytes(bytes.Length));
        WriteBytes(bytes);

        Native.NETSND_Send(ptr, targetId, 0, 2, 0);
    }

    private void DebugLog(string text)
    {
        TextLog.text += (text + "\n");
    }
}