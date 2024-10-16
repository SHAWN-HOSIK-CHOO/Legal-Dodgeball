using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts.Network;

namespace Attack
{
    public class ExplodeBall : BallScriptBase
    {
        public float explodeRadius  = 3f;
        public float explosionForce = 100f;
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);

            if(GameManager.instance.IsServer)
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, explodeRadius);
                foreach (Collider nearbyCollider in colliders)
                {
                    Rigidbody rb = nearbyCollider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(explosionForce, transform.position, explodeRadius);
                    }

                    if (nearbyCollider.gameObject.CompareTag("Player"))
                    {
                        GManager.Instance.MinusPlayerHP(nearbyCollider.gameObject,20f);
                    }
                }
            }
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
                GManager.Instance.MinusPlayerHP(other.gameObject,50f);
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
