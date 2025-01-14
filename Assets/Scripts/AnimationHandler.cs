using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public static class AnimationHandler {
    
   public static IEnumerator ScaleUpAndDownAsync<T>(
        IEnumerable<T> itemsToAnimate,
        float targetMaxScale,
        float targetLowestScale,
        float duration,
        Action onComplete = null
    ) where T : Mover
    {
        // 1) Mark *all* items as busy (not idle)
        foreach (var item in itemsToAnimate)
        {
            item.idle = false;
        }

        // 2) Build a DOTween sequence
        Sequence sequence = DOTween.Sequence();

        // 2a) Scale all items up in parallel
        foreach (var item in itemsToAnimate)
        {
            sequence.Join(item.transform.DOScale(targetMaxScale, duration));
        }
        sequence.AppendInterval(0f);
        // 2b) Then scale all items down in parallel
        foreach (var item in itemsToAnimate)
        {
            sequence.Join(item.transform.DOScale(targetLowestScale, duration));
        }

        sequence.Play();
        // 3) When the entire sequence completes, mark them idle again & invoke callback
        sequence.OnComplete(() =>
        {
            foreach (var item in itemsToAnimate)
            {
                item.idle = true;
            }
            onComplete?.Invoke();
        });

        // 4) Wait until the sequence completes
        yield return sequence.WaitForCompletion();
    }
    public static void ScaleUpAndDown<T>(
        IEnumerable<T> itemsToAnimate,
        float targetMaxScale,
        float targetLowestScale,
        float duration,
        Action onComplete = null
    ) where T : Mover
    {
        // 1) Mark *all* items as busy (not idle)
        foreach (var item in itemsToAnimate)
        {
            item.idle = false;
        }

        // 2) Build a DOTween sequence
        Sequence sequence = DOTween.Sequence();

        // 2a) Scale all items up in parallel
        foreach (var item in itemsToAnimate)
        {
            sequence.Join(item.transform.DOScale(targetMaxScale, duration));
        }
        sequence.AppendInterval(0f);
        // 2b) Then scale all items down in parallel
        foreach (var item in itemsToAnimate)
        {
            sequence.Join(item.transform.DOScale(targetLowestScale, duration));
        }

        sequence.Play();
        // 3) When the entire sequence completes, mark them idle again & invoke callback
        sequence.OnComplete(() =>
        {
            foreach (var item in itemsToAnimate)
            {
                item.idle = true;
            }
            onComplete?.Invoke();
        });
        
    }
    
    public static void ScaleUpAndDown<T>(
        T itemToAnimate,
        float targetMaxScale,
        float targetLowestScale,
        float duration,
        Action onComplete = null
    ) where T : Mover
    {
        // 1) Mark *all* items as busy (not idle)
        itemToAnimate.idle = false;
        // 2) Build a DOTween sequence
        Sequence sequence = DOTween.Sequence();
        // 2a) Scale  item up 
        sequence.Append(itemToAnimate.transform.DOScale(targetMaxScale, duration));
        // 2b) Then scale  item down 
        sequence.Append(itemToAnimate.transform.DOScale(targetLowestScale, duration));

        sequence.Play();
        sequence.OnComplete(() =>
        {
            itemToAnimate.idle = true;
            onComplete?.Invoke();
        });
        
    }

    public static void FallAndRotate<T>(T[] itemToFall,Vector3[] endPosition,float jumpPower,float duration,int numJumps = 1, Action onJumpComplete = null) 
    where T : Mover
    {
        Sequence sequence = DOTween.Sequence();
        if (itemToFall.Count() != endPosition.Count())
        {
            Debug.Log("Count of End Positions should be same as ttotal number of items");
        }
        
        for(int i = 0; i < itemToFall.Count(); i++)
        {
            Tween fallTween = itemToFall[i].transform.DOJump(endPosition[i], jumpPower, numJumps, duration);
            Tween rotateTween = itemToFall[i].transform.DORotate(new Vector3(0, 0, 360), .3f, RotateMode.FastBeyond360).SetLoops(7, LoopType.Incremental);
            sequence.Join(fallTween);
            sequence.Join(rotateTween);
        }
        
        sequence.OnComplete(() =>
        {
            onJumpComplete?.Invoke();
        });
    }
    
    public static IEnumerator ScaleUpAndRotate<T>(
        T itemToAnimate,
        float targetMaxScale,
        float duration,
        Vector3 endPosition,
        Action onComplete = null
    ) where T : Mover
    {
        // 1) Mark *all* items as busy (not idle)
        itemToAnimate.idle = false;
        // 2) Build a DOTween sequence
        Sequence sequence = DOTween.Sequence();
        // 2a) Scale  item up 
        sequence.Join(itemToAnimate.transform.DOScale(targetMaxScale, duration)
                                              .SetEase(Ease.OutElastic));
        // 2b) Then scale  item down 
        sequence.Join(itemToAnimate.transform.DORotate(endPosition, duration,RotateMode.FastBeyond360).SetEase(Ease.OutElastic));
        sequence.Play();
        sequence.OnComplete(() =>
        {
            itemToAnimate.idle = true;
            onComplete?.Invoke();
        });
        yield return sequence.WaitForCompletion();
    }

    public static IEnumerator MegaTntMergeIntoOne(
        List<Tnt> allTnts,
        Tnt mainTnt,                    
        float duration,
        float partialTimeScale,
        Action onComplete = null,
        Action onPartialTime = null)
    {
        foreach (var tnt in allTnts)
        {
            tnt.idle = false;
        }

        // 2) Build sequence
        Sequence sequence = DOTween.Sequence();

        // 2a) Move all Tnts to the center in parallel
        foreach (var tnt in allTnts)
        {
            sequence.Join(
                tnt.transform.DOMove(mainTnt.transform.position, duration * 0.4f).SetEase(Ease.OutSine)
            );
        }

        // 2b) Once they've converged, remove/disable all but one
        sequence.AppendCallback(() =>
        {
            // Hide or remove all Tnts except 'mainTnt'
            foreach (var tnt in allTnts)
            {
                if (tnt != mainTnt)
                {
                    // Option A: disable or hide
                    tnt.DisableRenderer();
                }
            }
        });

        // 2c) Now continue animating just 'mainTnt'
        // Example: scale it up or rotate, etc.
        sequence.Append(
            mainTnt.transform.DOScale(2.0f, duration * 0.7f).SetEase(Ease.OutElastic)
        );
        sequence.Join(
            mainTnt.transform.DORotate(new Vector3(0, 0, 360), duration * 0.7f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutElastic)
        );
        
        
        float colorFlashDuration = 0.5f;

            SpriteRenderer sr = mainTnt.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color originalColor = sr.color;
                Color flashColor = Color.red; // pick a color you like

                // color up
                sequence.Join(
                    sr.DOColor(flashColor, duration * 0.35f)
                        .SetEase(Ease.OutQuad).OnComplete((() =>
                        {
                            sr.DOColor(originalColor, duration * 0.35f)
                                .SetEase(Ease.InQuad);
                        }))
                );
             
            }
            
            float OnPartialTime = duration * partialTimeScale;
            sequence.InsertCallback(OnPartialTime,() =>
            {
                onPartialTime?.Invoke();
            });

        // 3) OnComplete => set the main Tnt idle = true, invoke callback if needed
        sequence.OnComplete(() =>
        {
            mainTnt.idle = true;
            onComplete?.Invoke();
        });

        // 4) Play and yield
        sequence.Play();
        yield return sequence.WaitForCompletion();
    }

    /*public static IEnumerator PlayTnt(TntType tntType, Action onComplete = null)
    {
        switch (tntType)
        {
            case TntType.Regular:
                //yield return ScaleUpAndRotate();
                break;
            case TntType.Mega:
                //yield return MegaTnt();
                break;
        }
    }*/
}

public struct AnimationData
{
    public readonly Action OnComplete;
    public readonly IEnumerable<Tnt> ItemsToAnimate;
    public readonly Vector3 CenterTnt;
    public readonly Vector3 EndPosition;
    public readonly float SingleScaleUp;
    public readonly float AllFinalScale;
    public readonly float Duration;

    public AnimationData(Action onComplete, IEnumerable<Tnt> itemsToAnimate, Vector3 centerTnt, Vector3 endPosition, float singleScaleUp, float allFinalScale, float duration)
    {
        OnComplete = onComplete;
        this.ItemsToAnimate = itemsToAnimate;
        this.CenterTnt = centerTnt;
        this.EndPosition = endPosition;
        this.SingleScaleUp = singleScaleUp;
        this.AllFinalScale = allFinalScale;
        this.Duration = duration;
    }


}