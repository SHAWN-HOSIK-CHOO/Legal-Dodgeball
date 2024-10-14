using UnityEngine;

namespace Attack
{
    public class WaterBall : BallScriptBase
    {
        public GameObject pfWater;
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);

            if (ColliedObjectTag == "Floor")
                Instantiate(pfWater, transform.position, pfWater.transform.rotation);
            
            Destroy(this.gameObject);
        }
    }
}
