using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the damage application to obstacles during blast events.
/// </summary>
public class ObstacleDamageController: MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlastManager blastManager;

    private void Awake()
    {
        if (blastManager == null)
        {
            blastManager = FindObjectOfType<BlastManager>();
        }
    }

    /// <summary>
    /// Applies damage to a list of obstacles based on the BlastType.
    /// </summary>
    /// <param name="obstacles">List of obstacles to damage.</param>
    /// <param name="blastType">Type of blast causing the damage.</param>
    /// <returns>List of obstacles that were damaged.</returns>
    public List<IObstacle> ApplyDamage(List<IObstacle> obstacles, BlastType blastType)
    {
        List<IObstacle> damagedObstacles = new List<IObstacle>();
        foreach (IObstacle obstacle in obstacles)
        {
            bool isDead = obstacle.TakeDamage(1, blastType);

            if (isDead)
            {
                damagedObstacles.Add(obstacle);
            }
        }
        return damagedObstacles;
    }
    
    public List<IObstacle> DamageObstacles(List<Interactable> matchingInteractables, BlastType blastType)
    {
        List<IObstacle> nearObstacles = new List<IObstacle>();
        List<IObstacle> damagedObstacles = new List<IObstacle>();

        if (blastType == BlastType.Regular)
        {
            // 1) Find obstacles around matching interactables
            foreach (Interactable matchingInteractable in matchingInteractables)
            {
                if (blastManager.MyInteractableGridSystem.LookForInteractablesOnAxis(out HashSet<IObstacle> foundObstacles, matchingInteractable))
                {
                    nearObstacles.AddRange(foundObstacles);
                }
            }
            // 2) Remove duplicates
            nearObstacles = nearObstacles.Distinct().ToList();
        }
        else if (blastType == BlastType.Tnt)
        {
            nearObstacles = matchingInteractables
                .OfType<IObstacle>()
                .Distinct()
                .ToList();
        }

        damagedObstacles = ApplyDamage(nearObstacles, blastType);

        // Return the obstacles that actually took damage
        return damagedObstacles;
    }
    
   
}