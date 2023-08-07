using System;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public enum PowerUpType
    {
        Shuffle,
        Suggests,
        Undo,
    }
    public class PowerUpItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text countText;
        [SerializeField] private PowerUpType powerUpType;
        private Action<PowerUpType> _onClick;
        public PowerUpType PowerUpType { get => powerUpType; set => powerUpType = value; }

        public void Init(int count, Action<PowerUpType> onClick)
        {
            _onClick = onClick;
            UpdateText(count);
            Debug.Log("Init: " + count);
        }
        
        private void UpdateText(int count)
        {
            countText.SetText(count.ToString());
        }
        
        public void OnClick()
        {
            _onClick?.Invoke(powerUpType);
            UpdateText(GameConfig.GetPowerUpCount(powerUpType));
            Debug.Log("_onClick: " + powerUpType);
        }
    }
}
