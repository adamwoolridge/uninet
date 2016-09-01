﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridDisplay : MonoBehaviour {    
	// Use this for initialization
	void Start () {        
        
	}

    void OnDrawGizmos()
    {
        if (Grid.Instance == null) return;        

        foreach(GridCell cell in Grid.Instance.Cells)
        {
            Gizmos.DrawWireCube(cell.BoundingBox.center, cell.BoundingBox.size);

#if UNITY_EDITOR
            int count = cell.Entities.Count;
            if (count > 0)
                Handles.color = Color.green;
            else
                Handles.color = Color.grey;
            Handles.Label(cell.BoundingBox.center, count.ToString());
#endif
        }
    }
}
