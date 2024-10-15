using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BallScriptBase : MonoBehaviour
{
    public abstract void ReleaseMe(Vector3 direction, float gain);
}
