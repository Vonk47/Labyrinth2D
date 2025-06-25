using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace TestGame.Gameplay.Controllers
{
    public sealed class InputController
    {
        private readonly PlayerInputAction _inputActions;
        public PlayerInputAction InputActions => _inputActions;

        public event Action<Vector2> OnClick;
        public event Action OnClickEnd;

        public InputController()
        {
            _inputActions = new PlayerInputAction();
            _inputActions.Enable();
            _inputActions.Default.ClickAction.performed += OnClickPerformed;
            _inputActions.Default.ClickAction.canceled += OnClickEnded;

        }

        private void OnClickPerformed(CallbackContext context)
        {
            OnClick?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnClickEnded(CallbackContext context)
        {
            OnClickEnd?.Invoke();
        }

        public void Dispose()
        {
            _inputActions.Default.ClickAction.performed -= OnClickPerformed;
            _inputActions.Default.ClickAction.canceled -= OnClickEnded;
            _inputActions.Dispose();
        }

    }


}
