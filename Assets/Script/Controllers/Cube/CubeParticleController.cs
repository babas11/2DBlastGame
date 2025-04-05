using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Script.Controllers.Cube
{
    public class CubeParticleController : MonoBehaviour
    {
        #region Variables

        #region Serialized Variables

        [SerializeField] private ParticleSystem cubeParticle;
        [SerializeField] private ParticleSystem[] tntParticle;

        #endregion

        #endregion
        
        internal void SetParticleData(Sprite particleSprite)
        {
            cubeParticle.textureSheetAnimation.AddSprite(particleSprite);
        }
        
        [Button]
        internal void PlayCubeParticle(Action onComplete = null)
        {
            transform.localScale = new Vector3(1, 1, 1);
            cubeParticle.Play();
            StartCoroutine(WaitForParticle(cubeParticle, onComplete));
        }
        
        [Button]
        internal void PlayTntParticle(Action onComplete = null)
        {
            transform.localScale = new Vector3(1, 1, 1);
            tntParticle.ForEach(x => x.Play());
            StartCoroutine(WaitForParticle(tntParticle[0], onComplete));
        }
        private IEnumerator WaitForParticle(ParticleSystem particle, Action onComplete)
        {
            yield return new WaitUntil(() => !particle.isPlaying);
            onComplete?.Invoke();
        }
    }
}