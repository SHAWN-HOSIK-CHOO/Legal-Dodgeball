using System;
using UnityEngine;

namespace Attack
{
    public abstract class BallScriptBase : MonoBehaviour
    {
        private   Rigidbody  _rigidbody;
        public    GameObject onDestroyEffect;
        protected string     ColliedObjectTag;
        private void Awake()
        {
            _rigidbody             = this.GetComponent<Rigidbody>();
            _rigidbody.useGravity  = false;
            _rigidbody.isKinematic = true;
<<<<<<< HEAD
            ColliedObjectTag = String.Empty;
=======
            ColliedObjectTag       = String.Empty;
>>>>>>> f3a07decf6d03d5c11693cbf73ce96f052c2a061
        }

        public void ReleaseMe(Vector3 direction, float gain)
        {
            if (this.transform.parent != null)
            {
                this.transform.parent  = null;
                _rigidbody.useGravity  = true;
                _rigidbody.isKinematic = false;
                _rigidbody.AddForce(direction * gain);
            }
        }

        protected abstract void ExplodeOrDestroyThisBall();
<<<<<<< HEAD
    
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Wall"))
            {
                ColliedObjectTag = other.gameObject.tag;
                ExplodeOrDestroyThisBall();
            }
            ColliedObjectTag = String.Empty;
        }
=======
        
>>>>>>> f3a07decf6d03d5c11693cbf73ce96f052c2a061
    }
}
