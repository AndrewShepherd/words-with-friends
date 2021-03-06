using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordsWithFriends.Gui
{
	/// <summary>
	/// Interaction logic for BoardView.xaml
	/// </summary>
	public partial class BoardView : UserControl
	{
		
		public BoardView()
		{
			InitializeComponent();
		}

		public static DependencyProperty BoardDependencyProperty = DependencyProperty.Register(
			nameof(Board),
			typeof(Board),
			typeof(BoardView),
			new()
			{
				PropertyChangedCallback = BoardView.BoardPropertyChangedCallback
			}
		);

		public static DependencyProperty SuggestedMoveDependencyProperty = DependencyProperty.Register(
			nameof(SuggestedMove),
			typeof(Move),
			typeof(BoardView),
			new()
			{ 
				PropertyChangedCallback = BoardView.SuggestedMovePropertyChangedCallback
			}
		);


		static void BoardPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var boardView = d as BoardView;
			if(boardView != null)
			{
				boardView.Board = e.NewValue as Board;
			}
		}

		static void SuggestedMovePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var boardView = d as BoardView;
			if(boardView != null)
			{
				boardView.SuggestedMove = e.NewValue as Move;
			}
		}

		private BoardViewModel? GetViewModel()
		{
			return this.Resources["ViewModel"] as BoardViewModel;
		}

		public Board? Board
		{
			get => GetViewModel()?.Board;
			set => (GetViewModel() ?? new()).Board = value;
		}

		public Move? SuggestedMove
		{
			get => GetViewModel()?.SuggestedMove;
			set => (GetViewModel() ?? new()).SuggestedMove = value;
		}

	}
}
