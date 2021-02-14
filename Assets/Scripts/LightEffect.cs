using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour
{
    [Space]
    [SerializeField] List<SpriteRenderer> lights;
    [SerializeField] List<SpriteRenderer> glitch;
    bool limit = false;
    float alpha = 1f;
    float initialValue = 1f;

    void Update()
    {
        if (limit)
        {
            alpha -= Time.deltaTime;
            if (alpha <= initialValue / 2)
                limit = false;
        }
        else
        {
            alpha += Time.deltaTime;
            if (alpha >= initialValue)
                limit = true;
        }

        foreach (SpriteRenderer sr in lights)
        {
            if (sr != null)
            {
                var alphaValue = sr.color;
                alphaValue.a = alpha;
                sr.color = alphaValue;
            }
        }
    }
}
