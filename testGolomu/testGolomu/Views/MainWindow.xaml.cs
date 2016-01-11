using System;
using System.Windows;
using testGolomu.ViewModels;
using Prism;
using Prism.Mvvm;
using System.Windows.Media;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using Quobject.SocketIoClientDotNet.Client;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;

namespace testGolomu.Views
{
    /// <summary>
   
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket;
        public bool isNext = false;
        bool IsChangeName = true;
        int Index = 0;
       
        public int isPlayer = 0;
        public string Player1;
        public String Player2 = "";
        AIGomoku AI;
        ChessBoard cb;
        private BackgroundWorker worker = new BackgroundWorker();
        public BackgroundWorker ChatNetWord = new BackgroundWorker();
       
        public object Brushs { get; private set; }
       // int PointWin = -1;
        public MainWindow()
        {
             AI = new AIGomoku();
            InitializeComponent();
            menu mn = new menu();
           
            mn.RaiseCustomEvent += new EventHandler<menu.CustomEventArgs>(newWindow_RaiseCustomEvent);

            mn.ShowDialog();
            string chat = Name;
           

            if (Index == 2)
            {
                AI.InitiIsBlack();
              
                cb = new ChessBoard(AI._IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.ClickChange += ViewModel_ClickChange;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                DataContext = cb;
            }

            if (Index == 1)
            {
                AI.InitiIsBlack();
                
                TBName.Visibility = Visibility.Hidden;
                BTChangeName.Visibility = Visibility.Hidden;
                LBName.Visibility = Visibility.Hidden;
                Chattb.Visibility = Visibility.Hidden;
                Chat.Visibility = Visibility.Hidden;

                cb = new ChessBoard(AI._IsBlack, Player1);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.ClickChange += ViewModel_ClickChange;
                cb.message += ViewModels_Message;
                isPlayer = 4;
                DataContext = cb;
            }
            if (Index == 3)
            {
                AI.InitiIsBlack();
              
                cb = new ChessBoard(AI._IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.ClickChange += ViewModel_ClickChange;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_Net;
                worker.RunWorkerCompleted += worker_RunWorkerNetCompleted;
                DataContext = cb;
            }
            if (Index == 4)
            {
                AI.InitiIsBlack();
               
                cb = new ChessBoard(AI._IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.ClickChange += ViewModel_ClickChange;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_DoWork_Net;
                 worker.RunWorkerCompleted += worker_RunWorkerCompleted_Net;
               
                DataContext = cb;
            }
            if (Index == 0)
                this.Close();
            if (Index == 3 || Index == 4)
            {
                //var ConnStr = ConfigurationSettings.AppSettings["IPAddress"].ToString();
                socket = IO.Socket(ConfigurationSettings.AppSettings["IPAddress"].ToString());
                socket.On(Socket.EVENT_CONNECT, () =>
                {


                });
                socket.On(Socket.EVENT_MESSAGE, (data) =>
                {
                    Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });
                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                
                Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });
                socket.On("ChatMessage", (data) =>
                {
             
                Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                    if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                    {
                        socket.Emit("MyNameIs", Player1);
                        socket.Emit("ConnectToOtherPlayer");

                }
                    string s = data.ToString();
                    if (s.Contains("is now called"))
                    {
                        int n = s.IndexOf("is now called");
                        s = s.Substring(n + 14);
                        n = s.IndexOf("\"\r\n}");
                        s = s.Substring(0, n);
                        if (IsChangeName == true)
                        {
                           
                            Player1 = s;
                           
                           
                        }
                        else
                        {
                            Player2 = s;
                        }

                    }
                   
                    if (s.Contains("You are the first player!"))
                    {
                        
                        isNext = true;
                        string TName = data.ToString();
                        int n = TName.IndexOf(Player1);

                        TName = TName.Substring(n + 5 + Player1.Length);
                        n = TName.IndexOf("started a new game.");
                        TName = TName.Substring(0, n);
                        Player2 = TName;

                        if (Index == 4)
                        {
                            worker.RunWorkerAsync();
                        }
                    }
                    if (s.Contains("You are the second player!"))
                    {
                        isNext = false;
                        string TName = data.ToString();
                        int n = TName.IndexOf("and " + Player1 + " started a new game.");
                        TName = TName.Substring(0, n - 1);
                        TName = TName.Substring(17);
                        
                        Player2 = TName;
                    }
                   
                        


                });
                socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
              
            });
                socket.On("NextStepIs", (data) =>
                {
               
                Thread t = new Thread(() => NextStep("NextStepIs: " + data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                   });
            }
            else
                isNext = true;
        }

        private void ViewModel_ClickChange(string Show)
        {
            if(Index == 1 || Index ==2)
            {
                Player1 = Show;

            }
            else
            {
                IsChangeName = true;
                Player1 = Show;
                socket.Emit("MyNameIs", Player1);
            }
        }

        private void NextStep(string chat)
        {
            string[] numbers = Regex.Split(chat, @"\D+");
            int[] n = new int[4];
            int j = 0;
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    n[j] = int.Parse(value);
                    j++;
                }
            }
            int t = n[0];
            int x = n[1];
            int y = n[2];
            if (n[0] == 1)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        
                        AI.turn++;
                        if (Index == 3)
                        {
                            AI._IsBlack[x * 12 + y] = AI._IsBlack[x * 12 + y] * 10 + 8;
                            isPlayer = 8;


                            cb = new ChessBoard(AI._IsBlack, Player1);
                            cb.Click += ViewModels_Click;
                            cb.ClickChat += ViewModels_ClickChat;
                            cb.ClickChange += ViewModel_ClickChange;
                            cb.message += ViewModels_Message;
                            DataContext = cb;
                            if(t ==0)
                                SystemChat(x, y, isPlayer, Player1);
                            else
                                SystemChat(x, y, isPlayer, Player2);
                            AI.CheckWin( 8, AI._IsBlack);
                            
                            if (AI.isWin == true)
                            {

                                MessageBox.Show("white Win");
                                this.Hide();
                                MainWindow mn = new MainWindow();
                                mn.ShowDialog();
                                this.Close();
                            }
                            isNext = true;
                        }
                        if (Index == 4)
                        {
                            AI._IsBlack[x * 12 + y] = AI._IsBlack[x * 12 + y] * 10 + 4;
                            isPlayer = 4;


                            cb = new ChessBoard(AI._IsBlack, Player1);
                            cb.Click += ViewModels_Click;
                            cb.ClickChat += ViewModels_ClickChat;
                            cb.ClickChange += ViewModel_ClickChange;
                            cb.message += ViewModels_Message;
                            DataContext = cb;
                            SystemChat(x, y, isPlayer, Name);
                            AI.CheckWin( 4, AI._IsBlack);
                            if (AI.isWin == true)
                            {

                                MessageBox.Show("Black Win");
                                this.Hide();
                                MainWindow mn = new MainWindow();
                                mn.ShowDialog();
                                this.Close();
                            }
                            worker.RunWorkerAsync();

                        }
                    });
               
                    
            }
            

        }

        private void ChatNeWork(string chat)
        {
            int index = 2;
            if (chat.Contains("from"))
            {
                if (chat.Contains("\"from\": \"" + Player1 + "\""))
                    index = 1;
                else
                    index = 1;

                chat = chat.Substring(17);
                int n = chat.IndexOf("\"from\": \"");
               
                string Cname = chat.Substring(n+9);
               
                n = Cname.IndexOf("\r\n");
                Cname = Cname.Substring(0, n - 1);
                n = chat.IndexOf("\r\n");
                chat = chat.Substring(0, n - 2);
                chat = Cname  + ": " + chat;
            }
            if (chat.Contains("message"))
            {
                chat = chat.Substring(17);
                int n = chat.IndexOf("\"");
                chat = chat.Substring(0, chat.Length - 4);
                index = 2;
            }
            if (chat.Contains("<br />"))
            {
                int n = chat.IndexOf("<br />");
               chat = chat.Replace("<br />", "\n");
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate () {
                    TextBlock tb = new TextBlock();
                   
                    TextBlock tb2 = new TextBlock();
                   
                                
                    tb.Text = chat;
                    tb.TextWrapping = new TextWrapping();
                    
                    tb2.Text = DateTime.Now.ToString();
                    TextBlock tb3 = new TextBlock();
                    tb3.Text = "";
                    STChat.Children.Add(tb);
                    STChat.Children.Add(tb2);
                    STChat.Children.Add(tb3);
                    SVChat.ScrollToBottom();
                });
            
        }

        private void worker_RunWorkerCompleted_Net(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SystemChat(AI.PointWin / 12, AI.PointWin % 12, 4, Player1);
                isPlayer = (isPlayer + 1) % 2;
                AI.turn++;
                cb = new ChessBoard(AI._IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.ClickChange += ViewModel_ClickChange;
                cb.message += ViewModels_Message;
                DataContext = cb;
                
                AI.CheckWin( 8, AI._IsBlack);
                socket.Emit("MyStepIs", JObject.FromObject(new { row = AI.PointWin / 12, col = AI.PointWin % 12 }));
                if (AI.isWin == true)
                {

                    MessageBox.Show("White Win");
                    this.Hide();
                    MainWindow mn = new MainWindow();
                    mn.ShowDialog();
                    this.Close();
                }
            }));
        }

        private void worker_DoWork_Net(object sender, DoWorkEventArgs e)
        {
            isNext = false;
            AI.Computer();
            
           
        }

        private void worker_Net(object sender, DoWorkEventArgs e)
        {
           
        }

        private void worker_RunWorkerNetCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isNext = true;
        }

        private void ViewModels_Message(string message)
        {
            MessageBox.Show(message);
        }
        
       
        private void newWindow_RaiseCustomEvent(object sender, menu.CustomEventArgs e)
        {
            if (e.Title == 1)
            {
                Player1 = e.Message1;
                Player2 = e.Message2;
                Index = e.Title;
            }
            if (e.Title == 2 || e.Title == 3 || e.Title == 4)
            {
                Name = e.Message1;
                Player1 = e.Message1;

                Index = e.Title;
            }
        }
        

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
           
           ComputerChat(AI.PointWin / 12, AI.PointWin % 12);
            isPlayer = (isPlayer + 1) % 2;
            AI.turn++;
            cb = new ChessBoard(AI._IsBlack, Name);
            cb.Click += ViewModels_Click;
            cb.ClickChat += ViewModels_ClickChat;
            cb.ClickChange += ViewModel_ClickChange;
            cb.message += ViewModels_Message;
            DataContext = cb;
            AI.CheckWin( 8, AI._IsBlack);
            
            isNext = true;
            if (AI.isWin == true)
            {

                MessageBox.Show("White Win");
                this.Hide();
                MainWindow mn = new MainWindow();
                mn.ShowDialog();
                this.Close();
            }
            
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            isNext = false;
            AI.Computer();
        }

        private void ViewModels_ClickChat(string Show)
        {
            if (Show != null)
            {
                
                if (Index == 3 || Index == 4)
                {
                    Show = Show.Substring((Player1.Length + 1));
                    socket.Emit("ChatMessage", Show);
                }
              
                 if(Index ==1 || Index == 2)
                {
                    TextBlock lb = new TextBlock();
                    TextBlock lb2 = new TextBlock();
                    TextBlock lb3 = new TextBlock();
                    lb.Text = Show;
                    STChat.Children.Add(lb);
                    lb2.Text = DateTime.Now.ToString();
                    STChat.Children.Add(lb2);
                    lb3.Text = "";
                    STChat.Children.Add(lb3);
                    SVChat.ScrollToBottom();
                }
            }
           
        }
        public void ComputerChat(int x, int y)
        {
            if (Index == 4)
                return;
            TextBlock lb = new TextBlock();
            TextBlock lb2 = new TextBlock();
            TextBlock lb3 = new TextBlock();
            lb.Text = "Computers just go (" + x.ToString() + ";" + y.ToString() ;
            STChat.Children.Add(lb);
            lb2.Text = DateTime.Now.ToString();
            STChat.Children.Add(lb2);
            lb3.Text = "";
            STChat.Children.Add(lb3);
            SVChat.ScrollToBottom();
        }
        
     public void SystemChat(int x, int y, int z, String _Name)
        {
            TextBlock lb = new TextBlock();
            TextBlock lb2 = new TextBlock();
            TextBlock lb3 = new TextBlock();

            lb3.Text = "";
            lb.Text = _Name + ": Just go ("+ x +","+ y + ").";
            lb2.Text = DateTime.Now.ToString();
            STChat.Children.Add(lb);
            STChat.Children.Add(lb2);
            STChat.Children.Add(lb3);
            SVChat.ScrollToBottom();
        }
        private void ViewModels_Click(int x, int y,int z,string Name)
        {

            if (z == 1 || z == 2)
            {
                setIsblack(x, y, z,Name);
                
            }


        }

       

      


        
       public void setIsblack(int x, int y,int z,string Name)
        {
            if (isNext == true)
            {
                if (Index == 2)
                {
                    AI._IsBlack[x * 12 + y] = AI._IsBlack[x * 12 + y] * 10 + 4;
                    isPlayer = 4;
                    cb = new ChessBoard(AI._IsBlack, Name);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                    cb.ClickChange += ViewModel_ClickChange;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    AI.CheckWin( 4, AI._IsBlack);
                    if (AI.isWin == true)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    AI.PointWin = -1;
                    isPlayer = (isPlayer + 1) % 2;
                    AI.turn++;
                    worker.RunWorkerAsync();

                }
                if (Index == 1)
                {
                    AI._IsBlack[x * 12 + y] = AI._IsBlack[x * 12 + y] * 10 + isPlayer;
                    if(isPlayer == 4)
                        cb = new ChessBoard(AI._IsBlack, Player2);
                    if (isPlayer == 8)
                        cb = new ChessBoard(AI._IsBlack, Player1);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                    cb.ClickChange += ViewModel_ClickChange;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    AI.CheckWin(isPlayer, AI._IsBlack);
                    if (AI.isWin == true && isPlayer == 4)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    if (AI.isWin == true && isPlayer == 8)
                    {

                        MessageBox.Show("White Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    
                    isPlayer = ((isPlayer / 4) % 2 + 1) * 4;
                }
                if(Index ==3)
                {
                    AI._IsBlack[x * 12 + y] = AI._IsBlack[x * 12 + y] * 10 + 4;
                    isPlayer = 4;
                    cb = new ChessBoard(AI._IsBlack, Name);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                      cb.ClickChange += ViewModel_ClickChange;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    AI.CheckWin( 4, AI._IsBlack);
                    socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));
                    if (AI.isWin == true)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    AI.PointWin = -1;
                    isPlayer = (isPlayer + 1) % 2;
                    AI.turn++;
                    isNext = false;
                   
                }
                
            }
           
        }
      
    }
}
