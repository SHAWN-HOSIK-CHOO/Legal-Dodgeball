using UnityEngine;
using Assets.Scripts.PacketEvent;
using Unity.VisualScripting;
using Server;
using System.ComponentModel;




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

    public bool NetEnable = true;

    float LookDealy = 0;
    float LookTimeout = 0.2f;

    private void Start()
    {
        NetEnable = GetComponent<NetViewer>().IsMine && !GameManager.instance.IsServer;
    }


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
        if (move != newMoveDirection && NetEnable)
        {
            var CNET = GetComponent<NetViewer>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.NetID, transform.position, GetComponent<Assets.Scripts.Network.Player>().CinemachineCameraTarget.transform.rotation);
            Event_MoveInput MOVE = new Event_MoveInput(CNET.NetID, newMoveDirection);
            CNET.user?.DefferedSend(SYNC.GetBytes());
            CNET.user?.DefferedSend(MOVE.GetBytes());
        }
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {

        LookDealy += Time.deltaTime;
        if (look != newLookDirection && NetEnable)
        {
            if (LookDealy > LookTimeout)
            {

                var CNET = GetComponent<NetViewer>();
                Event_TansformSync SYNC = new Event_TansformSync(CNET.NetID, transform.position, GetComponent<Assets.Scripts.Network.Player>().CinemachineCameraTarget.transform.rotation);
                CNET.user?.DefferedSend(SYNC.GetBytes());
                LookDealy = 0;
            }
        }
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        if (jump != newJumpState && NetEnable)
        {
            var CNET = GetComponent<NetViewer>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.NetID, transform.position, GetComponent<Assets.Scripts.Network.Player>().CinemachineCameraTarget.transform.rotation);
            Event_JumpInput JUMP = new Event_JumpInput(CNET.NetID, newJumpState);
            CNET.user?.DefferedSend(SYNC.GetBytes());
            CNET.user?.DefferedSend(JUMP.GetBytes());
        }
        jump = newJumpState;
    }

    public void ThrowInput(bool newThrowState)
    {
        if (throwShoot != newThrowState && NetEnable)
        {
            var CNET = GetComponent<NetViewer>();
            Event_ThrowInput THROW = new Event_ThrowInput(CNET.NetID, newThrowState);
            CNET.user?.DefferedSend(THROW.GetBytes());
        }
        throwShoot = newThrowState;
    }

    public void SprintInput(bool newSprintState)
    {
        if (sprint != newSprintState && NetEnable)
        {
            var CNET = GetComponent<NetViewer>();
            Event_TansformSync SYNC = new Event_TansformSync(CNET.NetID, transform.position, GetComponent<Assets.Scripts.Network.Player>().CinemachineCameraTarget.transform.rotation);
            Event_sprintInput Sprint = new Event_sprintInput(CNET.NetID, newSprintState);
            CNET.user?.DefferedSend(Sprint.GetBytes());
        }
        sprint = newSprintState;
    }

    public void ChargeInput(bool newAimState)
    {
        if (charge != newAimState && NetEnable)
        {
            var CNET = GetComponent<NetViewer>();
            Event_chargeInput CHARGE = new Event_chargeInput(CNET.NetID, newAimState);
            CNET.user?.DefferedSend(CHARGE.GetBytes());
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