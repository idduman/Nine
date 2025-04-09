using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class MouseFollower : MonoBehaviour
{
    [SerializeField] private bool touchAnimation = true;
    [SerializeField] GameObject particlePrefab;

    public float offsetX = 0;
    public float offsetY = 0;

    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    void Update()
    {
        transform.position = new Vector2(Input.mousePosition.x + offsetX, Input.mousePosition.y + offsetY);
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            image.enabled = image.enabled ? false : true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (particlePrefab != null)
                Instantiate(particlePrefab.gameObject, Input.mousePosition, Quaternion.identity, transform);

            if (!touchAnimation) return;
            transform.DOScale(transform.localScale * .8f, .1f);
            transform.DORotate(new Vector3(0, 0, 30), .1f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!touchAnimation) return;
            transform.DOScale(Vector3.one, .1f);
            transform.DORotate(Vector3.zero, .1f);
        }
    }
}
