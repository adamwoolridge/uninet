using UnityEngine;
using System.Collections;

public class Networkable
{
    public uint ID;
    
    public Networkable(uint id)
    {
        ID = id;
    }
       
    public virtual void Send()
    {

    }   
}
