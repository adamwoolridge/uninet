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

    public void Init()
    {        
        if (NetworkManager.IsClient)
        {
            float tick = 1.0f / (float)TicksPerSecond;
            InvokeRepeating("SendUpdate", 0f, tick);
        }
    }    
 
    private void SendUpdate()
    {
        if (transform.hasChanged)
        {
            Debug.Log("Sending position update to server.");
            NetworkMessage updateMsg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
            updateMsg.Write(networkable.ID);
            updateMsg.Write(transform.position);
            updateMsg.Write(transform.rotation);

            NetworkManager.Instance.SendToServer(updateMsg);
            transform.hasChanged = false;
        }
    }

    public void Update()
    {
        if (locallyControlled)
        {

        }
        else
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
