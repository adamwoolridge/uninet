using UnityEngine;

namespace Facepunch
{
    public static class Instantiate
    {
        public static UnityEngine.GameObject GameObject( UnityEngine.GameObject go )
        {
            var info = WrapStart( go );
            var co = UnityEngine.GameObject.Instantiate( go );
            WrapEnd( go, info );
            return co;
        }

        public static UnityEngine.GameObject GameObject( UnityEngine.GameObject go, Vector3 pos, Quaternion rot )
        {
            var info = WrapStart( go );
            var co = UnityEngine.GameObject.Instantiate( go, pos, rot ) as GameObject;
            WrapEnd( go, info );
            return co;
        }

        public static long WrapStart( GameObject go )
        {
            return 0;

           // System.GC.Collect();
            
            //return Profiler.usedHeapSize;
        }

        public static void WrapEnd( GameObject go, long memory )
        {
            //System.GC.Collect();
           // var diff = Profiler.usedHeapSize - memory;

           // System.IO.File.AppendAllText( "GameObjectCreations.txt", string.Format( "{0}\t{1}\n", diff, go.name ) );
        }

    }
}