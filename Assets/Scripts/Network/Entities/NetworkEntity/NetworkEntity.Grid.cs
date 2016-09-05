using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class NetworkEntity : MonoBehaviour
{
    public GridCell gridCell;
    public uint cellID;

    // Called on server when an NE's transform has changed
    public void UpdateGrid()
    {
        if (Grid.Instance == null) return;

        if (NetworkManager.IsServer)
        {
            GridCell newCell = Grid.Instance.GetCell(transform.position);

            if (newCell == null) return; 

            // In a new cell?
            if (gridCell == null || gridCell != newCell)
            {                
                Grid.Instance.TransferEntity(this, gridCell, newCell);          
            }
        }
    }    
}
