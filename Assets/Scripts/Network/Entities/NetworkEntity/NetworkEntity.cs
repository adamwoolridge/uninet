using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public string Path = "";
    public int TicksPerSecond = 10;
    
    public Networkable networkable;

    public bool locallyControlled = false;

    Vector3 realPos;
    Quaternion realRot;

    public void Init()
    {        
        if (NetworkManager.IsClient && locallyControlled || NetworkManager.IsServer)
        {
            float tick = 1.0f / (float)TicksPerSecond;
            InvokeRepeating("SendUpdate", 0f, tick);
        }
    }    
 
    private void SendUpdate()
    {
        if (transform.hasChanged)
        {
            if (NetworkManager.IsClient)
            {
                Debug.Log("sending update to server");
                NetworkMessage updateMsg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
                updateMsg.Write(networkable.ID);
                updateMsg.Write(Path);
                updateMsg.Write(transform.position);
                updateMsg.Write(transform.rotation);
                NetworkManager.Instance.SendToServer(updateMsg);
            }
            if (NetworkManager.IsServer)
            {

            }
            transform.hasChanged = false;
        }
    }

    public void Update()
    {
        if (NetworkManager.IsClient)
        {
            if (locallyControlled)
            {
                transform.position += new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * 2f * Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, realPos, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRot, Time.deltaTime * 5f);
            }
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
