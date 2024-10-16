using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTimeController : MonoBehaviour
{
    public float lastingTime = 0.5f;
    private void Start()
    {
        StartCoroutine(TimerSet());
    }

    IEnumerator TimerSet()
    {
        yield return new WaitForSeconds(lastingTime);
        Destroy(this.gameObject);
    }
}
