using UnityEngine;
using UnityEngine.Serialization;
using Assets.Scripts.PacketEvent;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace Server
{
    public class NetPlayerInput : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool charge;
        public bool throwShoot;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnThrow(InputValue value)
        {
            ThrowInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnCharge(InputValue value)
        {
            ChargeInput(value.isPressed);
        }
#endif
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;

        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void ThrowInput(bool newThrowState)
        {
            throwShoot = newThrowState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void ChargeInput(bool newAimState)
        {
            charge = newAimState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}