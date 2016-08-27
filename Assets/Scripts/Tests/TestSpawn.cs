using UnityEngine;
using System.Collections;

public class TestSpawn : MonoBehaviour {

	// Use this for initialization
	void Start () {        
        NetworkEntity netEnt = GetComponent<NetworkEntity>();

        // Server side would call this        
        EntityManager.Register(netEnt);
	}		
}
