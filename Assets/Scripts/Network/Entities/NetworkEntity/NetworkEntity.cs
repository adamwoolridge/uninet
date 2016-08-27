using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public int TicksPerSecond = 10;
    
    public Networkable networkable;

    public bool locallyControlled = false;

    Vector3 realPos;
    Quaternion realRot;

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
 
    public void Update()
    {
        if (!locallyControlled)
        {
            transform.position = Vector3.Lerp(transform.position, realPos, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRot, Time.deltaTime * 5f);
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
}
