using System.Collections.Generic;
using Script.Commands.Input;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    #region Self Variables

    #region Private Variables

    private InputData _data;
    private bool _isAvailableForTouch, _isFirstTimeTouchTaken, _isTouching;
    private float _tapTime;

    #endregion


    #endregion

    private void Awake()
    {
        _data = GetInputData();
    }

    private InputData GetInputData()
    {
        return Resources.Load<CD_Input>("Data/InputData").Data;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        CoreGameSignals.Instance.onResetActiveLevel += OnReset;
        InputSignals.Instance.onEnableInput += OnEnableInput;
        InputSignals.Instance.onDisableInput += OnDisableInput;
    }

    private void OnReset()
    {
        _isAvailableForTouch = false;
        _isFirstTimeTouchTaken = false;
        _isTouching = false;
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
        CoreGameSignals.Instance.onResetActiveLevel -= OnReset;
        InputSignals.Instance.onEnableInput -= OnEnableInput;
        InputSignals.Instance.onDisableInput -= OnDisableInput;
    }
    
        private void Update()
    {
        if (!_isAvailableForTouch) return;
        
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            _isTouching = true;
            _tapTime = Time.time;
            
            InputSignals.Instance.onInputTaken?.Invoke();
            Debug.Log("Executed ---onInputTaken---");
            
            if (!_isFirstTimeTouchTaken)
            {
                _isFirstTimeTouchTaken = true;
                InputSignals.Instance.onFirstTimeTouchTaken?.Invoke();
                Debug.Log("Executed ---onFirstTimeTouchTaken---");
            }
        }
        
        if (Input.GetMouseButtonUp(0) && !IsPointerOverUIElement())
        {
            _isTouching = false;
            InputSignals.Instance.onInputReleased?.Invoke();
            Debug.Log("Executed ---onInputReleased---");
            
            float tapDuration = Time.time - _tapTime;
            if (tapDuration > _data.maxValidInputTime)
            {
                Debug.Log("Tap too long! Ignoring interaction.");
                return;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f)) 
            {
                var interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    // 1C. Create and execute a command
                    var clickCommand = new OnInteractableClickCommand(interactable);
                    clickCommand.Execute();
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
