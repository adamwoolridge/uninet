using UnityEngine;
using System.Collections;

public class NetworkClientID
{
    public int ConnectionID;
    public uint NetID;

    public NetworkClientID(int conID, uint netID)
    {
        ConnectionID = conID;
        NetID = netID;
    }

}
