using UnityEngine;

namespace Crosswork.Demo.Tweens
{
    public static class TransformTweenExcensions
    {
        public static TweenAnimator<Vector3> TweenLocalMove(this Transform transform, Vector3 to, float duration)
        {
            var tween = new Vector3TweenAnimator(() => transform.localPosition, (value) => transform.localPosition = value)
                .SetTrackedObject(transform)
                .SetDuration(duration)
                .SetEnd(to);

            TweenEngine.Shared.Add(tween);

            return tween;
        }

        public static TweenAnimator<Vector3> TweenMove(this Transform transform, Vector3 to, float duration)
        {
            var tween = new Vector3TweenAnimator(() => transform.position, (value) => transform.position = value)
                .SetTrackedObject(transform)
                .SetDuration(duration)
                .SetEnd(to);

            TweenEngine.Shared.Add(tween);

            return tween;
        }

        public static TweenAnimator<Vector3> TweenScale(this Transform transform, Vector3 to, float duration)
        {
            var tween = new Vector3TweenAnimator(() => transform.localScale, (value) => transform.localScale = value)
                .SetTrackedObject(transform)
                .SetDuration(duration)
                .SetEnd(to);

            TweenEngine.Shared.Add(tween);

            return tween;
        }
    }
}
