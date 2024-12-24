using UnityEngine;

/// <summary>
/// Represents the base class for all interactable objects in the game.
/// Provides fundamental properties and methods for interaction, state management, and grid integration.
/// </summary>
/// <seealso cref="Interactable"/>
/// <seealso cref="InteractableGridSystem"/>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : Mover
{
  

    /// <summary>
    /// Determines if the interactable object can fall. Can be overridden by derived classes.
    /// </summary>
    public virtual bool CanFall => true;

    /// <summary>
    /// Reference to the SpriteRenderer component for visual representation.
    /// </summary>
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// The type of this interactable object, defining its behavior and interactions.
    /// </summary>
    [SerializeField]
    private InteractableType type;

    /// <summary>
    /// Reference to the UI controller, responsible for managing UI elements related to interactions.
    /// </summary>
    protected UI uiController;


    /// <summary>
    /// The position of this interactable object within the grid system.
    /// </summary>
    [SerializeField]
    public Vector2Int MatrixPosition;

    /// <summary>
    /// Gets the <see cref="InteractableType"/> of this interactable object.
    /// </summary>
    public InteractableType Type { get => type; }

    /// <summary>
    /// Reference to the grid system managing the placement and state of interactable objects.
    /// </summary>
    protected InteractableGridSystem interactableGridSystem { get; private set; }

    void Start()
    {
        uiController = GameObject.FindObjectOfType<UI>();
        // Get the SpriteRenderer component attached to this GameObject.
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing.");
        }

        // Attempt to find and assign the InteractableGridSystem in the scene.
        interactableGridSystem = FindObjectOfType<InteractableGridSystem>();

        if (interactableGridSystem == null)
        {
            Debug.LogError("InteractableGridSystem not found in the scene.");
        }
    }

    /// <summary>
    /// Handles mouse click events on the interactable object.
    /// Decreases the player's available moves via the UI controller.
    /// Can be overridden by derived classes to implement additional behaviors.
    /// </summary>
    protected virtual void OnMouseDown()
    {
        uiController.DecreaseMoves();
    }

    /// <summary>
    /// Returns a string representation of the interactable object.
    /// Overrides the base <see cref="ToString"/> method to provide type information.
    /// </summary>
    /// <returns>A string representing the interactable type.</returns>
    public override string ToString()
    {
        return this.Type.RawValue();
    }

    /// <summary>
    /// Resets the interactable object's state to its default settings.
    /// This method should be implemented to clear or reset any transient states or properties.
    /// </summary>
    public void ResetInteractable()
    {

        //TODO: Organize to reset all settings of the Interactable
    }

}
