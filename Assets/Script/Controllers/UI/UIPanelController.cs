using System;
using System.Collections.Generic;
using Script.Enums;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Controllers.UI
{
    public class UIPanelController : MonoBehaviour
    {
        #region Variables
        
        #region Serialized Fields
        [SerializeField] private List<Transform> layers= new List<Transform>();

        #endregion

        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreUISignals.Instance.onClosePanel += OnClosePanel;
            CoreUISignals.Instance.onOpenPanel += OnOpenPanel;
            CoreUISignals.Instance.onCloseAllPanel += OnCloseAllPanels;
        }
        
        [Button("Open Panel_Test")]
        private void OnOpenPanel(UIPanelTypes panelType, int value)
        {
            OnClosePanel(value);
            Instantiate(Resources.Load<GameObject>($"Screens/{panelType}Panel"),layers[value]);
        }

        [Button("Close Panel_Test")]
        private void OnClosePanel(int value)
        {
            if (layers[value].childCount <= 0) return;
            
#if UNITY_EDITOR
                DestroyImmediate(layers[value].GetChild(0).gameObject);
#else
                Destroy(layers[value].GetChild(0).gameObject;
#endif
        }
        
        [Button("Close All Panel_Test")]
        private void OnCloseAllPanels()
        {
            foreach (var layer in layers)
            {
                if (layer.childCount <= 0) continue;
#if UNITY_EDITOR
                DestroyImmediate(layer.GetChild(0).gameObject);
#else
                Destroy(layer.GetChild(0).gameObject);
#endif
            }
        }
        
        

        private void OnDisable()
        {
            //CoreUISignals.Instance.onClosePanel -= OnClosePanel;
            CoreUISignals.Instance.onOpenPanel -= OnOpenPanel;
            CoreUISignals.Instance.onCloseAllPanel -= OnCloseAllPanels;
        }
    }
}