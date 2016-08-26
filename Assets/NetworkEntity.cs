using UnityEngine;
using System.Collections;

public class NetworkEntity 
{
    public uint ID;
    public Vector3 Position;
    public Quaternion Rotation;
    
    public NetworkEntity(uint id)
    {
        ID = id;
    }
       
    public virtual void Send()
    {

    }                    
}
