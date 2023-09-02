using System.Collections.Generic;
using UnityEngine;

namespace Match_3
{
    [CreateAssetMenu(fileName = "IconConfig", menuName = "Match 3/Icon Config")]
    public class IconConfig : ScriptableObject
    {
        public List<SpriteData> _iconData = new List<SpriteData>();
    }
}