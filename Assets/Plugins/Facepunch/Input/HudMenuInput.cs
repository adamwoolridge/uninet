using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Used on the inventory/crafting menus to surpress keyboard input when using 
/// a text box.
/// </summary>
public class HudMenuInput : ListComponent<HudMenuInput>
{
    /// <summary>
    /// Return true if any Input Fields with this component are currently being typed into
    /// </summary>
    public static bool AnyActive()
    {
        foreach( var instance in InstanceList )
        {
            if ( instance.IsCurrentlyActive() )
                return true;
        }

        return false;
    }


    private InputField inputField;

    void Start()
    {
        inputField = GetComponent<InputField>();
    }

    bool IsCurrentlyActive()
    {
        if ( inputField == null ) return false;

        return inputField.isFocused;
    }
}
