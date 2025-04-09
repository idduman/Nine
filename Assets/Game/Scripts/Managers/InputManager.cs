using UnityEngine;

namespace Garawell.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputControl controlSystem;

        public void Initialize()
        {
            controlSystem.Initialize();
            controlSystem.SetInput(true);
        }
    }
}