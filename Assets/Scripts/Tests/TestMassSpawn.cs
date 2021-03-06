﻿using UnityEngine;
using System.Collections;

public static class TestMassSpawn
{ 
    public static void Spawn(string prefabpath, int count)
    {
        for (int i =0; i< count; i++)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load(prefabpath)) as GameObject;
            NetworkEntity ent = obj.GetComponent<NetworkEntity>();
            ent.locallyControlled = true;        
            EntityManager.Register(ent);
            obj.transform.position = new Vector3(Random.Range(0, 100f), 0f, Random.Range(0f, 100f));
            obj.transform.eulerAngles = new Vector3(0f, Random.Range(0, 359f), 0f);
        }
    }
}
