using Garawell.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nine
{
    public class ShapeTile : MonoBehaviour
    {
        [SerializeField] private GameObject _highlightBlock;
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _numberParent;
        public ColorCode Color { get; private set; }
        public int Number { get; private set; } = 0;

        public int StackSize = 0;

        public Transform Pivot => _pivot;
        public Transform NumberParent => _numberParent;
        
        public Vector2Int Index { get; private set; }

        private GameObject _numberObject;
        
        private Renderer _tileRenderer;
        private Material _originalMat;
        public void SetColor(ColorCode color)
        {
            Color = color;
            Number = (int)Color;
            
            var numberPrefab = MainManager.Instance.AssetManager.GetPrefab(Number.ToString());

            if (!_numberObject)
            {
                if(_numberParent.childCount > 0)
                    for (int i = _numberParent.childCount - 1; i >= 0; i--)
                    {
                        var o = _numberParent.GetChild(i);
                        Destroy(o.gameObject);
                    }
            }
            else
            {
                Destroy(_numberObject);
            }
            _numberObject = Instantiate(numberPrefab, _numberParent);
            _numberObject.transform.localScale = Vector3.one;
            _originalMat = MainManager.Instance.AssetManager.GetMaterial(color.ToString());
            
            _tileRenderer = _numberObject.GetComponent<Renderer>();
            _tileRenderer.material = _originalMat;
        }

        public void Increment()
        {
            if(Number < 9)
                SetColor((ColorCode)((int)Color + 1));
        }
        
        public void Decrement()
        {
            if(Number > 2)
                SetColor((ColorCode)((int)Color - 1));
        }

        public void SetIndex(Vector2Int index)
        {
            Index = index;
        }

        public void ToggleShadow(bool active)
        {
            _tileRenderer.shadowCastingMode = active ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }

        public void ToggleHighlight(bool highlight)
        {
            _highlightBlock.SetActive(highlight);
            _numberParent.gameObject.SetActive(!highlight);
        }

        public void HideNumber()
        {
            _highlightBlock.GetComponent<Renderer>().material = _originalMat;
            _numberObject.SetActive(false);
            _highlightBlock.transform.SetParent(_numberParent);
            _highlightBlock.SetActive(true);
        }

        public void ResetNumberRotation()
        {
            _numberParent.rotation = Quaternion.Euler(90f, 180f, 0f);
        }
    }
}
