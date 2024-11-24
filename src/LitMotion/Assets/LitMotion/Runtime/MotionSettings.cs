using UnityEngine;

namespace LitMotion
{
    public record MotionSettings<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        public TValue StartValue { get; init; }
        public TValue EndValue { get; init; }
        public float Duration { get; init; }
        public TOptions Options { get; init; }
        public Ease Ease { get; init; }
        public AnimationCurve CustomEaseCurve { get; init; }
        public float Delay { get; init; }
        public DelayType DelayType { get; init; }
        public int Loops { get; init; } = 1;
        public LoopType LoopType { get; init; }
        public bool CancelOnError { get; init; }
        public bool SkipValuesDuringDelay { get; init; }
        public bool BindOnSchedule { get; init; }
        public IMotionScheduler Scheduler { get; init; }
    }
}