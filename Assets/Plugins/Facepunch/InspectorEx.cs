using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( UnityEngine.Object ), true )]
[CanEditMultipleObjects]
public class InspectorEx : Editor
{
	private UnityEngine.Object lastTarget;
	private MemberInfo[] members;

	public override void OnInspectorGUI()
	{
		if ( targets.Length == 1 )
		{
			if ( lastTarget != target || members == null )
			{
				lastTarget = target;
				members = target.GetType()
					.GetMembers( BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy )
					.Where( o => Attribute.IsDefined( o, typeof( BaseEditorExAttribute ) ) )
					.ToArray();
			}

			foreach ( var member in members )
			{
				foreach ( BaseEditorExAttribute attr in member.GetCustomAttributes( typeof( BaseEditorExAttribute ), true ) )
				{
					attr.OnPreGUI( target, member );
				}
			}
		}

		base.OnInspectorGUI();

		if ( targets.Length == 1 )
		{
			foreach ( var member in members )
			{
				foreach ( BaseEditorExAttribute attr in member.GetCustomAttributes( typeof( BaseEditorExAttribute ), true ) )
				{
					attr.OnGUI( target, member );
				}
			}
		}
	}
}
#endif
