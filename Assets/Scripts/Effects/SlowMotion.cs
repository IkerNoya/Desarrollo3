using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public IEnumerator ActivateSlowMotion(float time, float intensity)
    {
        Time.timeScale = intensity;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
        yield break;
    }
}
