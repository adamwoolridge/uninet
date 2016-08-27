using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{  
    public void OnDestroy()
    {
        if (NetworkManager.IsServer)
        {
            Destroy();
        }
    }   
}
