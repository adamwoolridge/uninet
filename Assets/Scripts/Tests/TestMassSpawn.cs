using UnityEngine;
using System.Collections;

public static class TestMassSpawn
{ 
    public static void Spawn(string prefabpath, int count)
    {
        for (int i =0; i< count; i++)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load(prefabpath)) as GameObject;
            NetworkEntity ent = obj.GetComponent<NetworkEntity>();            
            EntityManager.Register(ent);
            obj.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(5f, 5f));

        }
    }
}
