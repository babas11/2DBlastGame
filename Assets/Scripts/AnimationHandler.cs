using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class AnimationHandler : MonoBehaviour {
    
    public static void ScaleUpAndDownAndCreateNewObject<TCreate,TGroup>( TCreate newType,IEnumerable<TGroup> objectsTo,float scale,float duration,
        Vector3? position = null,
        Quaternion? rotation = null,
        Transform parent = null) 
    where TCreate: MonoBehaviour
    where TGroup : Component
    {
        
        
    }



}