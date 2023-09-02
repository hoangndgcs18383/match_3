using UnityEngine;
using Zeff.Core.Parser;

namespace Zeff.Extensions
{
    public static class DebugFormat 
    {
        public static void ToString(object args)
        {
            if (args != null)
                Debug.Log("<b><color=#00ff00>[{0}]</color></b> : <color=#ffff33>{1}</color> ".Replace("{0}", args.GetType().Name)
                    .Replace("{1}",args.ToJson()));
        }
    }
}
