using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestGrid : MonoBehaviour {

    Grid grid;

	// Use this for initialization
	void Start () {
        grid = new Grid(10, 10, 10f);
	}

    void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach(GridCell cell in grid.Cells)
        {
            Gizmos.DrawWireCube(cell.BoundingBox.center, cell.BoundingBox.size);

#if UNITY_EDITOR
            Handles.Label(cell.BoundingBox.center, cell.Entities.Count.ToString());
#endif
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
