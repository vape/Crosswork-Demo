using System.Collections;
using UnityEngine;

namespace Crosswork.Demo.Match3.View
{
    public class ParticleEffectLifetimeController : MonoBehaviour
    {
        [SerializeField]
        private new ParticleSystem particleSystem;

        private IEnumerator Start()
        {
            while (particleSystem.isPlaying)
            {
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
