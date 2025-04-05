using System;
using DG.Tweening;
using Script.Managers;
using Script.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.Controllers.UI
{
    public class WinPanelController : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private RectTransform starImage;
        [SerializeField] private ParticleSystem starParticle;
        [SerializeField] private RectTransform labelText;
        [SerializeField] private RectTransform dialogueText;
        [SerializeField] private Image backGroundImage;
        
        private UIManager _manager;
        private void OnEnable()
        {
            GetReference();
            AnimateWinPanel();
        }

        private void GetReference()
        {
            _manager = FindObjectOfType<UIManager>();
        }

        private void AnimateWinPanel()
        {
            labelText.localScale = Vector3.zero;
            starImage.localScale = Vector3.one * 2f;
            starImage.gameObject.SetActive(false);
            dialogueText.localScale = Vector3.zero;

            Sequence InitSequence = DOTween.Sequence();
            InitSequence.AppendInterval(1f);
            InitSequence.Append(backGroundImage.DOFade(0.93f, 1f));
            InitSequence.Append(labelText.DOScale(Vector3.one * 1.25f, .75f).SetEase(Ease.OutBack));
            InitSequence.Append(labelText.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack).SetDelay(.15f));
            InitSequence.AppendInterval(.75f);
            InitSequence.AppendCallback(() => starImage.gameObject.SetActive(true));
            InitSequence.Append(starImage.DOScale(Vector3.one * 1.5f, .5f).SetEase(Ease.OutBack).OnComplete(() => starParticle.Play()));
            InitSequence.Join(starImage.DOLocalRotate(Vector3.forward * 360, .5f, RotateMode.LocalAxisAdd));
            InitSequence.AppendInterval(1f);
            InitSequence.Append(dialogueText.DOScale(Vector3.one, .5f));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _manager.OnNextLevel();
        }
    }
    
    
    
}