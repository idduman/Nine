using System;
using DG.Tweening;
using Garawell.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nine
{
    public class GridElement : MonoBehaviour
    {
        [SerializeField] private GameObject _lockBlock;
        public Vector2Int Index { get; private set; }
        
        public ShapeTile Tile { get; private set; }
        
        private Tween _unlockTween;

        public bool IsActive { get; private set; } = true;
        
        public bool IsFilled { get; private set; }

        public bool IsLocked { get; private set; }

        public void Activate()
        {
            gameObject.SetActive(true);
            IsActive = true;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }

        public void Fill(ShapeTile tile)
        {
            Tile = tile;
            IsFilled = true;
            Tile.transform.SetParent(transform);
        }

        public void Empty()
        {
            if (!IsFilled)
                return;
            
            Destroy(Tile.gameObject);
            Tile = null;
            IsFilled = false;
        }

        public void Detach()
        {
            Tile = null;
            IsFilled = false;
        }

        public void Lock()
        {
            IsLocked = true;
            _lockBlock.SetActive(true);
        }

        public void Unlock()
        {
            IsLocked = false;
            _lockBlock.SetActive(false);
        }


        public void SetIndex(int x, int y)
        {
            Index = new Vector2Int(x, y);
        }

        public void UnlockWithAnimation()
        {
            _unlockTween.Kill();
            _unlockTween = transform.DOScaleY(1.694915f, 0.2f).SetEase(Ease.OutBounce)
                .OnComplete(Unlock);
        }
    }
}
