using System;
using System.Collections;
using System.Collections.Generic;
using Attack;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

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
    public Transform        ballPlacePosition;
    public List<GameObject> pfPossessedBallList = new List<GameObject>();
    public int              ballIndex = 0;
    public float            gain              = 100f;
    
    private                 Animator   _animator;
    private static readonly int        Charge = Animator.StringToHash("Charge");
    private static readonly int        Throw  = Animator.StringToHash("Throw");

    private                  Vector2                  _screenCenterPoint;
    private                  Vector3                  _targetDirection = Vector3.zero;
    [SerializeField] private LayerMask                aimColliderMask  = new LayerMask();

    //Other Scripts from this gameObject
    private StarterAssetsInputs   _starterAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private float                 _sprintSpeedForRecovery;
    
    private GameObject       _throwableBall;
    
    //Player State
    [Space(5)]
    [Header("Do not change")]
    public EPlayerState currentPlayerState;
    
    private void Awake()
    {
        _animator            = this.GetComponent<Animator>();
        _animator.SetLayerWeight(1,0);
        _screenCenterPoint      = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _starterAssetsInputs    = GetComponent<StarterAssetsInputs>();
        _thirdPersonController  = GetComponent<ThirdPersonController>();
        _sprintSpeedForRecovery = _thirdPersonController.SprintSpeed;
        currentPlayerState      = EPlayerState.Default;
    }

    private void Start()
    {
        throwAimCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_starterAssetsInputs.charge)
        {
            if(GameManager.Instance.playerETurn == ETurn.Attack)
                AttackChargeAction();
        }
        
        if (_starterAssetsInputs.throwShoot)
        {
            if(GameManager.Instance.playerETurn == ETurn.Attack)
                AttackShootAction();
        }

        if(GameManager.Instance.playerETurn == ETurn.Attack)
            if (currentPlayerState == EPlayerState.Default)
            {
                throwAimCamera.gameObject.SetActive(false);
                _thirdPersonController.SprintSpeed = _sprintSpeedForRecovery;
                _animator.SetLayerWeight(1,0);
            }

        debug.text = "Player: " + GameManager.Instance.PlayerHP + "\n" 
                     + "Enemy: " + GameManager.Instance.EnemyHP;
    }

    private void AttackChargeAction()
    {
        _animator.SetLayerWeight(1,1);
        _animator.SetBool(Throw,  false);
        _animator.SetBool(Charge, true);
        _starterAssetsInputs.charge        = false;
        currentPlayerState                 = EPlayerState.Charge;
        _thirdPersonController.SprintSpeed = _thirdPersonController.MoveSpeed * 1.6f;
        throwAimCamera.gameObject.SetActive(true);
    }

    private void AttackShootAction()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray     ray                = Camera.main.ScreenPointToRay(_screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit,999f, aimColliderMask))
        {
            mouseWorldPosition   = hit.point;
            mouseWorldPosition.y = this.transform.position.y;
        }

        Vector3 throwDirection = ( mouseWorldPosition - this.transform.position ).normalized;
        this.transform.forward = throwDirection;
            
        _animator.SetBool(Charge, false);
        _animator.SetBool(Throw,  true);
        currentPlayerState              = EPlayerState.Throw;
        _starterAssetsInputs.throwShoot = false;
        StartCoroutine(WaitAndChangePlayerState(0.3f, EPlayerState.Default));
    }
    
    IEnumerator WaitAndChangePlayerState(float seconds, EPlayerState nextState)
    {
        yield return new WaitForSeconds(seconds);
        currentPlayerState = nextState;
    }

    //Called from Unity Animation System , (Animation Clip : Charge)
    public void ThrowBall()
    {
        if (_throwableBall == null)
        {
            Debug.Log("No Ball Attached");
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(_screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hit,999f, aimColliderMask))
            {
                _targetDirection = (hit.point - _throwableBall.transform.position).normalized;
            }
            
            _throwableBall.GetComponent<BallScriptBase>().ReleaseMe(_targetDirection, gain);
            _animator.SetBool(Throw, false);
        }
    }

    //Called from Unity Animation System , (Animation Clip : Throw)
    public void InstantiateBall()
    {
        GameObject ball = Instantiate(pfPossessedBallList[GameManager.Instance.PlayerBallIndex], ballPlacePosition);
        ball.transform.localPosition = Vector3.zero;
        _throwableBall               = ball;
    }
}
