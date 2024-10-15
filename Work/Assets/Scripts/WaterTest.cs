using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTest : MonoBehaviour
{
   public float lastingTime = 3.0f;
   private void Start()
   {
      StartCoroutine(StartTimer());
   }

   IEnumerator StartTimer()
   {
      yield return new WaitForSeconds(lastingTime);
      Destroy(this.gameObject);
   }

   private void OnTriggerStay(Collider other)
   {
      Debug.Log(other.gameObject.tag);
   }
}
