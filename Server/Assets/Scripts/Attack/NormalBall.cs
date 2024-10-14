namespace Attack
{
    public class NormalBall : BallScriptBase
    {
        protected override void ExplodeOrDestroyThisBall()
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
