using UnityEngine;
using System.Collections;

public class NeedsMouseButtons : ListComponent<NeedsMouseButtons> 
{
	public static bool AnyActive()
	{
		return InstanceList.Count > 0;
	}
}
