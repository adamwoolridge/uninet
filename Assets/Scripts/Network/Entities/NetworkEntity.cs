using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public int TicksPerSecond = 10;

    public Networkable networkable;
  
    public void Init(Networkable net)
    {
        networkable = net;

        Debug.Log("New entity, id: " + net.ID);

        if (NetworkManager.IsClient)
        {
            float tick = (float)   TicksPerSecond / 1.0f;
            InvokeRepeating("SendUpdate", 0f, tick);
        }
    }    
 
    public void Destroy()
    {
        if (NetworkManager.IsServer)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_Destroy);
            msg.Write(networkable.ID);
            NetworkManager.Instance.SendToClients(msg);
        }

        EntityManager.Unregister(this);

        if (gameObject!=null)
            Destroy(gameObject);
    }

    public NetworkMessage GetUpdateMessage()
    {
        NetworkMessage msg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
        msg.Write(networkable.ID);
        msg.Write(transform.position);
        msg.Write(transform.rotation);
        return msg;
    }

    public void OnReceiveEntityUpdate(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}
