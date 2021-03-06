﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public string Path = "";
    public int TicksPerSecond = 10;
    
    public Networkable networkable;
    public NetworkClientID clientID;
    public bool locallyControlled = false;

    Vector3 realPos;
    Quaternion realRot;

    public void Init()
    {        
        if (NetworkManager.IsClient && locallyControlled)
        {
            float tick = 1.0f / (float)TicksPerSecond;
            InvokeRepeating("SendUpdate", 0f, tick);
        }
    }    
 
    private void SendUpdate()
    {
        if (NetworkManager.IsClient && locallyControlled)
        {
            if (transform.hasChanged)
            {
                if (NetworkManager.IsClient)
                {
                    NetworkMessage updateMsg = new NetworkMessage(NetworkMessageType.Entity_UpdateTransform);
                    updateMsg.Write(networkable.ID);
                    updateMsg.Write(Path);
                    updateMsg.Write(transform.position);
                    updateMsg.Write(transform.rotation);
                    NetworkManager.Instance.SendToServer(updateMsg);
                    transform.hasChanged = false;
                }
            }
        }        
    }

    public void Update()
    {
        if (NetworkManager.IsClient)
        {
            if (locallyControlled)
            {
                transform.position += new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * 5f * Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, realPos, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRot, Time.deltaTime * 5f);
            }
        }

        if (NetworkManager.IsServer)
        {
            if (transform.hasChanged)
            {
                UpdateGrid();
                transform.hasChanged = false;
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

            if (gridCell != null)
                gridCell.OnEntityExit(this);
        }
               
        EntityManager.Unregister(this);

        if (gameObject!=null)
            Destroy(gameObject);
    }

    public void OnReceiveEntityUpdate(Vector3 pos, Quaternion rot, bool lerp = true)
    {
        realPos = pos;
        realRot = rot;

        if (NetworkManager.IsServer || !lerp)
        {
            transform.position = realPos;
            transform.rotation = realRot;
        }
    }
}
