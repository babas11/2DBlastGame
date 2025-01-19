using System;
using UnityEngine;

public class BlastManager : MonoBehaviour
{
    [SerializeField]
    private InteractablePool interactablePool;
    public InteractablePool MyInteractablePool {get { return interactablePool; }}
    
    [SerializeField]
    private  ParticlePool particlePool;
    public ParticlePool MyParticlePool  {get { return particlePool; }}
    
    [SerializeField]
    private  GridViewController gridViewController;
    public GridViewController MyGridViewController  {get { return gridViewController; }}
    
    [SerializeField]
    private  INteractableGridSystem interactableGridSystem;
    public INteractableGridSystem MyInteractableGridSystem  {get { return interactableGridSystem; }}
    
    [SerializeField]
    private  ObstacleDamageController obstacleDamageController;
    public ObstacleDamageController MyObstacleDamageController  {get { return obstacleDamageController; }}
    
    public static event Action<Cube> OnRegularBlast; 
    public static event Action<Tnt> ExplosionBlast;

    private void OnEnable()
    {
        InputReader.OnInteractableSelected += HandleInteractableSelection;
    }

    private void OnDisable()
    {
        InputReader.OnInteractableSelected -= HandleInteractableSelection;
    }

    private void Start()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        gridViewController = GetComponent<GridViewController>();
    }


    private void HandleInteractableSelection(Interactable interactable)
        {
            if (interactable == null)
            {
                Debug.LogError("Selected interactable is null.");
                return;
            }
            if (interactable == null)
            {
                Debug.LogError("Interactable is null");
                return;
            }

            switch (interactable)
            {
                case Cube cube:
                    OnRegularBlast?.Invoke(cube);
                    break;
                case Tnt tnt:
                    ExplosionBlast?.Invoke(tnt);
                    break;
                default:
                    Debug.LogWarning($"Unhandled interactable type: {interactable.GetType()}");
                    break;
            }
        }
    


        // Static methods to trigger events (optional, used by Interactables)
        public static void TriggerRegularBlast(Cube cube)
        {
            OnRegularBlast?.Invoke(cube);
        }

        public static void TriggerExplosionBlast(Tnt tnt)
        {
            ExplosionBlast?.Invoke(tnt);
        }
    }
