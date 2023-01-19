using UnityEngine;

namespace Crosswork.Demo.Tweens
{
    public abstract class Tween
    {
        internal bool InSequence
        { get; set; }

        public abstract bool IsPlaying
        { get; }
        public abstract bool IsCompleted
        { get; }
        public abstract bool IsStopped
        { get; }

        public abstract void Play();
        public abstract void Stop(bool complete);
        public abstract void Update(float delta);
    }

    public delegate TValue TweenGetter<TValue>();
    public delegate void TweenSetter<TValue>(TValue value);

    public enum TweenEasing
    {
        Linear,
        OutQuad,
        InQuad,
        InOutQuad
    }

    public abstract class TweenAnimator<TAnimatable> : Tween
    {
        protected static float Ease(float value, TweenEasing easing)
        {
            switch (easing)
            {
                case TweenEasing.OutQuad:
                    return 1 - (1 - value) * (1 - value);
                case TweenEasing.InQuad:
                    return value * value;
                case TweenEasing.InOutQuad:
                    return value < 0.5 ? 8 * value * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 4) / 2;
                default:
                    return value;
            }
        }

        public override bool IsPlaying
        { get { return isPlaying; } }
        public override bool IsCompleted
        { get { return isCompleted; } }
        public override bool IsStopped
        { get { return isStopped; } }

        protected bool isStopped;
        protected bool isCompleted;
        protected bool isPlaying;
        protected float time;
        protected float duration;
        protected bool relative;
        protected float virtualTime;

        protected TAnimatable startValue;
        protected TAnimatable endValue;

        protected bool trackedObjectSet;
        protected UnityEngine.Object trackedObject;

        protected TweenGetter<TAnimatable> getter;
        protected TweenSetter<TAnimatable> setter;

        protected TweenEasing easing;

        public TweenAnimator(TweenGetter<TAnimatable> getter, TweenSetter<TAnimatable> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public virtual TweenAnimator<TAnimatable> SetTrackedObject(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                trackedObjectSet = true;
                trackedObject = obj;
            }

            return this;
        }

        public virtual TweenAnimator<TAnimatable> SetEase(TweenEasing easing)
        {
            this.easing = easing;
            return this;
        }

        public virtual TweenAnimator<TAnimatable> SetRelative(bool value)
        {
            relative = value;
            return this;
        }

        public virtual TweenAnimator<TAnimatable> SetEnd(TAnimatable value)
        {
            endValue = value;
            return this;
        }

        public virtual TweenAnimator<TAnimatable> SetDuration(float duration)
        {
            this.duration = duration;
            return this;
        }

        public override sealed void Update(float delta)
        {
            if (!isPlaying)
            {
                return;
            }

            if (trackedObjectSet && trackedObject == null)
            {
                Stop(complete: false);
                return;
            }

            if (virtualTime == 0f)
            {
                OnUpdate(0f);
            }

            virtualTime = Mathf.Clamp01(virtualTime + (delta / duration));
            time = Ease(virtualTime, easing);

            OnUpdate(delta);

            if (virtualTime >= 1f)
            {
                Stop(complete: true);
            }
        }

        public override sealed void Play()
        {
            if (isPlaying)
            {
                return;
            }

            isPlaying = true;

            if (!trackedObjectSet || trackedObject != null)
            {
                startValue = getter();
            }
        }

        public override sealed void Stop(bool complete)
        {
            if (!isPlaying)
            {
                return;
            }

            if (complete && virtualTime < 1f)
            {
                Update(1f - virtualTime);
                return;
            }

            isPlaying = false;
            isCompleted = complete;
            isStopped = true;

            OnStopped(complete);
        }

        protected virtual void OnPlay()
        { }

        protected virtual void OnStopped(bool completed)
        { }

        protected virtual void OnUpdate(float delta)
        { }
    }

    public class Vector3TweenAnimator : TweenAnimator<Vector3>
    {
        public Vector3TweenAnimator(TweenGetter<Vector3> getter, TweenSetter<Vector3> setter)
            : base(getter, setter)
        { }

        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            if (relative)
            {
                var previousValue = Vector3.Lerp(startValue, endValue, time - delta);
                var currentValue = Vector3.Lerp(startValue, endValue, time);

                setter(getter() + (currentValue - previousValue));
            }
            else
            {
                setter(Vector3.Lerp(startValue, endValue, time));
            }
        }
    }
}
