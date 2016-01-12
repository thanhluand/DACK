using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;

namespace testGolomu.ViewModels
{
    class ClMenu : BindableBase
    {
        public ICommand IndexCommand { get; private set; }
       
        private string _Name1;
        public string Name1
        {
            get
            {
                return _Name1;
            }
            set
            {
                SetProperty(ref _Name1, value);
               // DataContext = new ChessBoard();
            }
        }
        
        private string _Name2;
        public string Name2
        {
            get
            {
                return _Name2;
            }
            set
            {
                SetProperty(ref _Name2, value);

            }
        }
        private Visibility _Visi;
        public Visibility Visi
        {
            get
            {
                return _Visi;
            }
            set
            {
                SetProperty(ref _Visi, value);

            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if(_isSelected == true )
                {
                    Index = 1;
                    Visi = Visibility.Visible;
                    Name1 = null;
                    Name2 = null;
                }
               
            }
        }
        private bool _isSelected1;
        public bool IsSelected1
        {
            get { return _isSelected1; }
            set
            {
                _isSelected1 = value;
                if (_isSelected1 == true)
                {
                    Visi = Visibility.Hidden;
                    Index = 2;
                    Name1 = null;
                    Name2 = null;
                }
               
            }
        }
        private bool _isSelected2;
        public bool IsSelected2
        {
            get { return _isSelected2; }
            set
            {
                _isSelected2 = value;
                if (_isSelected2 == true)
                {
                    Visi = Visibility.Hidden;
                    Index = 3;
                    Name1 = null;
                    Name2 = null;
                }
               
            }
        }
        private bool _isSelected3;
        public bool IsSelected3
        {
            get { return _isSelected3; }
            set
            {
                _isSelected3 = value;
                if (_isSelected3 == true)
                {
                    Visi = Visibility.Hidden;
                    Index = 4;
                    Name1 = null;
                    Name2 = null;
                }
               
            }
        }
        private int _Index;
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                SetProperty(ref _Index, value);
                //Change(_Index);
                
            }
        }
        public ClMenu()
        {

            IndexCommand = new DelegateCommand(ConfirmName);
           
            //ComputerCommand = new DelegateCommand(ComputerName);

        }

        //private void ComputerName()
        //{
        //    if (TBFName == "" || TBFName == null)
        //    {
        //        MessageBox.Show("Ban chua nhap Ten");
        //    }
        //    else
        //    {




        //        if (Click != null)
        //            Click(TBFName, 2);

        //    }
        //}

        private void ConfirmName()
        {
            if (Index == 1)
            {
                if (Name1 == "" || Name1 == null || Name2 == "" || Name2 == null)
                {
                    message("Ban chua nhap ten");

                    //  MessageBox.Show("Ban chua nhap Ten");
                }
                else
                {




                    if (Click != null)
                        Click(Name1, Name2, Index);

                }
            }
            if (Index == 2 || Index == 3 || Index ==4)
            {

                if (Name1 == "" || Name1 == null)
                {
                    message("Ban chua nhap ten");
                   // MessageBox.Show("Ban chua nhap Ten");
                }
                else
                {
                   



                    if (Click != null)
                        Click(Name1, "", Index);
                   

                }
            }
        }

        public delegate void ClickNewGame(string Player1, string Player2, int index);
        public event ClickNewGame Click;
        public delegate void ChangeIndex(int index);
        public event ChangeIndex Change;
        public delegate void ShowMessage(string message);
        public event ShowMessage message;
    }
}
