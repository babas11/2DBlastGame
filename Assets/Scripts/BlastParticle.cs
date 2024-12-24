using UnityEngine;

public class BlastParticle: Mover
{
    [SerializeField]
    ParticleType effectType;

    public void ResetParticle()
    {
        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }


}

public enum ParticleType{
    LighEffect,
    ExplosionEffect
}

