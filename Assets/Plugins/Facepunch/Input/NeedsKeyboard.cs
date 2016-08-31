using UnityEngine;
using System.Collections;

public class NeedsKeyboard : ListComponent<NeedsKeyboard>
{
	public UnityEngine.Events.UnityEvent onNoKeysDown;

	public static bool AnyActive()
	{
		return InstanceList.Count > 0;
	}

	bool watchForNoKeys = false;

	protected override void OnEnable()
	{
		base.OnEnable();

		watchForNoKeys = true;
	}

	public void Update()
	{
		if ( !watchForNoKeys ) return;
		if ( Input.anyKey ) return;

		watchForNoKeys = false;
		onNoKeysDown.Invoke();
	}
}
