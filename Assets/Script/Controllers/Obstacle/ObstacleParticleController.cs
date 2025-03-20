using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Controllers.Obstacle
{
    public class ObstacleParticleController : MonoBehaviour
    {
        #region Variables

        #region Serialized Variables

        [SerializeField] private ParticleSystem obstacleParticle;

        #endregion

        #endregion
        
        internal void SetParticleData(Sprite[] particleSprite)
        {
            foreach (var sprite in particleSprite)
            {
                obstacleParticle.textureSheetAnimation.AddSprite(sprite);
            }
        }
        
        [Button]
        internal void PlayObstacleParticle(Action onComplete = null)
        {
            transform.localScale = new Vector3(1, 1, 1);
            obstacleParticle.Play();
            StartCoroutine(WaitForParticle(obstacleParticle, onComplete));
        }
        
        private IEnumerator WaitForParticle(ParticleSystem particle, Action onComplete)
        {
            yield return new WaitUntil(() => !particle.isPlaying);
            onComplete?.Invoke();
        }
    }
}