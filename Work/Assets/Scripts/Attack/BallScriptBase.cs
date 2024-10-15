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
            ColliedObjectTag       = String.Empty;
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
        
    }
}
