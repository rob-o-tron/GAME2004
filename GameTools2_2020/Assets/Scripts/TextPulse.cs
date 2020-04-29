using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPulse : MonoBehaviour
{
    public CanvasGroup canvGroup;
    public AnimationCurve flashingCurve;
    private Coroutine flashCoroutine = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }

    private void OnEnable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(Flashing(0.5f));

    }

    IEnumerator Flashing(float period)
    {
        float startTime = Time.time;
        float endTime = startTime + period;
        canvGroup.alpha = 0.0f;
        yield return null;
        while (true)
        {
            float eval = Mathf.Repeat(Time.time-startTime,period);
            canvGroup.alpha = flashingCurve.Evaluate(eval);
            yield return null;
        }
    }
}
