using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match_3
{
    public class IconManager : MonoBehaviour
    {
        public static IconManager Current;
        
        private Dictionary<string, SpriteData> _iconData = new Dictionary<string, SpriteData>();

        private void Awake()
        {
            if (Current == null)
            {
                Current = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(this);
                }
            }
        }

        private void Start()
        {
            InitIcon();
        }

        private void InitIcon()
        {
            _iconData = new Dictionary<string, SpriteData>();
            
            var iconConfig = Resources.Load<IconConfig>("Config/IconConfig");
            
            foreach (var icon in iconConfig._iconData)
            {
                _iconData.Add(icon.id, icon);
            }
        }

        public Sprite GetIcon(string id)
        {
            if (_iconData.ContainsKey(id))
            {
                return _iconData[id].sprite;
            }

            return null;
        }
    }
}