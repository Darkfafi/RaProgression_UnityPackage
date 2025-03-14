using System;

namespace RaProgression
{
	public interface IRaProgress : IDisposable
	{
		public delegate void Handler(IRaProgress progress);
		public delegate void SignalHandler(IRaProgress progress, string message, object source);

		public event Handler ProgressedEvaluatedEvent;
		public event Handler ProgressStartedEvent;
		public event Handler ProgressCompletedEvent;
		public event Handler ProgressCancelledEvent;
		public event Handler ProgressResetEvent;
		public event SignalHandler ProgressSignalEvent;

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
