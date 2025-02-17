using UnityEngine;

namespace Script.Commands.Input
{
    public class OnInteractableClickCommand 
    {
        private Interactable _interactable;

        public OnInteractableClickCommand(Interactable interactable)
        {
            _interactable = interactable;
        }

        public void Execute()
        {
            // Trigger Some Actions
            //BlastManager.OnInteractableSelected?.Invoke(Interactable)
            Debug.Log($"OnInteractableClickCommand, {_interactable.Type}");
        }
    }
}