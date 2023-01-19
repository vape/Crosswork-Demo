using System.Collections.Generic;
using UnityEngine;

namespace Crosswork.Demo.Tweens
{
    public class TweenSequence : Tween
    {
        public static TweenSequence Create()
        {
            var tween = new TweenSequence();

            TweenEngine.Shared.Add(tween);

            return tween;
        }

        public override bool IsPlaying
        { get { return isPlaying; } }

        public override bool IsCompleted
        { get { return isCompleted; } }

        public override bool IsStopped
        { get { return isStopped; } }

        private bool isCompleted;
        private bool isStopped;
        private bool isPlaying;
        private List<Tween> tweens;

        public TweenSequence Add(Tween tween)
        {
            if (tweens == null)
            {
                tweens = new List<Tween>(capacity: 2);
            }

            tween.InSequence = true;
            tweens.Add(tween);

            return this;
        }

        public override void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
        }

        public override void Stop(bool complete)
        {
            if (isStopped)
            {
                return;
            }

            isPlaying = false;
            isStopped = true;
            isCompleted = complete;
        }

        public override void Update(float delta)
        {
            if (!isPlaying)
            {
                return;
            }

            for (int i = 0; i < tweens.Count; ++i)
            {
                var tween = tweens[i];

                if (!tween.IsPlaying)
                {
                    tween.Play();
                }

                if (tween.IsCompleted)
                {
                    continue;
                }

                tween.Update(delta);
                break;
            }

            if (tweens[tweens.Count - 1].IsCompleted)
            {
                Stop(complete: true);
            }
        }
    }
}
