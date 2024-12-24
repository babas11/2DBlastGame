using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

/// <summary>
/// Manages pooling of <see cref="Particle"/> objects, grouped by <see cref="InteractableType"/>.
/// Each InteractableType can have its own "batches" of multiple <see cref="Particle"/> prefabs.
/// 
/// The pool uses a nested structure:
/// - A dictionary where the key is an <see cref="InteractableType"/>.
/// - The value is a list of batches (each batch is a <c>List&lt;BlastParticle&gt;</c>).
/// 
/// Example:
/// <code>
/// particlePool[red] -> [ batch1, batch2, ... ]
/// batch1 -> [ redParticle1, redParticle2, ... ]
/// </code>
/// </summary>
public partial class ParticlePool : MonoBehaviour
{
    /// <summary>
    /// A struct pairing an <see cref="InteractableType"/> with its list of <see cref="Particle"/> prefabs.
    /// Each entry indicates that for a particular <see cref="InteractableType"/>, these prefabs constitute
    /// a single batch of particles.
    /// </summary>
    [System.Serializable]
    struct ParticleBatchDefinition
    {
        /// <summary>
        /// Which <see cref="InteractableType"/> this batch definition belongs to.
        /// </summary>
        [Tooltip("Which InteractableType this batch of particle prefabs belongs to.")]
        public InteractableType InteractableType;

        /// <summary>
        /// The list of particle prefabs that should spawn together as a batch.
        /// </summary>
        [Tooltip("Particle prefabs to spawn together as a batch.")]
        public List<Particle> ParticlePrefabs;
    }

    /// <summary>
    /// The definitions mapping each <see cref="InteractableType"/> to a set ("batch") of particle prefabs.
    /// </summary>
    [Tooltip("The definitions mapping an InteractableType to a set (batch) of particle prefabs.")]
    [SerializeField]
    private List<ParticleBatchDefinition> batchDefinitions;

    /// <summary>
    /// Number of new batches to create for each <see cref="InteractableType"/> when populating the pool.
    /// </summary>
    [Tooltip("Amount of batches to create when populating the pool.")]
    [SerializeField]
    public int batchCount = 15; 

    /// <summary>
    /// If true, the pool automatically expands (creates new batches) when no inactive batch is found.
    /// </summary>
    [Tooltip("If true, the pool will expand automatically when no inactive batches are available.")]
    public bool allowPoolExpansion = true;

    /// <summary>
    /// The main dictionary representing the particle pool:
    /// Key = <see cref="InteractableType"/>
    /// Value = A list of batches (each batch is a <c>List&lt;BlastParticle&gt;</c>).
    /// Example:
    /// <code>
    /// particlePool[type] -> [batch1, batch2, ...]
    /// </code>
    /// </summary>
    private Dictionary<InteractableType, List<List<Particle>>> particlePool;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the dictionary and populates the pool with an initial set of batches.
    /// </summary>
    private void Awake()
    {
        // Prepare the dictionary for storing multiple batches per InteractableType.
        particlePool = new Dictionary<InteractableType, List<List<Particle>>>();

        // Populate the pool with the specified number of batches (bactchCount) for each type.
        PopulatePool(batchCount);
    }

    /// <summary>
    /// Creates or expands the pool by instantiating the specified number of new batches
    /// for each <see cref="InteractableType"/>.
    /// </summary>
    /// <param name="batchCount">Number of new batches to create per InteractableType.</param>
    public void PopulatePool(int batchCount = 15)
    {
        foreach (var definition in batchDefinitions)
        {
            // If this type isn't tracked yet, initialize its list of batches.
            if (!particlePool.ContainsKey(definition.InteractableType))
            {
                particlePool.Add(definition.InteractableType, new List<List<Particle>>());
            }

            // Create 'batchCount' new batches for this InteractableType.
            for (int i = 0; i < batchCount; i++)
            {
                // Each batch is a list of BlastParticles.
                List<Particle> newBatch = new List<Particle>();

                // Instantiate each prefab in the definition, disable it, and add to the batch.
                foreach (var prefab in definition.ParticlePrefabs)
                {
                    Particle particleInstance = Instantiate(prefab, transform);
                    particleInstance.gameObject.SetActive(false);
                    newBatch.Add(particleInstance);
                }

                // Add this new batch to the dictionary's list of batches for the current InteractableType.
                particlePool[definition.InteractableType].Add(newBatch);
            }
        }
    }

    /// <summary>
    /// Retrieves a batch of inactive <see cref="Particle"/> objects for the specified <see cref="InteractableType"/>.
    /// If none is available, optionally expands the pool (if <see cref="allowPoolExpansion"/> is true).
    /// Otherwise, it instantiates new particles directly from the defined prefabs as a fallback.
    /// </summary>
    /// <param name="type">The <see cref="InteractableType"/> to fetch a batch for.</param>
    /// <param name="autoActivate">
    /// If true, all particles in the batch will be set active upon retrieval (default = true).
    /// </param>
    /// <param name="expansionBatchCount">
    /// Number of new batches to create if expansion is needed (default = 5).
    /// </param>
    /// <returns>
    /// An array of <see cref="Particle"/> forming one complete batch, or null if none can be found or created.
    /// </returns>
    public Particle[] GetParticleBatch(InteractableType type, bool autoActivate = true, int expansionBatchCount = 5)
    {
        // Check if the dictionary has any entries for this type.
        if (!particlePool.ContainsKey(type))
        {
            Debug.LogError($"No particle pool found for InteractableType '{type}'.");
            return null;
        }

        // Try to find an entirely inactive batch.
        Particle[] inactiveBatch = FindInactiveBatch(type);
        if (inactiveBatch != null)
        {
            // If user wants them active, activate them.
            if (autoActivate)
            {
                SetBatchActive(inactiveBatch, true);
            }
            return inactiveBatch;
        }

        // If no inactive batch is found, but expansion is allowed, populate new batches.
        if (allowPoolExpansion)
        {
            PopulatePool(expansionBatchCount);
            
            // Try again after expansion.
            inactiveBatch = FindInactiveBatch(type);
            if (inactiveBatch != null)
            {
                if (autoActivate)
                {
                    SetBatchActive(inactiveBatch, true);
                }
                return inactiveBatch;
            }
        }

        // As a last resort, create a new batch directly from the prefab definitions,
        // and add it to the pool.
        Particle[] particlePrefabs = GetPrefabsForType(type);
        if (particlePrefabs == null || particlePrefabs.Length == 0)
        {
            Debug.LogError($"No particle prefab found or assigned for InteractableType '{type}'.");
            return null;
        }

        // Instantiate a new batch directly (not from the existing pre-populated batches).
        Particle[] directInstantiation = new Particle[particlePrefabs.Length];
        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            Particle newParticle = Instantiate(particlePrefabs[i], transform);
            directInstantiation[i] = newParticle;
        }

        // Add this new direct-instantiated batch to the pool for future reuse.
        particlePool[type].Add(directInstantiation.ToList());

        // Optionally activate the new batch if requested.
        if (autoActivate)
        {
            SetBatchActive(directInstantiation, true);
        }

        return directInstantiation;
    }

    /// <summary>
    /// Searches for the first batch of <see cref="Particle"/> for the specified <see cref="InteractableType"/>
    /// that is entirely inactive.
    /// </summary>
    /// <param name="type">The <see cref="InteractableType"/> in which to search for an inactive batch.</param>
    /// <returns>
    /// An array of <see cref="Particle"/> representing an inactive batch, 
    /// or null if none is found.
    /// </returns>
    private Particle[] FindInactiveBatch(InteractableType type)
    {
        // Iterate over each batch for the given type.
        foreach (var batch in particlePool[type])
        {
            // Check if all particles in this batch are inactive.
            if (AreAllInactive(batch))
            {
                // Return the batch as an array for convenience.
                return batch.ToArray();
            }
        }
        return null;
    }

    /// <summary>
    /// Determines whether all <see cref="Particle"/> objects in a batch are inactive.
    /// </summary>
    /// <param name="batch">The list of <see cref="Particle"/> to check.</param>
    /// <returns>True if every particle in the batch is inactive; otherwise false.</returns>
    private bool AreAllInactive(List<Particle> batch)
    {
        for (int i = 0; i < batch.Count; i++)
        {
            if (batch[i].gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Sets the active state of all <see cref="Particle"/> objects in the specified batch.
    /// </summary>
    /// <param name="particleBatch">An array of <see cref="Particle"/> to modify.</param>
    /// <param name="isActive">Whether to set them active (true) or inactive (false).</param>
    private void SetBatchActive(Particle[] particleBatch, bool isActive)
    {
        for (int i = 0; i < particleBatch.Length; i++)
        {
            particleBatch[i].gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Returns a single <see cref="Particle"/> to the pool by resetting it
    /// (e.g., disabling it and clearing any transient state).
    /// </summary>
    /// <param name="particleToReturn">The particle instance to reset and return.</param>
    public void ReturnParticle(Particle particleToReturn)
    {
        if (particleToReturn == null)
        {
            Debug.LogError("Cannot return a null particle to the pool.");
            return;
        }

        // Reset the particle so that it's ready for reuse.
        particleToReturn.ResetParticle();
    }

    /// <summary>
    /// Returns a list of <see cref="Particle"/> objects to the pool by resetting each one.
    /// </summary>
    /// <param name="particlesToReturn">The list of particle instances to reset and return.</param>
    public void ReturnParticles(List<Particle> particlesToReturn)
    {
        // Check for any null references in the provided list.
        foreach (var particle in particlesToReturn)
        {
            if (particle == null)
            {
                Debug.LogError("Cannot return a null particle to the pool.");
                return;
            }
        }

        // Reset each particle in the list.
        foreach (var particle in particlesToReturn)
        {
            ReturnParticle(particle);
        }
    }

    /// <summary>
    /// Retrieves an array of <see cref="Particle"/> prefabs for the specified <see cref="InteractableType"/>
    /// from the configured <see cref="batchDefinitions"/>.
    /// </summary>
    /// <param name="type">The <see cref="InteractableType"/> whose prefabs are to be retrieved.</param>
    /// <returns>An array of <see cref="Particle"/> prefabs, or null if not found.</returns>
    private Particle[] GetPrefabsForType(InteractableType type)
    {
        // Search the batch definitions for a matching type.
        foreach (var definition in batchDefinitions)
        {
            if (definition.InteractableType == type)
            {
                return definition.ParticlePrefabs.ToArray();
            }
        }

        Debug.LogError($"No particle prefab definition found for InteractableType '{type}'.");
        return null;
    }
}