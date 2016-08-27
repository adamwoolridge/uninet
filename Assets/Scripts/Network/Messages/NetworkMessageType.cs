using UnityEngine;
using System.Collections;

public enum NetworkMessageType : byte
{
    Entity_UpdateTransform,
    Entity_Destroy,
}