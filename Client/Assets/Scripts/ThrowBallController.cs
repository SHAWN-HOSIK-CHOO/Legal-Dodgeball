using System;
using System.Collections;
using System.Collections.Generic;
using Attack;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

public enum EPlayerState
{
    Default,
    Charge,
    Throw,
    Count
}

public class ThrowBallController : MonoBehaviour
{
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
            _animator.SetLayerWeight(1,1);
            _animator.SetBool(Throw,  false);
            _animator.SetBool(Charge, true);
            _starterAssetsInputs.charge        = false;
            currentPlayerState                 = EPlayerState.Charge;
            _thirdPersonController.SprintSpeed = _thirdPersonController.MoveSpeed;
            throwAimCamera.gameObject.SetActive(true);
        }
        
        if (_starterAssetsInputs.throwShoot)
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

        if (currentPlayerState == EPlayerState.Default)
        {
            throwAimCamera.gameObject.SetActive(false);
            _thirdPersonController.SprintSpeed = _sprintSpeedForRecovery;
            _animator.SetLayerWeight(1,0);
        }
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
            if (Physics.Raycast(ray, out RaycastHit hit,999f, aimColliderMask))
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
        _throwableBall               = ball;
    }
}
