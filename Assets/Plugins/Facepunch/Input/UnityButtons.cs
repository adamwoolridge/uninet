using UnityEngine;
using System.Collections;

namespace Facepunch
{
    public static class UnityButtons
    {
        static bool isRegistered = false;

        public static void Register()
        {
            if ( isRegistered  )
            {
                Debug.LogError( "UnityButtons.Register called twice!" );
                return;
            }

            isRegistered = true;

            //
            // Make all of the unity keys accessible to Facepunch.Input
            //
            foreach ( UnityEngine.KeyCode key in System.Enum.GetValues( typeof( UnityEngine.KeyCode ) ) )
            {
                if ( key == KeyCode.None ) continue;

                var keyName = key.ToString();
                var localKey = key;
                var isFKey = keyName.Length == 2 && keyName.StartsWith( "F" );
                var isMouseButton = keyName.StartsWith( "mouse", System.StringComparison.CurrentCultureIgnoreCase );

                if ( keyName.StartsWith( "Alpha" ) ) keyName = keyName.Replace( "Alpha", "" ); // Alpha3 => 3

                Facepunch.Input.AddButton( keyName, () => 
                {
                    var isKeyDown = UnityEngine.Input.GetKey( localKey );
                    if ( !isKeyDown ) return false;

                    if ( !isFKey && !KeyBinding.IsOpen && ( NeedsKeyboard.AnyActive() || HudMenuInput.AnyActive() ) ) return false;
                    if ( isMouseButton && NeedsMouseButtons.AnyActive() ) return false;

                    return true;
                });
            }

            //
            // Mouse wheel
            //
            {
                float wheelValue = 0.0f;
                float lastWheelValue = 0.0f;

                Facepunch.Input.AddButton( "MouseWheelUp", 
                () => 
                {
                    if ( lastWheelValue > 0.0f ) wheelValue = 0.0f;
                    lastWheelValue = wheelValue;
                    wheelValue = 0.0f;
                    return lastWheelValue > 0.0f;
                }, 
                () =>
                {
                    if ( Cursor.visible )
                    {
                        wheelValue = 0;
                        return;
                    }

                    wheelValue = Mathf.Max( wheelValue, UnityEngine.Input.GetAxis( "Mouse ScrollWheel" ) );
                } );
            }

            {
                float wheelValue = 0.0f;
                float lastWheelValue = 0.0f;

                Facepunch.Input.AddButton( "MouseWheelDown",
                () =>
                {
                    if ( lastWheelValue > 0.0f ) wheelValue = 0.0f;
                    lastWheelValue = wheelValue;
                    wheelValue = 0.0f;
                    return lastWheelValue > 0.0f;
                },
                () =>
                {
                    if ( Cursor.visible )
                    {
                        wheelValue = 0;
                        return;
                    }

                    wheelValue = Mathf.Max( wheelValue, UnityEngine.Input.GetAxis( "Mouse ScrollWheel" ) * -1.0f );
                } );
            }
        }

    }
}