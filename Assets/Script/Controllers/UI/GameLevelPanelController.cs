using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Signals;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.Controllers.UI
{
    public class GameLevelPanelController : MonoBehaviour
    {
        #region Self Variables
        
        [SerializeField] private List<Image> stageImages = new List<Image>();
        [SerializeField] private List<TextMeshProUGUI> levelTexts = new List<TextMeshProUGUI>();

        #endregion

        private void OnEnable()
        {
            UISignals.Instance.onSetLevelValue += OnSetLevelValue;
            //UISignals.Instance.onSetStageColor += OnSetStageColor;
        }

        private void OnSetLevelValue(byte levelValue)
        {
            var additionalValue = ++levelValue;
            
            levelTexts[0].text = additionalValue.ToString();
            additionalValue++;
            levelTexts[1].text = additionalValue.ToString();

        }

        [Button("Set Stage Color")]
        private void OnSetStageColor(byte stageValue)
        {
            stageImages[stageValue].DOColor(new Color(0.996f,0.419f,0.41f,1f),0.5f);
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void UnSubscribe()
        {
            UISignals.Instance.onSetLevelValue -= OnSetLevelValue;
            //UISignals.Instance.onSetStageColor += OnSetStageColor;
        }
    }
}