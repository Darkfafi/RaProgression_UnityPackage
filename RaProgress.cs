using System;
using UnityEngine;
using static RaProgression.IRaProgress;

namespace RaProgression
{
	public class RaProgress : IRaProgress
	{
		public event Handler ProgressedEvaluatedEvent;
		public event Handler ProgressStartedEvent;
		public event Handler ProgressCompletedEvent;
		public event Handler ProgressCancelledEvent;
		public event Handler ProgressResetEvent;
		public event SignalHandler ProgressSignalEvent;

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

		public RaProgress OnStart(Handler callback)
		{
			ProgressStartedEvent += callback;
			return this;
		}

		public RaProgress OnEvaluate(Handler callback)
		{
			ProgressedEvaluatedEvent += callback;
			return this;
		}

		public RaProgress OnCompleted(Handler callback)
		{
			ProgressCompletedEvent += callback;
			return this;
		}

		public RaProgress OnCancelled(Handler callback)
		{
			ProgressCancelledEvent += callback;
			return this;
		}

		public RaProgress OnEnd(Handler callback)
		{
			ProgressCancelledEvent += callback;
			ProgressCompletedEvent += callback;
			return this;
		}

		public RaProgress OnSignal(SignalHandler callback)
		{
			ProgressSignalEvent += callback;
			return this;
		}

		public RaProgress OnReset(Handler callback)
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

		public void FireSignal(string message, object source)
		{
			ProgressSignalEvent?.Invoke(this, message, source);
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

		public void Recycle(bool clearSignalChannel)
		{
			ProgressStartedEvent = null;
			ProgressedEvaluatedEvent = null;
			ProgressCompletedEvent = null;
			ProgressCancelledEvent = null;
			ProgressResetEvent = null;

			if(clearSignalChannel)
			{
				ClearSignalChannel();
			}

			State = RaProgressState.None;
			NormalizedValue = 0f;
		}

		public void ClearSignalChannel()
		{
			ProgressSignalEvent = null;
		}

		public void Dispose()
		{
			Recycle(clearSignalChannel: true);
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
