//#define POOL_DIAGNOSTICS
using System.Collections.Generic;

//
// This is an object list
// Its purpose is to hack around using params - which create garbage
//

namespace Facepunch
{
    public struct ObjectList
    {
        object object0;
        object object1;
        object object2;
        object object3;
        object object4;

        public ObjectList( object object0 = null, object object1 = null, object object2 = null, object object3 = null, object object4 = null )
        {
            this.object0 = object0;
            this.object1 = object1;
            this.object2 = object2;
            this.object3 = object3;
            this.object4 = object4;
        }

        public object Get( int i )
        {
            switch ( i )
            {
                case 0: return object0;
                case 1: return object1;
                case 2: return object2;
                case 3: return object3;
                case 4: return object4;
            }

            return null;
        }

        public int Length
        {
            get
            {
                if ( object0 == null ) return 0;
                if ( object1 == null ) return 1;
                if ( object2 == null ) return 2;
                if ( object3 == null ) return 3;
                if ( object4 == null ) return 4;

                return 5;
            }
        }
    }
}