using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseEditorExAttribute : PropertyAttribute
{
	public string isTrue;
	public string isFalse;

#if UNITY_EDITOR
	public virtual void OnGUI( UnityEngine.Object mono, MemberInfo info )
	{
	}
	public virtual void OnPreGUI( UnityEngine.Object mono, MemberInfo info )
	{
	}

	public virtual bool Passes( UnityEngine.Object mono, MemberInfo info )
	{
		if ( !string.IsNullOrEmpty( isTrue ) )
		{
			var value = GetValue( mono, isTrue );
			if ( string.IsNullOrEmpty( value ) ) return false;
			if ( value.Equals( "0", StringComparison.CurrentCultureIgnoreCase ) ) return false;
			if ( value.Equals( "false", StringComparison.CurrentCultureIgnoreCase ) ) return false;
		}

		if ( !string.IsNullOrEmpty( isFalse ) )
		{
			var value = GetValue( mono, isFalse );
			if ( !string.IsNullOrEmpty( value ) &&
				!value.Equals( "0", StringComparison.CurrentCultureIgnoreCase ) &&
				!value.Equals( "false", StringComparison.CurrentCultureIgnoreCase ) ) return false;
		}

		return true;
	}

	public string GetValue( System.Object obj, string name )
	{
		var member = obj.GetType().GetMember( name );
		if ( member == null || member.Length == 0 ) return "";

		if ( member[0] is FieldInfo )
		{
			return ( member[0] as FieldInfo ).GetValue( obj ).ToString();
        }

		return "";
	}
#endif
}
