using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage( System.AttributeTargets.Method )]
public class ButtonAttribute : BaseEditorExAttribute
{
	public string Label;

	public ButtonAttribute( string name )
	{
		Label = name;
	}

#if UNITY_EDITOR
	public override void OnGUI( UnityEngine.Object mono, MemberInfo info )
	{
		if ( !Passes( mono, info ) ) return;

		if ( GUILayout.Button( Label ) )
		{
			UnityEditor.EditorUtility.SetDirty( mono );
			( info as MethodInfo ).Invoke( mono, null );
		}
	}
#endif
}

[System.AttributeUsage( System.AttributeTargets.All )]
public class ComponentHelpAttribute : BaseEditorExAttribute
{
	public string help;

	public ComponentHelpAttribute( string help )
	{
		this.help = help;
	}

#if UNITY_EDITOR
	public override void OnPreGUI( UnityEngine.Object mono, MemberInfo info )
	{
		if ( !Passes( mono, info ) ) return;

		EditorGUILayout.HelpBox( help, MessageType.Info );
	}
#endif
}

