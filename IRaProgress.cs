using System;

namespace RaProgression
{
	public interface IRaProgress
	{
		public event Action<IRaProgress> ProgressedEvaluatedEvent;
		public event Action<IRaProgress> ProgressStartedEvent;
		public event Action<IRaProgress> ProgressCompletedEvent;
		public event Action<IRaProgress> ProgressCancelledEvent;
		public event Action<IRaProgress> ProgressResetEvent;

		float NormalizedValue
		{
			get;
		}

		RaProgressState State
		{
			get;
		}
	}
}
