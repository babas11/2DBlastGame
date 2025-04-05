using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Interfaces;
using Script.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Managers
{
    public class InputManager : MonoBehaviour
    {
        #region Self Variables

        #region Private Variables

        private InputData _data;
        private bool _isAvailableForTouch, _isTouching;
        private float _tapTime;

        #endregion


        #endregion

        private void Awake()
        {
            _data = GetInputData();
        }

        private InputData GetInputData()
        {
            return Resources.Load<CD_Input>("Data/Input/InputData").Data;
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (InputSignals.Instance != null)
            {
                InputSignals.Instance.onEnableInput += OnEnableInput;
                InputSignals.Instance.onDisableInput += OnDisableInput;
            }
        }
        
        private void OnEnableInput()
        {
            _isAvailableForTouch = true;
        }
        private void OnDisableInput()
        {
            _isAvailableForTouch = false;
        }
        private void UnSubscribeEvents()
        {
            if (InputSignals.Instance != null)
            {
                InputSignals.Instance.onEnableInput -= OnEnableInput;
                InputSignals.Instance.onDisableInput -= OnDisableInput;
            }
            
        }
    
        private void Update()
        {
            if (!_isAvailableForTouch) return;
        
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                _isTouching = true;
                _tapTime = Time.time;
                InputSignals.Instance.onInputTaken?.Invoke();
            }
        
            if (Input.GetMouseButtonUp(0) && !IsPointerOverUIElement())
            {
                _isTouching = false;
                InputSignals.Instance.onInputReleased?.Invoke();
            
                float tapDuration = Time.time - _tapTime;
                if (tapDuration > _data.maxValidInputTime)
                {
                    return;
                }
            
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInfo = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
                if (hitInfo.collider != null)
                {
                    IGridElement touchedElement;
                    if (hitInfo.collider.CompareTag("Cube"))
                    {
                        touchedElement = hitInfo.transform.GetComponentInParent<CubeManager>();
                        InputSignals.Instance.onGridTouch?.Invoke(touchedElement);
                    
                    }
                    else if (hitInfo.collider.CompareTag("Obstacle"))
                    {
                        touchedElement = hitInfo.transform.GetComponentInParent<ObstacleManager>();
                        InputSignals.Instance.onGridTouch?.Invoke(touchedElement);
                    }
                }
            }
        }
        void OnDisable()
        {
            UnSubscribeEvents();
        }
        private bool IsPointerOverUIElement()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

    }
}
