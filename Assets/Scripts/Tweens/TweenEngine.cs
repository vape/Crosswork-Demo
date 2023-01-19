using System.Collections.Generic;
using UnityEngine;

namespace Crosswork.Demo.Tweens
{
    public class TweenEngine : MonoBehaviour
    {
        public static TweenEngine Shared
        {
            get
            {
                if (!sharedCreated)
                {
                    var gameObject = new GameObject("Shared Tween Engine");
                    DontDestroyOnLoad(gameObject);
                    gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
                    shared = gameObject.AddComponent<TweenEngine>();
                    sharedCreated = true;
                }

                return shared;
            }
        }

        private static bool sharedCreated;
        private static TweenEngine shared;

        private List<Tween> tweens = new List<Tween>(capacity: 16);

        public void Add(Tween tween)
        {
            tweens.Add(tween);
        }

        private void Update()
        {
            var delta = Time.deltaTime;

            for (int i = tweens.Count - 1; i >= 0; --i)
            {
                var tween = tweens[i];

                if (tween.IsPlaying && !tween.InSequence)
                {
                    tween.Update(delta);
                }

                if (tween.IsCompleted)
                {
                    tweens.RemoveAt(i);
                }
            }
        }
    }
}
