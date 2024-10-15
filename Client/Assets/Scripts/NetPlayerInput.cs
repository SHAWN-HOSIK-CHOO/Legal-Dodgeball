using UnityEngine;
using UnityEngine.Serialization;
using Assets.Scripts.PacketEvent;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
        if(move!=newMoveDirection)
        {
           var CNET = GetComponent<NetComponent>();
           Event_TansformSync SYNC = new Event_TansformSync(CNET.ID, transform.position, GetComponent<Player>().CinemachineCameraTarget.transform.rotation);
           Event_MoveInput MOVE = new Event_MoveInput(CNET.ID, newMoveDirection);
           CNET.user.DefferedSend(SYNC.GetBytes());
           CNET.user.DefferedSend(MOVE.GetBytes());
        }
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        if (look != newLookDirection)
        {
            var CNET = GetComponent<NetComponent>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.ID, transform.position, GetComponent<Player>().CinemachineCameraTarget.transform.rotation);
            //Event_lookInput Look = new Event_lookInput(CNET.ID, newLookDirection);
            CNET.user.DefferedSend(SYNC.GetBytes());
            //CNET.user.DefferedSend(Look.GetBytes());
        }
        look = newLookDirection;
        
    }

    public void JumpInput(bool newJumpState)
    {
        if (jump != newJumpState)
        {
            var CNET = GetComponent<NetComponent>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.ID, transform.position, GetComponent<Player>().CinemachineCameraTarget.transform.rotation);
            Event_JumpInput JUMP = new Event_JumpInput(CNET.ID, newJumpState);
            CNET.user.DefferedSend(SYNC.GetBytes());
            CNET.user.DefferedSend(JUMP.GetBytes());
        }
        jump = newJumpState;
    }

    public void ThrowInput(bool newThrowState)
    {
        if (throwShoot != newThrowState)
        {
            var CNET = GetComponent<NetComponent>();
            Event_ThrowInput e = new Event_ThrowInput(CNET.ID, newThrowState);
            GetComponent<NetComponent>().user.DefferedSend(e.GetBytes());
        }
        throwShoot = newThrowState;
    }

    public void SprintInput(bool newSprintState)
    {
        if (sprint != newSprintState)
        {
            var CNET = GetComponent<NetComponent>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.ID, transform.position, GetComponent<Player>().CinemachineCameraTarget.transform.rotation);
            Event_sprintInput Sprint = new Event_sprintInput(CNET.ID, newSprintState);
            CNET.user.DefferedSend(Sprint.GetBytes());
        }
        sprint = newSprintState;
    }

    public void ChargeInput(bool newAimState)
    {
        if (charge != newAimState)
        {
            var CNET = GetComponent<NetComponent>();
            Event_chargeInput e = new Event_chargeInput(CNET.ID, newAimState);
            GetComponent<NetComponent>().user.DefferedSend(e.GetBytes());
        }
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