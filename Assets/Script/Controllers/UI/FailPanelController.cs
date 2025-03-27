using System;
using DG.Tweening;
using Script.Managers;
using Script.Signals;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controllers.UI
{
    public class FailPanelController : MonoBehaviour
    {
        #region Self Variables

        #region Private Variables

        private UIManager _manager;
        private Vector2 _targetBasePosition;
        private Vector2 _targetRibbonPosition;

        #endregion

        #region Serialized Variables
        
        [SerializeField] private Image popUpRibbon;
        [SerializeField] private Image popUpBase;
        [SerializeField] Button restartButton;
        [SerializeField] private float offScreenValue;
        [SerializeField] private float animationDuration;
        [SerializeField] private float ScaleDuration;
        [SerializeField] private float ScaleFactor;
        
        #endregion

        #endregion

        private void Awake()
        {
            GetReferences();
            AnimateFailPanel();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UISignals.Instance.onRestartPressed += ButtonPressed;
        }

        private void ButtonPressed()
        {
            restartButton.interactable = false;
            RectTransform rectTransform = restartButton.GetComponent<RectTransform>();
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScale(Vector3.one * ScaleFactor, ScaleDuration).SetEase(Ease.OutBack));
            seq.Append(rectTransform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack));
            seq.AppendCallback(() => _manager.RestartLevel());
            seq.Play(); 
        }

        private void GetReferences()
        {
            _manager = FindObjectOfType<UIManager>();
        }

        private void AnimateFailPanel()
        {
            _targetBasePosition = popUpBase.rectTransform.anchoredPosition;
            _targetRibbonPosition = popUpRibbon.rectTransform.anchoredPosition;
            popUpBase.rectTransform.anchoredPosition = new Vector2(_targetBasePosition.x, _targetBasePosition.y - offScreenValue);
            popUpRibbon.rectTransform.anchoredPosition = new Vector2(_targetRibbonPosition.x, _targetRibbonPosition.y +offScreenValue);
            Vector2 popUpRibbonFinalPos = new Vector2(_targetRibbonPosition.x, _targetRibbonPosition.y + offScreenValue);
            Sequence seq = DOTween.Sequence();
            seq.Append(popUpRibbon.rectTransform.DOAnchorPos(_targetRibbonPosition, animationDuration).SetEase(Ease.OutSine));
            seq.AppendInterval(1.5f);
            seq.Append(popUpRibbon.rectTransform.DOAnchorPos(popUpRibbonFinalPos, animationDuration/2).SetEase(Ease.InSine));
            seq.Append(popUpBase.rectTransform.DOAnchorPos(_targetBasePosition, animationDuration).SetEase(Ease.OutSine));
            seq.Play();
        }
        
        private void OnDisable()
        {
            UISignals.Instance.onRestartPressed -= ButtonPressed;
        }

        
    }
}