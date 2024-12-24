using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;




public class Mover : MonoBehaviour
{
    private Vector3 from;
    private Vector3 to;


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
            transform.position = Vector3.Lerp(from, to, EasingElastic(howFar));
            yield return null;
        } while (howFar != 1);

        idle = true;
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

     public IEnumerator MoveToPositionWithJump(Vector3 targetPosition, float speed, float jumpHeight)
    {
        if (speed <= 0) { Debug.LogWarning("Speed must be a positive number"); }
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howFar = 0;
        idle = false;

        idle = false;
        do
        {
            howFar += Time.deltaTime * speed;
            if (howFar > 1)
            {
                howFar = 1;
            }
            transform.position = Vector3.Lerp(from, to, howFar);
            yield return null;
        } while (howFar != 1);

        // Add a small jump after reaching the position
        float jumpDuration = 0.3f;
        float jumpTime = 0;
        while (jumpTime < jumpDuration)
        {
            jumpTime += Time.deltaTime;
            float yOffset = Mathf.Sin((jumpTime / jumpDuration) * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(transform.position.x, to.y + yOffset, transform.position.z);
            yield return null;
        }

        idle = true;
    }

    public IEnumerator ParticleDissolution(float fragmentMoveDistance, float explosionDuration, Vector3 startPosition, BlastParticle particle = null)
    {
        // Choose an initial upward direction with randomness to spread the particles
        Vector3 initialDirection = (Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.Range(1f, -1f), 0)).normalized;

        // Vary the distance each particle will travel slightly for randomness
        float randomFragmentMoveDistance = fragmentMoveDistance * Random.Range(0.8f, 1.2f);

        float upwardDuration = explosionDuration * 0.1f; // 30% of the duration spent going up
        float fallDuration = explosionDuration * 0.9f;   // 70% of the duration spent falling

        float elapsed = 0f;
        float rotationSpeed = Random.Range(250f, 300f); // Random rotation speed for each fragment
        float horizontalMovementDuringFall = Random.Range(-0.5f, 0.5f);


        // Upward phase
        while (elapsed < upwardDuration)
        {
            float t = elapsed / upwardDuration;

            // Move fragment upwards in the calculated direction
            Vector3 upwardPosition = startPosition + (initialDirection * randomFragmentMoveDistance) * (t / 6);
            transform.position = upwardPosition;

            // Rotate fragment
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset elapsed time for the fall portion
        elapsed = 0f;
        Vector3 currentPosition = transform.position;
        float verticalVelocity = 0f;  // Initial velocity for falling down

        // Falling phase with gravity
        while (elapsed < fallDuration)
        {
            // Apply gravity effect
            verticalVelocity += -25 * Time.deltaTime;  // Gravity pulling the particle down

            // Update the fragment's position to simulate falling
            currentPosition.y += verticalVelocity * Time.deltaTime;

            currentPosition.x += horizontalMovementDuringFall * Time.deltaTime;

            // Apply the new position
            transform.position = currentPosition;

            // Rotate fragment
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (particle != null)
        {
            particle.gameObject.SetActive(false);
        }
    }
    

    
    private float Easing(float howFar)
    {
        return howFar * howFar;
    }
    
    
    public IEnumerator CartoonishScaleToTarget(float speed, float overshootScale, float targetScale, bool givenIdle = false)
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
    idle = givenIdle;
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
    if(TryGetComponent(out BlastParticle particle))
    {
        transform.parent = GameObject.FindAnyObjectByType<BlastParticlePool>().transform;
    }
    idle = true;
}

private float EasingOutQuad(float t)
{
    return 1 - (1 - t) * (1 - t); // A quick, smooth easing function
}

}
