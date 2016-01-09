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
using System.Windows.Shapes;
using testGolomu.ViewModels;

namespace testGolomu.Views
{
    /// <summary>
    /// Interaction logic for menu.xaml
    /// </summary>
    public partial class menu : Window
    {
        ClMenu mn;
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;
        public menu()
        {
            InitializeComponent();
            mn = new ClMenu();
            mn.IsSelected = true;
         //   mn.IsSelected = true;
         //   mn.Visi = Visibility.Hidden;
            mn.Click += ViewModels_Click;
            mn.Change += ViewModels_Change;
            mn.message += ViewModels_Message;
            DataContext = mn;

            RBHuman.IsChecked = true;
        }

        private void ViewModels_Message(string message)
        {
            MessageBox.Show(message);
        }

        private void ViewModels_Change(int index)
        {
            if(index == 1)
            {
                LBPlayer1.Content = "Player1";
                
            }
            if(index == 2)
            {
                LBPlayer1.Content = "Player";
              
               this.UpdateLayout();
              
            }
        }

        public String NamePlayer1;
        public String NamePlayer2;
        public int Index;
        private void ViewModels_Click(string Player1, String Player2 , int _index)
        {
            NamePlayer1 = Player1;
            NamePlayer2 = Player2;
            Index = _index;
            RaiseCustomEvent(this, new CustomEventArgs(NamePlayer1, NamePlayer2, Index));
            this.Close();
            //MainWindow mn = new MainWindow();
            //mn.ShowDialog();
        }

        
        public class CustomEventArgs : EventArgs
        {
            public CustomEventArgs(string s,string s2,int Title )
            {
                msg1 = s;
                _Title = Title;
                msg2 = s2;
            }
            private string msg1;
            public string Message1
            {
                set { msg1 = value; }
                get { return msg1; }
            }
            private string msg2;
            public string Message2
            {
                set { msg2 = value; }
                get { return msg2; }
            }

            private int _Title;
            public int Title
            {
                set { _Title = value; }
                get { return _Title; }
            }
        }


    }
}
