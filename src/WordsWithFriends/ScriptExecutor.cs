using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public static class ScriptExecutor
	{
		enum ScriptElementType
		{
			Word,
			Number
		}
		record ScriptElement(ScriptElementType Type, object Value);

		enum InitialParseState
		{
			None,
			ReadingKeyword,
			ReadingNumber
		}

		private static IEnumerable<ScriptElement> InitialParse(string script)
		{
			var state = InitialParseState.None;
			StringBuilder valueString = new();
			foreach(var c in script.Append(' '))
			{
				switch (state)
				{
					case InitialParseState.None:
						if (Char.IsDigit(c))
						{
							valueString.Append(c);
							state = InitialParseState.ReadingNumber;
						}
						else if (Char.IsLetter(c))
						{
							valueString.Append(c);
							state = InitialParseState.ReadingKeyword;
						}
						break;
					case InitialParseState.ReadingNumber:
						if(Char.IsDigit(c))
						{
							valueString.Append(c);
						}
						else
						{
							yield return new(ScriptElementType.Number, Int32.Parse(valueString.ToString()));
							valueString.Clear();
							state = InitialParseState.None;
						}
						break;
					case InitialParseState.ReadingKeyword:
						if(Char.IsLetter(c))
						{
							valueString.Append(c);
						}
						else
						{
							yield return new(ScriptElementType.Word, valueString.ToString());
							valueString.Clear();
							state = InitialParseState.None;
						}
						break;
				}
			}
		}

		private static int ConvertToInt(ScriptElement element)
		{
			if(element.Type != ScriptElementType.Number)
			{
				throw new Exception($"Expected a number, instead got '{element.Value}'");
			}
			return Convert.ToInt32(element.Value);
		}

		private static Direction ConvertToDirection(ScriptElement element)
		{
			if(element.Type != ScriptElementType.Word)
			{
				throw new Exception($"Expected a direction");
			}
			return ((string)element.Value).ToLower() switch
			{
				"down" => Direction.Down,
				"d" => Direction.Down,
				"across" => Direction.Across,
				"a" => Direction.Across,
				_ => throw new Exception($"Unrecognised direction \"{element.Value}\"")
			};
		}

		private static string ConvertToWord(ScriptElement element)
		{
			if(element.Type != ScriptElementType.Word)
			{
				throw new Exception($"Expected a word. Not ${element.Value}");
			}
			return (string)element.Value;
		}

		public static IEnumerable<Func<Board, Board>> GenerateActions(string script)
		{
			var elements = InitialParse(script).ToArray();
			int index = 0;
			while (index < elements.Length)
			{
				var element = elements[index];
				if (element.Type != ScriptElementType.Word)
				{
					throw new Exception("Expecting a keyword here");
				}
				var actionName = (string)element.Value;
				if (actionName.ToLowerInvariant() == "add")
				{
					int row = ConvertToInt(elements[++index]);
					int column = ConvertToInt(elements[++index]);
					Direction direction = ConvertToDirection(elements[++index]);
					String word = ConvertToWord(elements[++index]);
					yield return new Func<Board, Board>(
						(board) =>
						{
							board.Place(
								new WordPlacement
								{
									Position = new(row, column),
									Direction = direction,
									Word = word
								}
							);
							return board;
						}
					);

				}
				else if (actionName.ToLowerInvariant() == "wildcard")
				{
					int row = ConvertToInt(elements[++index]);
					int column = ConvertToInt(elements[++index]);
					yield return new Func<Board, Board>(
						(board) =>
						{
							var placedTile = board.TileAt(new(row, column));
							if(placedTile == default)
							{
								throw new Exception($"No tile placement at {row}, {column}");
							}
							board.Place(
								new TilePlacement(
									new(row, column),
									new(' ', placedTile.EffectiveChar)
								)
							);
							return board;
						}
					);
				}
				else
				{
					throw new Exception($"Unrecognized action keyword {actionName}");
				}
				++index;
			}
		}

		public static ExecuteScriptResult Run(
			Func<Board> createBoard,
			string script
		)
		{
			var board = createBoard();
			try
			{
				var actions = GenerateActions(script);
				foreach(var a in actions)
				{
					board = a(board);
				}
				return new(board, true, "Script ran succesfully");
			}
			catch(Exception ex)
			{
				return new(board, false, ex.Message);
			}
		}
			
	}
}
