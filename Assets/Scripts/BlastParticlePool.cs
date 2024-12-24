using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BlastParticlePool : MonoBehaviour
{
    
    [SerializeField] 
    //private List<ParticlePrefabEntity> particlePrefabEntities; // Entries for different interactable types and their particles
    
    private Dictionary<InteractableType, Dictionary<ParticleType,List<List<BlastParticle>>>> pooledParticles;

    private void Awake()
    {
        pooledParticles = new Dictionary<InteractableType, Dictionary<ParticleType,List<List<BlastParticle>>>>();
        //PoolObjects(15); // Initial pooling amount
    }

 
    /* public BlastParticle[] GetPooledObject(InteractableType type)
    {
        if (!pooledParticles.ContainsKey(type))
        {
            Debug.LogError($"No pool exists for particle type {type}");
            return null;
        }

        // Retrieve an inactive particle from the pool if available
        foreach (var pooledParticleList in pooledParticles[type])
        {
            if (!pooledParticleList.Any(x => x.gameObject.activeInHierarchy))
            {
                pooledParticleList.ForEach(x => x.gameObject.SetActive(true));
                return pooledParticleList.ToArray();
            }
        }

        // If no inactive particles are available, create a new one from the prefab list
        BlastParticle[] prefabToInstantiate = GetParticlePrefabByType(type);
        BlastParticle[] instancedParticles = new BlastParticle[prefabToInstantiate.Count()];

        for(int i = 0; i < prefabToInstantiate.Count() ; i++ ){

            BlastParticle newParticleListElement = Instantiate(prefabToInstantiate[i],transform);
            instancedParticles[i] = newParticleListElement;

        }
        return instancedParticles;
    } */

    


/* 
     // Retrieve a the specific prefab of the associated type
    public BlastParticle GetParticlePrefabByEffectType(InteractableType type, ParticleType particleType)
    {
        foreach (var entry in particlePrefabEntities)
        {
            if (entry.interactableType == type)
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
    }  */

    public void ReturnObjectToPool(BlastParticle toBeReturned)
    {
        if (toBeReturned == null)
        {
            Debug.LogError("Cannot return a null particle to the pool.");
            return;
        }
        toBeReturned.ResetParticle();
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
/* 
    private BlastParticle[] GetParticlePrefabByType(InteractableType type)
    {
        foreach (var entry in particlePrefabEntities)
        {
            if (entry.interactableType == type)
            {
                return entry.particlePrefabs.ToArray();
            }
        }

        Debug.LogError($"No particle prefab found for type {type}");
        return null;
    } */
}
