using UnityEngine;
using System;
using System.Collections;

namespace Attack
{
    public class NormalBall : BallScriptBase
    {
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Wall"))
            {
                ColliedObjectTag = other.gameObject.tag;
                StartCoroutine(Timer());
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                ColliedObjectTag = other.gameObject.tag;
                Debug.Log("Player hit!");
                GManager.Instance.MinusPlayerHP(other.gameObject,10f);
                
                ExplodeOrDestroyThisBall();
            }
            ColliedObjectTag = String.Empty;
        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(0.7f);
            ExplodeOrDestroyThisBall();
        }
    }
}
