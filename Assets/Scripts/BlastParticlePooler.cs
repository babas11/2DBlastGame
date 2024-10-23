using System.Collections.Generic;
using UnityEngine;

public class BlastParticlePooler : MonoBehaviour, IObjectPooler<BlastParticle, InteractableType>
{
    [System.Serializable]
    public struct ParticlePrefabEntity
    {
        public InteractableType type;
        public List<BlastParticle> particlePrefabs; // List of particle prefabs associated with this type
    }

    [SerializeField] private List<ParticlePrefabEntity> particlePrefabEntries; // Entries for different interactable types and their particles
    private Dictionary<InteractableType, List<BlastParticle>> pooledParticles;

    private void Awake()
    {
        pooledParticles = new Dictionary<InteractableType, List<BlastParticle>>();
        PoolObjects(15); // Initial pooling amount
    }

    public void PoolObjects(int amount = 15)
    {
        if (particlePrefabEntries == null || particlePrefabEntries.Count == 0)
        {
            Debug.LogError("Particle prefab entries list is empty. Cannot pool particles.");
            return;
        }

        foreach (var entry in particlePrefabEntries)
        {
            if (!pooledParticles.ContainsKey(entry.type))
            {
                pooledParticles[entry.type] = new List<BlastParticle>();
            }

            foreach (var prefab in entry.particlePrefabs)
            {
                for (int i = 0; i < amount; i++)
                {
                    BlastParticle newParticle = Instantiate(prefab, transform);
                    newParticle.gameObject.SetActive(false);
                    pooledParticles[entry.type].Add(newParticle);
                }
            }
        }
    }

    public BlastParticle GetPooledObject(InteractableType type)
    {
        if (!pooledParticles.ContainsKey(type))
        {
            Debug.LogError($"No pool exists for particle type {type}");
            return null;
        }

        // Retrieve an inactive particle from the pool if available
        foreach (var pooledParticle in pooledParticles[type])
        {
            if (!pooledParticle.gameObject.activeInHierarchy)
            {
                pooledParticle.gameObject.SetActive(true);
                return pooledParticle;
            }
        }

        // If no inactive particles are available, create a new one from the prefab list
        BlastParticle prefabToInstantiate = GetParticlePrefabByType(type);
        if (prefabToInstantiate != null)
        {
            BlastParticle newParticle = Instantiate(prefabToInstantiate, transform);
            pooledParticles[type].Add(newParticle);
            newParticle.gameObject.SetActive(true);
            return newParticle;
        }

        return null;
    }

    public BlastParticle GetPooledObjectByTypeAndIndex(InteractableType type, int index)
{
    if (!pooledParticles.ContainsKey(type))
    {
        Debug.LogError($"No pool exists for particle type {type}");
        return null;
    }

    // Retrieve an inactive particle from the pool if available
    foreach (var pooledParticle in pooledParticles[type])
    {
        if (!pooledParticle.gameObject.activeInHierarchy)
        {
            pooledParticle.gameObject.SetActive(true);
            return pooledParticle;
        }
    }

    // If no inactive particles are available, create a new one from the prefab list using the specified index
    BlastParticle prefabToInstantiate = GetParticlePrefabByTypeAndIndex(type, index);
    if (prefabToInstantiate != null)
    {
        BlastParticle newParticle = Instantiate(prefabToInstantiate, transform);
        pooledParticles[type].Add(newParticle);
        newParticle.gameObject.SetActive(true);
        return newParticle;
    }

    return null;
}


    // Retrieve a the specific prefab of the associated type
     public BlastParticle GetParticlePrefabByTypeAndIndex(InteractableType type, int index)
    {
        foreach (var entry in particlePrefabEntries)
        {
            if (entry.type == type)
            {
                if (entry.particlePrefabs != null && entry.particlePrefabs.Count > index && index >= 0)
                {
                    BlastParticle newParticle = Instantiate(entry.particlePrefabs[index], transform);
                    pooledParticles[type].Add(newParticle);
                    newParticle.gameObject.SetActive(true);
                    return newParticle;
                }
                else
                {
                    Debug.LogError($"Index {index} is out of range for particle prefabs of type {type}");
                    return null;
                }
            }
        }

        Debug.LogError($"No particle prefab found for type {type}");
        return null;
    }

    public void ReturnObjectToPool(BlastParticle toBeReturned)
    {
        if (toBeReturned == null)
        {
            Debug.LogError("Cannot return a null particle to the pool.");
            return;
        }

        toBeReturned.gameObject.SetActive(false);
    }

    public void ReturnObjectToPool(List<BlastParticle> toBeReturned)
    {
        foreach (var particle in toBeReturned)
        {
            if (particle == null)
            {
                Debug.LogError("Cannot return a null particle to the pool.");
                return;
            }
        }

        foreach (var particle in toBeReturned)
        {
            ReturnObjectToPool(particle);
        }
    }

    private BlastParticle GetParticlePrefabByType(InteractableType type)
    {
        foreach (var entry in particlePrefabEntries)
        {
            if (entry.type == type)
            {
                // Return a random particle prefab associated with this type
                if (entry.particlePrefabs != null && entry.particlePrefabs.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, entry.particlePrefabs.Count);
                    return entry.particlePrefabs[randomIndex];
                }
            }
        }

        Debug.LogError($"No particle prefab found for type {type}");
        return null;
    }
}
