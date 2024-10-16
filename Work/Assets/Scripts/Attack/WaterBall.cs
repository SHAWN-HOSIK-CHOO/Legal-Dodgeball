using UnityEngine;
using System;
using System.Collections;

namespace Attack
{
    public class WaterBall : BallScriptBase
    {
        public GameObject pfWater;
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);

            if (ColliedObjectTag == "Floor")

                Instantiate(pfWater, transform.position - new Vector3(0f, 0.2f, 0f), pfWater.transform.rotation);
            
            Destroy(this.gameObject);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Wall"))
            {
                ColliedObjectTag = other.gameObject.tag;
                ExplodeOrDestroyThisBall();
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                ColliedObjectTag = other.gameObject.tag;
                Debug.Log("Enemy hit!");
                GameManager.Instance.MinusEnemyHP(10f);
                ExplodeOrDestroyThisBall();
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                ColliedObjectTag = other.gameObject.tag;
                Debug.Log("Player hit!");
                GameManager.Instance.MinusEnemyHP(10f);
                ExplodeOrDestroyThisBall();
            }
            ColliedObjectTag = String.Empty;
        }
    }
}
