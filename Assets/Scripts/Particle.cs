using System;
using System.Collections.Generic;
using UnityEngine;

public class Particle: Mover
{
    private const float cubeParticleScale = 0.2f;
    private const float tntParticleScale = 0.3f;
    private const float vaseParticleScale = 0.2f;
    private const float boxParticleScale = 0.1f;

    [SerializeField] private readonly Transform poolTransform;

    public static readonly Dictionary<InteractableType, Vector3> TypesParticleScale 
        = new Dictionary<InteractableType, Vector3>
        {
            // For red, green, blue, yellow, stone — let's assume they're "cube-like" so use the same scale
            { InteractableType.red,    new Vector3(0.15f, 0.15f, 1f) },
            { InteractableType.green,  new Vector3(0.15f, 0.15f, 1f) },
            { InteractableType.blue,   new Vector3(0.15f, 0.15f, 1f) },
            { InteractableType.yellow, new Vector3(0.15f, 0.15f, 1f) },
            { InteractableType.stone,  new Vector3(0.1f, 0.1f, 1f) },
            { InteractableType.tnt,    new Vector3(0.3f, 0.3f, 1f) },
            { InteractableType.vase,   new Vector3(0.2f, 0.2f, 1f) },
            { InteractableType.box,    new Vector3(0.1f, 0.1f, 1f) },
            // "random" could default to something safe, or you could skip it
            { InteractableType.random, new Vector3(1, 1, 1f) }
        };
    
    public void ResetParticle(Transform poolTransform = null)
    {
        gameObject.SetActive(false);
        
        if (poolTransform is not null)
            transform.parent = poolTransform;
    }

    public static void ArrangeParticlesScale(IEnumerable<Particle> particles,InteractableType interactableType)
    {
        foreach (var particle in particles)
        {
            particle.transform.localScale = TypesParticleScale[interactableType];
        }
    }

}


