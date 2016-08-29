using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public void OnReceiveEntityUpdate(Vector3 pos, Quaternion rot)
    {
        realPos = pos;
        realRot = rot;

        if (NetworkManager.IsServer)
        {
        	transform.position = realPos;
        	transform.rotation = realRot;
        }
    }
}
