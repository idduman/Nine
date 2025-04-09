using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace Garawell.Utility
{
    [ExecuteInEditMode]
    public class ChaseCam : MonoBehaviour
    {
        public Transform lookAt;
        public Transform[] moveTo;
        public Transform rollTo;
        public float chaseTime = 0.5f;

        public float shakeDuration = .5f;
        public float shakeAmount = 0.7f;
        public float decreaseFactor = 1.0f;

        private Vector3 shakeMoveTo;
        private Vector3 mVelocity;
        private Vector3 mRollVelocity;
        private int cameraMode;
        private float chaseTimeReserve;

        private void Awake()
        {
            chaseTimeReserve = chaseTime;
        }
#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
            {
                if (moveTo[cameraMode])
                    transform.position = moveTo[cameraMode].position;
                if (lookAt)
                {
                    if (!rollTo) transform.LookAt (lookAt);
                    else transform.LookAt (lookAt, rollTo.up);
                }
                // if (RollTo)
                //     transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, RollTo.rotation.eulerAngles.z));
            }
        }
#endif

        // Update is called once per frame
        void LateUpdate()
        {
            if(shakeDuration > 0)
            {
                transform.localPosition = shakeMoveTo + transform.InverseTransformPoint(moveTo[cameraMode].position) + Random.insideUnitSphere * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                if (moveTo.Length > 0)
                    transform.position = Vector3.SmoothDamp(transform.position, moveTo[cameraMode].position, ref mVelocity, chaseTime);
                if (lookAt)
                {
                    if (!rollTo) transform.LookAt(lookAt);
                    else transform.LookAt(lookAt, Vector3.SmoothDamp(transform.up, rollTo.up, ref mRollVelocity, chaseTime));
                }
            }
          
            // if (RollTo)
            //     transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, RollTo.rotation.eulerAngles.z));
        }

        public void DisableRotation()
        {
            chaseTime = chaseTimeReserve * 4f;
        }

        public void EnableRotation()
        {
            chaseTime = chaseTimeReserve;
        }

        public void Finish()
        {
            moveTo[cameraMode].localPosition = new Vector3(-6, 0, 0);
        }

        [Button("Shake")]
        public void Shake()
        {
            shakeMoveTo = transform.localPosition - transform.InverseTransformPoint(moveTo[cameraMode].position);
            shakeDuration = .5f;
        }

        public void SetCameraMode(int cMode)
        {
            if(moveTo.Length > cMode)
            {
                this.cameraMode = cMode;
            }
        }
    }
}
