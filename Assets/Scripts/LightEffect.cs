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
    float timer;
    float timeLimit;
    void Start()
    {
        timeLimit = Random.Range(10,25);
    }
    void Update()
    {
        AmbianceFade();
        GlitchEffect();
    }
    void AmbianceFade()
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
    void GlitchEffect()
    {
        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            StartCoroutine(Glitch(0.25f));
        }
    }
    IEnumerator Glitch(float duration)
    {
        foreach(SpriteRenderer sr in glitch) 
        {
            var alpha = sr.color;
            alpha.a = 0.5f;
            sr.color = alpha;
            yield return new WaitForSeconds(duration);
            alpha.a = 1;
            sr.color = alpha;
            yield return new WaitForSeconds(duration);
            alpha.a = 0.5f;
            sr.color = alpha;
            yield return new WaitForSeconds(duration);
            alpha.a = 1;
            sr.color = alpha;
        }
        timeLimit = Random.Range(10,25);
        timer = 0;
        yield return null;
    }
}
