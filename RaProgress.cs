using UnityEngine;
using System;

namespace RaProgression
{
	public class RaProgress : IRaProgress
	{
		public event Action<IRaProgress> ProgressedEvaluatedEvent;
		public event Action<IRaProgress> ProgressStartedEvent;
		public event Action<IRaProgress> ProgressCompletedEvent;
		public event Action<IRaProgress> ProgressCancelledEvent;
		public event Action<IRaProgress> ProgressResetEvent;

		public float NormalizedValue
		{
			get; private set;
		}

		public RaProgressState State
		{
			get; private set;
		}

		public RaProgress()
		{
			State = RaProgressState.None;
			NormalizedValue = 0f;
		}

		public void Start()
		{
			if(State != RaProgressState.None)
			{
				return;
			}

			State = RaProgressState.InProgress;
			ProgressStartedEvent?.Invoke(this);
			Evaluate(0f);
		}

		public void Evaluate(float normalizedValue)
		{
			ThrowIfNotInProgress(nameof(Evaluate));
			NormalizedValue = Mathf.Clamp01(normalizedValue);
			ProgressedEvaluatedEvent?.Invoke(this);
		}

		public void Complete()
		{
			ThrowIfNotInProgress(nameof(Complete));
			State = RaProgressState.Completed;
			ProgressCompletedEvent?.Invoke(this);
		}

		public void Cancel()
		{
			ThrowIfNotInProgress(nameof(Cancel));
			State = RaProgressState.Cancelled;
			ProgressCancelledEvent?.Invoke(this);
		}

		public void Reset()
		{
			if(State == RaProgressState.None)
			{
				return;
			}

			State = RaProgressState.None;
			NormalizedValue = 0f;
			ProgressResetEvent?.Invoke(this);
		}

		public void Recycle()
		{
			ProgressStartedEvent = null;
			ProgressedEvaluatedEvent = null;
			ProgressCompletedEvent = null;
			ProgressCancelledEvent = null;
			ProgressResetEvent = null;

			State = RaProgressState.None;
			NormalizedValue = 0f;
		}

		private void ThrowIfNotInProgress(string operation)
		{
			if(State != RaProgressState.InProgress)
			{
				throw new InvalidOperationException($"Can't {operation} {nameof(RaProgress)} which is not in {nameof(RaProgressState)} {nameof(RaProgressState.InProgress)}. Be sure to call {nameof(Start)} (also after {nameof(Reset)})");
			}
		}
	}
}
