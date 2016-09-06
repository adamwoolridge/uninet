using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Networking.Raknet;

unsafe public class NetworkManager : MonoBehaviour
{     
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
    }
    
    public void Host()
    {
        if (connected) return;

        ptr = Native.NET_Create();

        if (Native.NET_StartServer(ptr, "", 8888, 128) == 0)
        {
            IsServer = true;
            IsClient = false;
            connected = true;
            DebugLog("Hosting!");
        }
        else
        {
            connected = false;
            IsServer = false;
            IsClient = false;
            DebugLog("Failed to start server!");
            ShutDown();
        }

        // Move this to server init when I star that shit for realz
        new Grid(10, 10, 10f);
    }

    public void Connect()
    {
        if (connected) return;

        ptr = Native.NET_Create();

       if (Native.NET_StartClient(ptr, "127.0.01", 8888, 10, 500, 0) == 0)
       //if (Native.NET_StartClient(ptr, "86.179.63.200", 8888, 10, 500, 0) == 0)
//		if (Native.NET_StartClient(ptr, "192.168.1.104", 8888, 10, 500, 0) == 0)
        {
            IsServer = false;
            IsClient = true;
            connected = true;
            DebugLog("connected?");
            return;
        }
        else
        {
            IsServer = false;
            IsClient = false;
            connected = false;
            DebugLog("Failed to connect");
            ShutDown();
        }     
    }

    private void ClientConnected(ulong id)
    {
        if (IsServer)
        {
            NetworkClientID clientID = ServerConnection.PlayerConnected(id);
            clients.Add(clientID.ConnectionID, clientID);            
        }             
    }

    private void ClientDisconnected(ulong id)
    {
        if (IsServer)
        {            
            DebugLog("Client disconnected, removing ID: " + id);

            NetworkEntity ent = EntityManager.Find(clients[id].NetID);
            if (ent!=null)
                ent.Destroy();

            clients.Remove(id);                        
        }    
        
        if (IsClient)
        {
            DebugLog("Disconnected from server!");
            ShutDown();
        }
    }

    bool RaknetPacket(byte type)
    {
        if (type == 180) return false;

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
                    clientConnectionID = guid;
                    DebugLog("Connection request accepted, server con id: " + clientConnectionID);
                    return true;
                }

            case PacketType.DISCONNECTION_NOTIFICATION:
                {
                    ClientDisconnected(Native.NETRCV_GUID(ptr));
                    return true;
                }

            case PacketType.CONNECTION_ATTEMPT_FAILED:
                {
                    DebugLog("Failed to connect!");
                    ShutDown();
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
                              
                if (RaknetPacket(type))
                {
                    
                }
                else
                {                 
                    int length = System.BitConverter.ToInt32(ReadBytes(4), 0);
                    
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

    public void SendEntity(NetworkClientID clientID, NetworkEntity entity)
    {        
        NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
        msg.Write(1);
        msg.Write(entity.networkable.ID);
        msg.Write(entity.gridCell.Index);
        msg.Write(entity.Path);
        msg.Write(entity.transform.position);
        msg.Write(entity.transform.rotation);
        Send(clientID.ConnectionID, msg);
    }

    public void SendEntities(NetworkClientID clientID, HashSet<NetworkEntity> entities)
    {
        if (entities == null || entities.Count == 0) return;
       	if (clientID == null) return;
       	              
        foreach (NetworkEntity ent in entities)
        {
            SendEntity(clientID, ent);
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
            TestMassSpawn.Spawn("PalmTree01", 500);
            //SendEntities();
        }
    }


    public void SendToServer(NetworkMessage netMsg)
    {        
        Send(clientConnectionID, netMsg);
    }

    public void SendToClients(NetworkMessage netMsg)
    {
        foreach (KeyValuePair<ulong, NetworkClientID> client in clients)
        {
            Send(client.Key, netMsg);
        }
    }
    
    public void SendToCell(GridCell cell, NetworkMessage message, NetworkClientID excludeClient=null)
    {
        foreach (NetworkEntity ne in cell.Listeners)
        {
            if (excludeClient == null || excludeClient != ne.clientID)
                Send(ne.clientID.ConnectionID, message);
        }
    }

    public void Send(ulong targetId, NetworkMessage message)
    {
        if (!connected) return;

        byte[] bytes = message.GetData();

        Native.NETSND_Start(ptr);
        WriteBytes(new byte[] { 180 });
        WriteBytes(System.BitConverter.GetBytes(bytes.Length));
        WriteBytes(bytes);

        Native.NETSND_Send(ptr, targetId, 2, 2, 0);
    }

    private void DebugLog(string text)
    {
        TextLog.text += (text + "\n");
    }
}