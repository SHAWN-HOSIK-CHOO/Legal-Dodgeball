using System;
using Attack;
using UnityEngine;

namespace DebugScripts
{
    public class TestEnemyShoot : MonoBehaviour
    {
        public GameObject pfThrowableBall;
        public GameObject target;
        public Transform  ballShootPosition;

        private GameObject _ball;

        private void Update()
        {
            Vector3 targetPosition = target.transform.position;
            Vector3 throwVector    = ( targetPosition - this.transform.position ).normalized;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                GameObject go = Instantiate(pfThrowableBall, ballShootPosition);
                _ball = go;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                _ball.GetComponent<BallScriptBase>().ReleaseMe(throwVector, 350f);
            }
        }
    }
}
