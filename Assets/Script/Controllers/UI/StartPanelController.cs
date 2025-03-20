using System;
using DG.Tweening;
using Script.Managers;
using Script.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Script.Controllers.UI
{
    public class StartPanelController : MonoBehaviour
    {
        #region Variables

        #region Serialized Variables

        [SerializeField] private TMP_Text levelButtonText;
        [SerializeField] private Button button;
        [SerializeField] private float ScaleDuration;
        [SerializeField] private float ScaleFactor;

        #endregion

        #region Private Variables

        private UIManager _manager;

        #endregion

        #endregion

        public void Init()
        {
            levelButtonText.text = $"Level {CoreGameSignals.Instance.OnGetLevelIndex?.Invoke().ToString()}";
        }
        private void GetReferences()
        {
            _manager = FindObjectOfType<UIManager>();
        }

        private void Awake()
        {
            Init();
            GetReferences();
        }
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UISignals.Instance.onStartLevelButtonPressed += OnStartLevelButtonPressed;
        }

        private void OnStartLevelButtonPressed()
        {
            button.interactable = false;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScale(Vector3.one * ScaleFactor, ScaleDuration).SetEase(Ease.OutBack));
            seq.Append(rectTransform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack));
            seq.AppendCallback(() => CoreGameSignals.Instance.onLevelPlay?.Invoke());
            seq.Play();    
        }

        private void OnDisable()
        {
            UnsubscribEvents();
        }

        private void UnsubscribEvents()
        {
            UISignals.Instance.onStartLevelButtonPressed -= OnStartLevelButtonPressed;

        }
    }
}