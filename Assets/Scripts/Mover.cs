using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;




public class Mover : MonoBehaviour
{
    private Vector3 from;
    private Vector3 to;


    private float howFar;

    
    public bool idle { get; set; } = true;


    /// <summary>
    /// 
    /// </summary>
    
    
     private bool _idle = true;

    /// <summary>
    /// Indicates whether the object is currently idle (not animating).
    /// </summary>
    public bool IsIdle
    {
        get => _idle;
        protected set => _idle = value;
    } 
   

    public virtual void ScaleTo(Vector3 targetScale, float duration, Ease ease = Ease.Linear, Action onComplete = null)
    {
        Tween tween = transform.DOScale(targetScale, duration).SetEase(ease);
        tween.SetId("Non-Moving Animation");
        ExecuteTween(tween, onComplete);
    }


    /// <summary>
    /// Rotates the object to the target rotation over the specified duration with the given easing.
    /// </summary>
    public virtual void RotateTo(Vector3 targetRotation, float duration, Ease ease = Ease.Linear, Action onComplete = null)
    {
        Tween tween = transform.DORotate(targetRotation, duration).SetEase(ease);
        tween.SetId("Non-Moving Animation");
        ExecuteTween(tween, onComplete);
    }

    /// <summary>
    /// Moves the object to the target position over the specified duration with the given easing.
    /// </summary>
    public virtual void MoveTo(Vector3 targetPosition, float duration, Ease ease = Ease.Linear, Action onComplete = null)
    {
        Tween tween = transform.DOMove(targetPosition, duration).SetEase(ease);
        tween.SetId("Moving Animation");
        ExecuteTween(transform.DOMove(targetPosition, duration).SetEase(ease),onComplete);
    }

    public IEnumerator ScaleUpAndDownAsync(float targetMaxScale,float targetLowestScale, float duration,bool idleOnStart, Ease ease = Ease.Linear, Action onComplete = null)
    {
        Tween tween1 = transform.DOScale(targetMaxScale, duration).SetEase(ease);
        tween1.SetId("Non-Moving Animation");
        Tween tween2 = transform.DOScale(targetLowestScale, duration).SetEase(ease);
        tween2.SetId("Non-Moving Animation");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tween1);
        sequence.Append(tween2);
        ExecuteSequence(sequence,idleOnStart,onComplete);
        
        yield return sequence.WaitForCompletion();
       ;
    }

   
     public void ScaleUpAndDown(float targetMaxScale,float targetLowestScale, float duration,bool idleOnStart, Action onComplete = null)
    {
        Tween tween1 = transform.DOScale(targetMaxScale, duration);
        tween1.SetId("Non-Moving Animation");
        Tween tween2 = transform.DOScale(targetLowestScale, duration);
        tween2.SetId("Non-Moving Animation");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tween1);
        sequence.Append(tween2);
        ExecuteSequence(sequence,idleOnStart,onComplete);
    }

     public IEnumerator ScaleUpAndDownAsync<T>(IEnumerable<T> itemsToAnimate,float targetMaxScale,float targetLowestScale, float duration,bool idleOnStart, Action onComplete = null) where T: Mover
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var item in itemsToAnimate)
        {
            Tween tween1 = item.transform.DOScale(targetMaxScale, duration);
            tween1.SetId("Non-Moving Animation");
            
        }

        foreach (var item in itemsToAnimate)
        {
            Tween tween2 = item.transform.DOScale(targetLowestScale, duration);
            sequence.Join(tween2);
        }

        ExecuteSequence(sequence,idleOnStart,onComplete);

        yield return sequence.WaitForCompletion();
        
    }
    protected void ExecuteSequence(Sequence sequence,bool idleOnStart, Action onComplete = null)
    {
        if (sequence == null) return;

        sequence.OnStart(() =>
        {
            IsIdle = idleOnStart;
            idle = idleOnStart;

        });


        sequence.OnComplete(() =>
        {
            IsIdle = true;
            idle = true;
            onComplete?.Invoke();

        });
    }


    /// <summary>
    /// Executes the tween with standardized callbacks.
    /// </summary>
    protected void ExecuteTween(Tween tween, Action onComplete = null)
    {
        if (tween == null) return;

        IsIdle = false; 

        tween.OnComplete(() =>
        {
            IsIdle = true;
            onComplete?.Invoke();
        });
    }

    

    /// <summary>
    /// Kills all active tweens on this GameObject when destroyed to prevent memory leaks.
    /// </summary>
    protected virtual void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    /// <summary>
    /// Kills all active tweens on this GameObject when disabled to prevent memory leaks.
    /// </summary>
    protected virtual void  OnDisable() {
        DOTween.Kill(transform);

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

    public IEnumerator ParticleDissolution(float fragmentMoveDistance, float explosionDuration, Vector3 startPosition, Particle particle = null)
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
        if (TryGetComponent(out Particle particle))
        {
            transform.parent = GameObject.FindAnyObjectByType<ParticlePool>().transform;
        }
    }

    private float EasingOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t); // A quick, smooth easing function
    }

}
