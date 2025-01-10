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
    
    

}