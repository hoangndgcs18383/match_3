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
        
        [SerializeField] private GameObject countIcon;
        [SerializeField] private GameObject adsIcon;
        
        private Action<PowerUpType> _onClick;
        public PowerUpType PowerUpType { get => powerUpType; set => powerUpType = value; }

        public void Init(int count, Action<PowerUpType> onClick)
        {
            _onClick = onClick;
            UpdateCount(count);
        }
        
        public void UpdateCount(int count)
        {
            countText.SetText(count.ToString());
            if (IsEnoughPowerUp())
            {
                countIcon.SetActive(true);
                adsIcon.SetActive(false);
            }
            else
            {
                countIcon.SetActive(false);
                adsIcon.SetActive(true);
            }
        }
        
        public void OnClick()
        {
            _onClick?.Invoke(powerUpType);
            UpdateCount(GameConfig.GetPowerUpCount(powerUpType));
        }
        
        private bool IsEnoughPowerUp()
        {
            return GameConfig.GetPowerUpCount(powerUpType) > 0;
        }
    }
}
