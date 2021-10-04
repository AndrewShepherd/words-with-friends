namespace WordsWithFriends.Gui
{
	using System;
	using System.Reactive.Subjects;
	using System.Threading.Tasks;

	class BackgroundTaskGenerationObservable<T, U> : IObserver<T>, IObservable<U>
	{
		public BackgroundTaskGenerationObservable(IObservable<T> source, Func<T, U> generator)
		{
			this._source = source;
			this._generator = generator;
		}

		private IObservable<T> _source;
		private Func<T, U> _generator;

		private readonly ReplaySubject<U> _replaySubject = new();

		enum ScriptRunState { Idle, Running, RunningButStale };
		private ScriptRunState _currentScriptRunState = ScriptRunState.Idle;

		private T? _pendingSource;

		async void LaunchScriptUpdate(T source)
		{
			var result = await Task.Run(
				() => _generator(source)
			);
			this._replaySubject.OnNext(result);
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Running:
					_currentScriptRunState = ScriptRunState.Idle;
					break;
				case ScriptRunState.RunningButStale:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(_pendingSource);
					break;
			}
		}

		void IObserver<T>.OnCompleted()
		{
		}

		void IObserver<T>.OnError(Exception error)
		{
		}

		void IObserver<T>.OnNext(T value)
		{
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Idle:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(value);
					break;
				case ScriptRunState.Running:
					_pendingSource = value;
					_currentScriptRunState = ScriptRunState.RunningButStale;
					break;
				case ScriptRunState.RunningButStale:
					_pendingSource = value;
					break;
			}
		}

		bool _hasSubscribed = false;

		IDisposable IObservable<U>.Subscribe(IObserver<U> observer)
		{
			var rv = this._replaySubject.Subscribe(observer);
			if(!_hasSubscribed)
			{
				this._source.Subscribe(this);
				_hasSubscribed = true;
			}
			return rv;
		}
	}

	public static class BackgroundTaskObservableExtensions
	{
		public static IObservable<U> TransformOnBackground<T, U>(this IObservable<T> source, Func<T, U> generator) =>
			new BackgroundTaskGenerationObservable<T, U>(source, generator);
	}
}
