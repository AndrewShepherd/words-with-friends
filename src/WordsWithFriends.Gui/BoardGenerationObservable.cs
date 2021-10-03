using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends.Gui
{
	record BoardGenerationFields(BoardType BoardType, string Script);

	class BoardGenerationObservable : IObserver<BoardGenerationFields>, IObservable<Board>
	{
		enum ScriptRunState { Idle, Running, RunningButStale };
		private ScriptRunState _currentScriptRunState = ScriptRunState.Idle;

		private BoardGenerationFields _pendingBoardGeneration = new(BoardType.Small, string.Empty);


		private readonly ReplaySubject<Board> _replaySubject;

		async void LaunchScriptUpdate(BoardGenerationFields boardGenerationFields)
		{
			var result = await Task.Run(
				() => ScriptExecutor.Run(
					() => boardGenerationFields.BoardType == BoardType.Large
						? BoardBuilder.ConstructLargeBoard()
						: BoardBuilder.ConstructSmallBoard(),
					boardGenerationFields.Script
				)
			);
			this._replaySubject.OnNext(result.Board);
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Running:
					_currentScriptRunState = ScriptRunState.Idle;
					break;
				case ScriptRunState.RunningButStale:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(_pendingBoardGeneration);
					break;
			}
		}

		public BoardGenerationObservable()
		{
			this._replaySubject = new ReplaySubject<Board>();
		}

		public void OnCompleted()
		{
			this._replaySubject.OnCompleted();
		}

		public void OnError(Exception error)
		{
			this._replaySubject.OnError(error);
		}

		public void OnNext(BoardGenerationFields value)
		{
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Idle:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(value);
					break;
				case ScriptRunState.Running:
					_pendingBoardGeneration = value;
					_currentScriptRunState = ScriptRunState.RunningButStale;
					break;
				case ScriptRunState.RunningButStale:
					_pendingBoardGeneration = value;
					break;
			}
		}

		public IDisposable Subscribe(IObserver<Board> observer)
		{
			return ((IObservable<Board>)_replaySubject).Subscribe(observer);
		}
	}
}
