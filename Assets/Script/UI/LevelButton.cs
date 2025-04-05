using System;
using DG.Tweening;
using Script.Extensions;
using Script.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UI
{
   public class LevelButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
   {
      #region Self Variables

      [SerializeField] private Button levelButton;
      [SerializeField] private TMP_Text levelButtonText;
      [SerializeField] private float ScaleDuration;
      [SerializeField] private float ScaleFactor;

      #endregion

      public void Init()
      {
         levelButtonText.text = GameData.CurrentLevel.ToString();
      }
      
      public void OnPointerDown(PointerEventData eventData)
      {
         transform.DOScale(Vector3.one * ScaleFactor, ScaleDuration).SetEase(Ease.OutBack);
      }
      
      public void OnPointerUp(PointerEventData eventData)
      {
         transform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack);
      }
   }
}
