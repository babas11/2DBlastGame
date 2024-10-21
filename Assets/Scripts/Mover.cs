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


    public IEnumerator ScaleToZero(float speed, float toScale)
    {
        if (speed <= 0)
        {
            Debug.LogWarning("Speed must be a positive number");
            yield break;
        }

        Vector3 fromScale = transform.localScale * 1.1f; // Slightly larger than current scale
        float howFar = 0f;
        idle = false;
        Vector3 toScaleVector = new Vector3(toScale, toScale, 1);
        // Set the initial scale to the larger scale
        transform.localScale = fromScale;

        while (howFar < 1f)
        {
            howFar += Time.deltaTime * speed;
            if (howFar > 1f)
            {
                howFar = 1f;
            }

            float easedValue = EasingElastic(howFar);
            transform.localScale = Vector3.LerpUnclamped(fromScale, toScaleVector, easedValue);
            if(transform.localScale.x < 0.1f)
            {
                break;
            }

            yield return null;
        }



    }
}
