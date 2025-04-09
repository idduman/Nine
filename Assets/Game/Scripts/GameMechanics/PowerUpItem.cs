using System;
using DG.Tweening;
using Garawell.Managers;
using TMPro;
using UnityEngine;

namespace Nine
{
    public class PowerUpItem : MonoBehaviour
    {
        [SerializeField] private PowerUpType _powerUpType;
        [SerializeField] private int _price = 100;
        [SerializeField] private int _unlockLevel = 3;
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _pricePanel;
        [SerializeField] private Transform _grayOut;
        [SerializeField] private TMP_Text _priceText;

        public int UnlockLevel => _unlockLevel;
        public int Price => _price;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                _tileTween.Kill();
                var scale = _selected ? 1.5f : 1f;
                _tileTween = _pivot.DOScale(scale, 0.25f)
                    .SetEase(Ease.Linear);
                
                _pricePanel.gameObject.SetActive(!_selected);
            }
        }

        public bool Available
        {
            get => _available;
            private set
            {
                _available = value;
                if (!_grayOut)
                    return;
                
                _grayOut.gameObject.SetActive(!_available);
            }
        }
        
        
        private bool _available;
        private bool _selected;
        private Tween _tileTween;
        
        public PowerUpType PowerUpType => _powerUpType;

        private void Start()
        {
            _priceText.text = _price.ToString();
            Available = MainManager.Instance.CurrencyManager.currencies[0].currencyAmount >= _price;
        }

        private void Update()
        {
            if (MainManager.Instance.CurrencyManager.currencies[0].currencyAmount >= _price)
            {
                if (!Available)
                    Available = true;
            }
            else
            {
                if (Available)
                    Available = false;
            }
        }
        
        
    }
}
