using UnityEngine;
using System.Collections;

public enum NetworkMessageType : byte
{
    Chat,

    Entity_UpdateTransform,
    Entity_Destroy,
    Entity_LocalPlayerCreated,

    Cell_Destroy,
}