using UnityEngine;

namespace Attack
{
    public class ExplodeBall : BallScriptBase
    {
        public float explodeRadius = 3f;
        public float explosionForce = 100f;
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, explodeRadius);
            foreach (Collider nearbyCollider in colliders)
            {
                Rigidbody rb = nearbyCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explodeRadius);
                }
            }

            Destroy(this.gameObject);
        }
    }
}
