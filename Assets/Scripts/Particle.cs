using UnityEngine;

public class Particle: Mover
{

    public void ResetParticle()
    {
        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

}


