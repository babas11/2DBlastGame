using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    // Event triggered when an interactable is selected
    public static event Action<Interactable> OnInteractableSelected;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleUserInput();
    }

    private void HandleUserInput()
    {
        // Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastAndSelectInteractable(Input.mousePosition);
        }

        // Handle touch input (optional)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastAndSelectInteractable(Input.GetTouch(0).position);
        }
    }

    private void RaycastAndSelectInteractable(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out Interactable interactable) && interactable.idle)
            {
                print(interactable.Type);
                OnInteractableSelected?.Invoke(interactable);
            }
            print(interactable.idle);
        }
    }
}