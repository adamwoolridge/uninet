using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkGameObject : MonoBehaviour
{

    public int TicksPerSecond = 10;

    protected NetworkEntity networkEntity;

    // Use this for initialization
    void Start()
    {
        if (NetworkManager.IsServer)
        {
            Init(EntityManager.GetNew());
        }        
    }
    
    public void Init(NetworkEntity entity)
    {
        networkEntity = entity;

        Debug.Log("New entity, id: " + entity.ID);

        if (NetworkManager.IsClient)
        {
            float tick = (float)   TicksPerSecond / 1.0f;
            InvokeRepeating("SendUpdate", 0f, tick);
        }
    }

    void SendUpdate()
    {

    }
}
