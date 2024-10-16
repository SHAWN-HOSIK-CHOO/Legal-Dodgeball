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
            else if (other.gameObject.CompareTag("Enemy"))
            {
                ColliedObjectTag = other.gameObject.tag;
                Debug.Log("Enemy hit!");
                GameManager.Instance.MinusEnemyHP(10f);
                GameManager.Instance.PlusPlayerHP(10f);
                
                ExplodeOrDestroyThisBall();
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                ColliedObjectTag = other.gameObject.tag;
                Debug.Log("Player hit!");
                GameManager.Instance.PlusEnemyHP(10f);
                GameManager.Instance.MinusPlayerHP(10f);
                
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
