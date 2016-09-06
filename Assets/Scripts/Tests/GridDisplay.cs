using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridDisplay : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Grid.Instance == null) return;

        Gizmos.color = Color.gray;
        foreach (GridCell cell in Grid.Instance.Cells)
        {
            Gizmos.DrawWireCube(cell.BoundingBox.center, cell.BoundingBox.size);


            int count = cell.Entities.Count;
            Handles.Label(cell.BoundingBox.center, count.ToString());

        }

        if (Grid.Instance != null)
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                NetworkEntity ent = selectedObject.GetComponent<NetworkEntity>();

                Gizmos.color = Color.green;

                int entCount = 0;

                if (ent)
                {
                    GridCell currentCell = Grid.Instance.GetCell(ent.transform.position);
                    
                    List<GridCell> neighbouringCells = Grid.Instance.GetNeighbours(currentCell, 2);

                    foreach (GridCell nc in neighbouringCells)
                    {
                        Gizmos.DrawWireCube(nc.BoundingBox.center, nc.BoundingBox.size);
                        entCount += nc.Entities.Count;
                    }

                    Gizmos.color = Color.red;
                    Handles.Label(ent.transform.position, "Visible ent count: " + entCount.ToString());
                }                
            }
        }
    }
#endif
}
