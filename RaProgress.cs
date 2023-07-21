using System;
using UnityEngine;

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

		public bool HasEnded => State == RaProgressState.Completed || State == RaProgressState.Cancelled;

		public RaProgress(bool markAsStarted = true)
		{
			State = RaProgressState.None;
			NormalizedValue = 0f;

			if(markAsStarted)
			{
				Start();
			}
		}

		public RaProgress OnStart(Action<IRaProgress> callback)
		{
			ProgressStartedEvent += callback;
			return this;
		}

		public RaProgress OnEvaluate(Action<IRaProgress> callback)
		{
			ProgressedEvaluatedEvent += callback;
			return this;
		}

		public RaProgress OnCompleted(Action<IRaProgress> callback)
		{
			ProgressCompletedEvent += callback;
			return this;
		}

		public RaProgress OnCancelled(Action<IRaProgress> callback)
		{
			ProgressCancelledEvent += callback;
			return this;
		}

		public RaProgress OnEnd(Action<IRaProgress> callback)
		{
			ProgressCancelledEvent += callback;
			ProgressCompletedEvent += callback;
			return this;
		}

		public RaProgress OnReset(Action<IRaProgress> callback)
		{
			ProgressResetEvent += callback;
			return this;
		}

		public bool Start(bool throwIfNotValid = true)
		{
			if(State != RaProgressState.None)
			{
				if(throwIfNotValid)
				{
					throw new InvalidOperationException($"Can't {nameof(Start)} {nameof(RaProgress)} which is not in {nameof(RaProgressState)} {nameof(RaProgressState.None)} (is now in {State}). Be sure to call {nameof(Reset)} if in use / used");
				}
				return false;
			}

			State = RaProgressState.InProgress;
			ProgressStartedEvent?.Invoke(this);
			return Evaluate(0f);
		}

		public bool Evaluate(float normalizedValue, bool throwIfNotValid = true)
		{
			if(IfNotInProgress(nameof(Evaluate), throwIfNotValid))
			{
				return false;
			}
			
			NormalizedValue = Mathf.Clamp01(normalizedValue);
			ProgressedEvaluatedEvent?.Invoke(this);

			return true;
		}

		public bool Complete(bool throwIfNotValid = true)
		{
			if(IfNotInProgress(nameof(Complete), throwIfNotValid))
			{
				return false;
			}

			Evaluate(1f);

			State = RaProgressState.Completed;
			ProgressCompletedEvent?.Invoke(this);
			return true;
		}

		public bool Cancel(bool throwIfNotValid = true)
		{
			if(IfNotInProgress(nameof(Cancel), throwIfNotValid))
			{
				return false;
			}

			State = RaProgressState.Cancelled;
			ProgressCancelledEvent?.Invoke(this);
			return true;
		}

		public bool Reset(bool throwIfNotValid = true)
		{
			if(State == RaProgressState.None)
			{
				if(throwIfNotValid)
				{
					throw new InvalidOperationException($"Can't {nameof(Start)} {nameof(RaProgress)} which is in {nameof(RaProgressState)} {nameof(RaProgressState.None)} (is now in {State}). Be sure to call {nameof(Reset)} only when the object has been used");
				}
				return false;
			}

			State = RaProgressState.None;
			NormalizedValue = 0f;
			ProgressResetEvent?.Invoke(this);
			return true;
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

		public void Dispose()
		{
			Recycle();
		}

		private bool IfNotInProgress(string operation, bool throwException)
		{
			if(State != RaProgressState.InProgress)
			{
				if(throwException)
				{
					throw new InvalidOperationException($"Can't {operation} {nameof(RaProgress)} which is not in {nameof(RaProgressState)} {nameof(RaProgressState.InProgress)} (is now in {State}). Be sure to call {nameof(Start)} (also after {nameof(Reset)})");
				}
				return true;
			}
			return false;
		}
	}
}
