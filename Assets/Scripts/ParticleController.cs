using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class ParticleController : MonoBehaviour
{
    [SerializeField] private float spawnRadius;

    ParticlePool particlePool;

    public static  Action<Vector3,InteractableType> BlastParticle;
    public static  Action<IEnumerable<Interactable>> BlastParticles;
    public static  Action<Transform, InteractableType> OnCreation;


    void OnEnable()
    {
        BlastParticle += EmitParticle;
        BlastParticles += EmitParticles;
        OnCreation += EmitCreationFlash;
    }

    void OnDisable()
    {
        BlastParticle -= EmitParticle;
        BlastParticles -= EmitParticles;
        OnCreation -= EmitCreationFlash;
    }

    private void Start()
    {
        particlePool = GetComponent<ParticlePool>();
    }

    public void EmitParticle(Vector3 position, InteractableType particleType)
    {
        // 1) Get a particle object from your pool
        Particle[] particleObj = particlePool.GetParticleBatch(particleType);
        
        Vector3[] jumpEndPos = new Vector3[particleObj.Length];
        for (int i = 0; i < particleObj.Length; i++)
        {
            // 3) Place the particle at (origin + random offset)
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 startPos = position + new Vector3(randomOffset.x, randomOffset.y, 0);
            particleObj[i].transform.position = startPos;
            particleObj[i].gameObject.SetActive(true);
            
            jumpEndPos[i] = startPos + (Vector3.right * Random.Range(-0.5f, 0.5f));
            jumpEndPos[i].y = jumpEndPos[i].y - 10f;
        }
        
        AnimationHandler.FallAndRotate(particleObj, jumpEndPos, 6f, 2f,1,
            () =>
        {
            particlePool.ReturnParticles(particleObj);
        });
        
    }

    public void EmitCreationFlash(Transform parentTransform, InteractableType particleType)
    {
        Particle[] particleObj = particlePool.GetParticleBatch(particleType,true,0f);
        
        
        Utilities.SetPositions(particleObj,parentTransform.position);
        Utilities.SetParent(particleObj,parentTransform);
        
        AnimationHandler.ScaleUpAndDown(particleObj[0],1f,0,.1f,
            () =>
        {
            particlePool.ReturnParticles(particleObj);
        });
        
        AnimationHandler.ScaleUpAndDown(particleObj[1],1f,0,.3f,
            () => 
            {
                particlePool.ReturnParticles(particleObj);
            });
        
    }

   

    void EmitParticles(IEnumerable<Interactable> particles)
    {
        foreach (var interactable in particles)
        {
            EmitParticle(interactable.transform.position, interactable.Type);
        }
    }
    void EmitParticles(IEnumerable<IObstacle> particles)
    {
        foreach (var obstacle in particles)
        {
            EmitParticle(obstacle.ObstacleWorldPos, obstacle.ObstacleType);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EmitParticle(new Vector3(0, 0, 0), InteractableType.box);
        }
    }
}


    
