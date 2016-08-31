using UnityEngine;
using System.Collections;

public class KeyBinding : ListComponent<KeyBinding>
{
    public static bool IsOpen
    {
        get
        {
            for ( int i=0; i< InstanceList.Count; i++  )
            {
                if ( InstanceList[i].isActiveAndEnabled ) return true;
            }

            return false;
        }
    }
}
