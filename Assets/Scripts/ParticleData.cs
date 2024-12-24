using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "NewParticleData", menuName = "ScriptableObjects/ParticleData", order = 1)]
public class ParticleData : ScriptableObject
{
    public  InteractableType particleTypeOF;

    public List<BlastParticle> LightParticles; 
    
    public List<BlastParticle> ExplosionParticle; 
}