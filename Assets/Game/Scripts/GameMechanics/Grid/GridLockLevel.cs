using System.Collections;
using DG.Tweening;
using Garawell.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Nine
{
    public class GridLockLevel : GridLock
    {
        [SerializeField] private int _levelToUnlock;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private SpriteRenderer _lockSprite;
        [SerializeField] private ParticleSystem _unlockParticle;
        
        public int LevelToUnlock => _levelToUnlock;
        
        private Sequence _unlockSequence;
        
        [Button]
        public override void UnlockWithAnimation()
        {
            if(_unlockSequence != null && _unlockSequence.IsActive())
                _unlockSequence.Kill();
            
            _unlockSequence = DOTween.Sequence().SetEase(Ease.Linear)
                .OnComplete(() => base.UnlockWithAnimation());
            _unlockSequence.Append(
                _lockSprite.transform.DOShakeRotation(1f, new Vector3(0f, 0f, 25f), 16, 120f, false)
                    .SetEase(Ease.InCubic));
            _unlockSequence.Insert(0f, _lockSprite.transform.DOScale(0.8f*Vector3.one, 1f)
                .SetEase(Ease.InCubic));
            _unlockSequence.Append(_lockSprite.transform.DOScale(1.2f * Vector3.one, 0.5f));
            _unlockSequence.Insert(1f, _lockSprite.DOFade(0f, 0.5f));
            _unlockSequence.InsertCallback(1.4f, () => _unlockParticle.Play());
        }
    }
}