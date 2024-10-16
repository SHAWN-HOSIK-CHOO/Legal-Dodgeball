using System;
using System.Collections;
using System.Collections.Generic;
using Attack;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using Client;
using Server;
using Assets.Scripts.PacketEvent;

public enum EPlayerState
{
    Default,
    Charge,
    Throw,
    Count
}

public class ThrowBallController : MonoBehaviour
{
    //Debug Object
    public TMP_Text debug;

    //SetFromEditor
    [Header("Set From Editor")]
    public CinemachineVirtualCamera throwAimCamera;
    public Transform ballPlacePosition;
    public List<GameObject> pfPossessedBallList = new List<GameObject>();
    public int ballIndex = 0;
    public float gain = 100f;

    private Animator _animator;
    private static readonly int Charge = Animator.StringToHash("Charge");
    private static readonly int Throw = Animator.StringToHash("Throw");

    private Vector2 _screenCenterPoint;
    private Vector3 _targetDirection = Vector3.zero;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();

    //Other Scripts from this gameObject
    private NetPlayerInput _NetPlayerInput;
    private Assets.Scripts.Network.Player _Player;
    private float _sprintSpeedForRecovery;

    private GameObject _throwableBall;

    //Player State
    [Space(5)]
    [Header("Do not change")]
    public EPlayerState currentPlayerState;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
        _animator.SetLayerWeight(1, 0);
        _screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _NetPlayerInput = GetComponent<NetPlayerInput>();
        _Player = GetComponent<Assets.Scripts.Network.Player>();
        _sprintSpeedForRecovery = _Player.SprintSpeed;
        currentPlayerState = EPlayerState.Default;
    }

    private void Start()
    {
        var CNET = GetComponent<NetViewer>();
        if (CNET.IsMine)
        {
            throwAimCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_NetPlayerInput.charge)
        {
            AttackChargeAction();
        }

        if (currentPlayerState == EPlayerState.Default)
        {
            var CNET = GetComponent<NetViewer>();
            if (CNET?.IsMine == true)
            {
                throwAimCamera.gameObject.SetActive(false);
            }
            _Player.SprintSpeed = _sprintSpeedForRecovery;
            _animator.SetLayerWeight(1, 0);
        }

        if (GetComponent<NetViewer>().IsMine)
        {
            debug.text = "Player: " + GManager.Instance.PlayerHP + "\n"
                         + "Enemy: " + GManager.Instance.Player2HP;
        }
    }

    private void AttackChargeAction()
    {
        _animator.SetLayerWeight(1, 1);
        _animator.SetBool(Throw, false);
        _animator.SetBool(Charge, true);
        _NetPlayerInput.charge = false;
        currentPlayerState = EPlayerState.Charge;
        _Player.SprintSpeed = _Player.MoveSpeed * 1.6f;

        var CNET = GetComponent<NetViewer>();
        if (CNET?.IsMine == true)
        {
            throwAimCamera.gameObject.SetActive(true);
        }

    }

    public void AttackShootAction()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(_screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
        {
            mouseWorldPosition = hit.point;
            mouseWorldPosition.y = this.transform.position.y;
        }

        Vector3 throwDirection = (mouseWorldPosition - this.transform.position).normalized;
        this.transform.forward = throwDirection;

        _animator.SetBool(Charge, false);
        _animator.SetBool(Throw, true);
        currentPlayerState = EPlayerState.Throw;
        _NetPlayerInput.throwShoot = false;
        StartCoroutine(WaitAndChangePlayerState(0.3f, EPlayerState.Default));
    }

    IEnumerator WaitAndChangePlayerState(float seconds, EPlayerState nextState)
    {
        yield return new WaitForSeconds(seconds);
        currentPlayerState = nextState;
    }

    public void SetBallIndex(int index)
    {
        if (index >= pfPossessedBallList.Count)
        {
            Debug.Log("Index overflow, automatically set to 0");
            ballIndex = 0;
        }
        else
        {
            ballIndex = index;
        }
    }

    public void ThrowBall()
    {
        if (_throwableBall == null)
        {
            Debug.Log("No Ball Attached");
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(_screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
            {
                _targetDirection = (hit.point - _throwableBall.transform.position).normalized;
            }

            _throwableBall.GetComponent<BallScriptBase>().ReleaseMe(_targetDirection, gain);
            _animator.SetBool(Throw, false);
        }
    }

    public void InstantiateBall()
    {
        GameObject ball = Instantiate(pfPossessedBallList[ballIndex], ballPlacePosition);
        ball.transform.localPosition = Vector3.zero;
        _throwableBall = ball;
    }
}
