using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public enum EDefenseType
{
   None,
   SpawnObject,
   NonSpawnObject,
   Count
}

public class PlayerDefenseController : MonoBehaviour
{
   [Header("Set From Editor")]
   public List<GameObject> pfDefenseObjectList = new List<GameObject>();

   public Transform objectPlacePosition;
   
   //Private Fields
   private GameObject          _pfChosenObject;
   private StarterAssetsInputs _starterAssetsInputs;
   
   //Aim
   private                  Vector2   _screenCenterPoint;
   private                  Vector3   _targetDirection = Vector3.zero;
   [SerializeField] private LayerMask aimColliderMask  = new LayerMask();

   [Space(5)] [Header("Do not change")] 
   public EDefenseType chosenActionType;

   private void Awake()
   {
      _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
      //DEBUG. must be changed to none
      chosenActionType     = EDefenseType.SpawnObject;
   }

   private void Update()
   {
      if (_starterAssetsInputs.charge)
      {
         if (GameManager.Instance.playerETurn == ETurn.Defense)
         {
            DefenseAction(chosenActionType);
         }
      }

      if (_starterAssetsInputs.throwShoot)
      {
         if (GameManager.Instance.playerETurn == ETurn.Defense)
         {
            
         }
      }
   }

   private void DefenseAction(EDefenseType actionType)
   {
      if (actionType == EDefenseType.SpawnObject)
      {
         _pfChosenObject =
            Instantiate(pfDefenseObjectList[GameManager.Instance.PlayerDefenseIndex], objectPlacePosition);
      }
      else if (actionType == EDefenseType.NonSpawnObject)
      {
         
      }
      else
      {
         Debug.Log("Please choose action type");
      }
   }
}
