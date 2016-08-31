using UnityEngine;
using System.Collections;

public class NetworkClientID
{
    public ulong ConnectionID;
    public uint NetID;

    public NetworkClientID(ulong conID, uint netID)
    {
        ConnectionID = conID;
        NetID = netID;
    }

}
