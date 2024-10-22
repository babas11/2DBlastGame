using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private Vector3 from;
    private Vector3 to;

    private float speed = .1f;

    private float howFar;

    public bool idle { get; set; }




    public IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {

        if (speed <= 0) { Debug.LogWarning("Speed must be a positive number"); }
        from = transform.position;
        to = targetPosition;
        howFar = 0;
        idle = false;
        do
        {
            howFar += Time.deltaTime * speed;
            if (howFar > 1)
            {
                howFar = 1;
            }
            transform.position = Vector3.LerpUnclamped(from, to, EasingElastic(howFar));
            yield return null;
        } while (howFar != 1);

        idle = true;
    }



    private float Easing(float howFar)
    {
        return howFar * howFar;
    }

    private float EasingElastic(float howFar)
    {
        float c5 = (2 * (float)Math.PI) / 4.5f;

        if (howFar == 0)
        {
            return 0;
        }
        else if (howFar == 1)
        {
            return 1;
        }
        else if (howFar < 0.5)
        {
            return -((float)Math.Pow(2, 20 * howFar - 10) * (float)Math.Sin((20 * howFar - 11.125) * c5)) / 2;
        }
        else
        {
            return ((float)Math.Pow(2, -20 * howFar + 10) * (float)Math.Sin((20 * howFar - 11.125) * c5)) / 2 + 1;
        }
    }

    public IEnumerator CartoonishScaleToTarget(float speed, float overshootScale, float targetScale)
{
    if (speed <= 0)
    {
        Debug.LogWarning("Speed must be a positive number");
        yield break;
    }

    if (targetScale < 0)
    {
        Debug.LogWarning("Target scale must be non-negative");
        yield break;
    }
    idle = false;
    Vector3 fromScale = transform.localScale;
    Vector3 toOvershootScale = fromScale * overshootScale; // Overshoot scale for the "pop" effect
    Vector3 toTargetScale = new Vector3(targetScale, targetScale, targetScale); // Target scale specified by user

    float howFar = 0f;

    // Phase 1: Quickly grow to an overshoot scale
    float growDuration = 0.2f / speed; // Adjust duration based on speed input
    while (howFar < 1f)
    {
        howFar += Time.deltaTime / growDuration;
        if (howFar > 1f)
        {
            howFar = 1f;
        }

        float easedValue = EasingOutQuad(howFar); // Using Quadratic easing for a quick but smooth effect
        transform.localScale = Vector3.LerpUnclamped(fromScale, toOvershootScale, easedValue);

        yield return null;
    }

    // Reset howFar for next phase
    howFar = 0f;

    // Phase 2: Smoothly shrink to target scale
    float shrinkDuration = 0.3f / speed; // Adjust duration based on speed input
    while (howFar < 1f)
    {
        howFar += Time.deltaTime / shrinkDuration;
        if (howFar > 1f)
        {
            howFar = 1f;
        }

        float easedValue = EasingOutQuad(howFar); // Using a simple easing for a smooth shrink down to target scale
        transform.localScale = Vector3.LerpUnclamped(toOvershootScale, toTargetScale, easedValue);

        yield return null;
    }

    // Ensure scale is exactly the target scale at the end
    transform.localScale = toTargetScale;
    idle = true;
}

private float EasingOutQuad(float t)
{
    return 1 - (1 - t) * (1 - t); // A quick, smooth quadratic easing function
}

}
