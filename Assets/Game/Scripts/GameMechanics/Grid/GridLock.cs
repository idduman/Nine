using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Nine
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridLock : MonoBehaviour
    {
        [SerializeField] private GameObject _display;
        
        protected List<GridElement> _blockedElements = new List<GridElement>();

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            private set
            {
                _locked = value;
                _display.SetActive(_locked);
                foreach (var e in _blockedElements)
                {
                    if(_locked)
                        e.Lock();
                    else
                        e.Unlock();
                }
            }
        }

        public virtual void Initialize(GridElement[] elements)
        {
            var bounds = GetComponent<BoxCollider>().bounds;
            _blockedElements = new List<GridElement>();
            foreach (GridElement e in elements)
            {
                var pos = e.transform.position;
                if (pos.x > bounds.min.x && pos.x < bounds.max.x
                                         && pos.z > bounds.min.z && pos.z < bounds.max.z)
                {
                    _blockedElements.Add(e);
                }
            }
        }

        [Button]
        public virtual void Lock() { Locked = true; }
        [Button]
        public virtual void Unlock() { Locked = false; }

        public virtual void Deactivate()
        {
            foreach (var e in _blockedElements)
            {
                e.Deactivate();
            }
            gameObject.SetActive(false);
        }

        public virtual void Activate()
        {
            foreach (var e in _blockedElements)
            {
                e.Activate();
            }
            gameObject.SetActive(true);
        }

        public virtual void UnlockWithAnimation()
        {
            StartCoroutine(UnlockRoutine());
        }
        
        private IEnumerator UnlockRoutine()
        {
            foreach (var e in _blockedElements)
            {
                e.UnlockWithAnimation();
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
