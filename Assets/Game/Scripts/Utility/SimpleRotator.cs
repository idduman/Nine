using UnityEngine;

namespace Garawell.Utility
{
    public class SimpleRotator : MonoBehaviour
    {
        public Vector3 rotationSpeed;

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}

