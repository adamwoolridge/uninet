using UnityEngine;
using System.Collections;

public class TestSpawn : MonoBehaviour {

    bool spawned = false;

    void Start()
    {
        spawned = false;
    }

    void Update()
    {
        if (!spawned && NetworkManager.IsServer && NetworkManager.connected)
        {
            NetworkEntity netEnt = GetComponent<NetworkEntity>();

            // Server side would call this        
            EntityManager.Register(netEnt);

            spawned = true;
        }
    }		
}
