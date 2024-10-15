using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBall : BallScriptBase
{
    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        _rigidbody             = this.GetComponent<Rigidbody>();
        _rigidbody.useGravity  = false;
        _rigidbody.isKinematic = true;
    }

    public override void ReleaseMe(Vector3 direction, float gain)
    {
        if (this.transform.parent != null)
        {
            Debug.Log("Released!");
            //this.transform.rotation = this.transform.parent.rotation;
            this.transform.parent  = null;
            _rigidbody.useGravity  = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(direction * gain);
        }
    }
}
