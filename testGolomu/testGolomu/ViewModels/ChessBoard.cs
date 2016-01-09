using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using Prism.Commands;

namespace testGolomu.ViewModels
{
    public class ChessSquare
    {
        public int Row { get; set; }

        public int Column { get; set; }
        public int IsBlack { get; set; }
        // private int _IsBlack;

        //   public int IsBlack { get { return (Row + Column) % 2 + 1; } set { _IsBlack = value; } }
    }
    public class Command<T> : ICommand
    {
        public Action<T> Action { get; set; }

        public void Execute(object parameter)
        {
            if (Action != null && parameter is T)
                Action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public Command(Action<T> action)
        {
            Action = action;
        }
    }

    public class ChessBoard : BindableBase
    {
        public List<ChessSquare> Squares { get;  set; }
      //  public BoxChat BC = new BoxChat();

        public Command<ChessSquare> SquareClickCommand { get;  set; }
        public ICommand ChatCommand { get; private set; }
        public ICommand ChangeCommand { get; private set; }
       public string  Name;
        private string _TBChat;
        public string TBChat
        {
            get
            {
                return _TBChat;
            }
            set
            {
                SetProperty(ref _TBChat, value);
            }
        }
        private string _TBName;
        public string TBName
        {
            get
            {
                return _TBName;
            }
            set
            {
                SetProperty(ref _TBName, value);
            }
        }


        public void ShowChat()
        {
            if (Name == "" || Name == null)
            {
                message("Ban chua nhap ten");
               // MessageBox.Show("Ban chua nhap Ten");
            }
            else
            {

                if (TBChat == "" || TBChat == null)
                {
                    //MessageBox.Show("Ban chua nhap noi dung");
                    message("Ban chua nhap noi dung");
                }
                else
                {
                    string Show = Name + ": " + TBChat;
                    TBChat = null;
                    if (ClickChat != null)
                        ClickChat(Show);
                }
            }
            
        }
        public ChessBoard(int[] ListIsblack)
        {
            Squares = new List<ChessSquare>();

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Squares.Add(new ChessSquare() { Row = i, Column = j,IsBlack = ListIsblack[i * 12 + j] });
                }
            }
            ChatCommand = new DelegateCommand(ShowChat);
            ChangeCommand = new DelegateCommand(ChangeName);
            SquareClickCommand = new Command<ChessSquare>(OnSquareClick);
        }
        public ChessBoard(int[] ListIsblack,string _Name)
        {
            Squares = new List<ChessSquare>();

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Squares.Add(new ChessSquare() { Row = i, Column = j, IsBlack = ListIsblack[i * 12 + j] });
                }
            }
            ChatCommand = new DelegateCommand(ShowChat);
            ChangeCommand = new DelegateCommand(ChangeName);
            SquareClickCommand = new Command<ChessSquare>(OnSquareClick);
            Name = _Name;
            TBName = _Name;
        }

        private void ChangeName()
        {
            Name = TBName;
        }

        private void OnSquareClick(ChessSquare square)
        {
           // MessageBox.Show("You clicked on Row: " + square.Row + " - Column: " + square.Column + " - asd:" + square.IsBlack);
            //int i = square.Row, j = square.Column;
            if (Click != null)
                Click(square.Row, square.Column, square.IsBlack,Name);

           // square.IsBlack = 11;
           // this.Squares.Clear();
            //ChessSquare sq = new ChessSquare() { Row = i, Column = j, IsBlack = 11 };
        
         
        
        }
       public delegate void  ClickChessSquare(int x, int y,int z, string name);
        public event ClickChessSquare Click;

        public delegate void ClickChatBox(string Show);
        public event ClickChatBox ClickChat;
        public delegate void ShowMessage(string message);
        public event ShowMessage message;
    }
}
