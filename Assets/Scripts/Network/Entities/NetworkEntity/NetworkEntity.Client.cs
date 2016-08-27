using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public void OnReceiveEntityUpdate(Vector3 pos, Quaternion rot)
    {
        realPos = pos;
        realRot = rot;
    }
}
