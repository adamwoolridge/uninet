using UnityEngine;
using System.Collections;

namespace Network
{
    public partial class Net
    {
#if SERVER
        public static Server sv = null;
#endif

#if CLIENT
        public static Client cl = null;
#endif
    }
}