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

namespace testGolomu.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessBoard cb;
        private BackgroundWorker worker = new BackgroundWorker();
        public BackgroundWorker ChatNetWord = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            menu mn = new menu();
           // Label LBSI = new Label();
         //   LBSI.Content = "Connected";
            // mn.RaiseCustomEvent += new EventHandler<CustomEventArgs>(newWindow_RaiseCustomEvent);
            mn.RaiseCustomEvent += new EventHandler<menu.CustomEventArgs>(newWindow_RaiseCustomEvent);

            mn.ShowDialog();
            string chat = Name;
           

            if (Index == 2)
            {
                InitiIsBlack();
                // MessageBox.Show("sdf");
                cb = new ChessBoard(_IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                DataContext = cb;
            }

            if (Index == 1)
            {
                InitiIsBlack();
                // MessageBox.Show("sdf");
                TBName.Visibility = Visibility.Hidden;
                BTChangeName.Visibility = Visibility.Hidden;
                LBName.Visibility = Visibility.Hidden;
                Chattb.Visibility = Visibility.Hidden;
                Chat.Visibility = Visibility.Hidden;

                cb = new ChessBoard(_IsBlack, Player1);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.message += ViewModels_Message;
                isPlayer = 4;
                DataContext = cb;
            }
            if (Index == 3)
            {
                InitiIsBlack();
               // MessageBox.Show("sdf");
                cb = new ChessBoard(_IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_Net;
                worker.RunWorkerCompleted += worker_RunWorkerNetCompleted;
                DataContext = cb;
            }
            if (Index == 4)
            {
                InitiIsBlack();
                // MessageBox.Show("sdf");
                cb = new ChessBoard(_IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.message += ViewModels_Message;
                worker.DoWork += worker_DoWork_Net;
                 worker.RunWorkerCompleted += worker_RunWorkerCompleted_Net;
                //worker.DoWork += worker_DoWork;
               // worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                DataContext = cb;
            }
            if (Index == 0)
                this.Close();
            if (Index == 3 || Index == 4)
            {
                socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
                socket.On(Socket.EVENT_CONNECT, () =>
                {

                // ViewModels_ClickChat("Connected");
                //Label lb = new Label();

                //MessageBox.Show("connected");


                //Thread t = new Thread(() => ChatNeWork("Connected"));
                //    t.SetApartmentState(ApartmentState.STA);
                //    t.Start();

                });
                socket.On(Socket.EVENT_MESSAGE, (data) =>
                {
                //Console.WriteLine(data);
                // MessageBox.Show(data.ToString());
                Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });
                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                // Console.WriteLine(data);
                // MessageBox.Show(data.ToString());
                Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                });
                socket.On("ChatMessage", (data) =>
                {
                //Console.WriteLine(data);
                //MessageBox.Show(data.ToString());
                Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                    if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                    {
                        socket.Emit("MyNameIs", Player1);
                        socket.Emit("ConnectToOtherPlayer");

                    //Console.ReadKey(intercept: true);

                }
                    
                    string s = data.ToString();
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
                        int n = TName.IndexOf(Player1);
                        TName = TName.Substring(0, n - 5);
                        TName = TName.Substring(17);
                        
                        Player2 = TName;
                    }
                   
                        


                });
                socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    Thread t = new Thread(() => ChatNeWork(data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                //Console.WriteLine(data);
                // MessageBox.Show(data.ToString());
            });
                socket.On("NextStepIs", (data) =>
                {
                //Console.WriteLine("NextStepIs: " + data);
                // MessageBox.Show("NextStepIs: " + data);

                Thread t = new Thread(() => NextStep("NextStepIs: " + data.ToString()));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                   // SystemChat(PointWin / 12, PointWin % 12, 8, Player1);

                    //Thread t2 = new Thread(() => ChatNeWork("Enter to make your move"));
                    //t2.SetApartmentState(ApartmentState.STA);
                    //t2.Start();

                });
            }
            else
                isNext = true;
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
                        //Label lb = new Label();
                        //Label lb2 = new Label();
                        //lb2.Background = Brushes.Blue;
                        //lb.Background = Brushes.Blue;
                        //lb.Content = chat;
                        //lb2.Content = DateTime.Now;
                        //STChat.Children.Add(lb);
                        //STChat.Children.Add(lb2);

                        turn++;
                        if (Index == 3)
                        {
                            _IsBlack[x * 12 + y] = _IsBlack[x * 12 + y] * 10 + 8;
                            isPlayer = 8;


                            cb = new ChessBoard(_IsBlack, Player1);
                            cb.Click += ViewModels_Click;
                            cb.ClickChat += ViewModels_ClickChat;
                            cb.message += ViewModels_Message;
                            DataContext = cb;
                            if(t ==0)
                                SystemChat(x, y, isPlayer, Player1);
                            else
                                SystemChat(x, y, isPlayer, Player2);
                            CheckWin( 8, _IsBlack);
                            if (isWin == true)
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
                            _IsBlack[x * 12 + y] = _IsBlack[x * 12 + y] * 10 + 4;
                            isPlayer = 4;


                            cb = new ChessBoard(_IsBlack, Player1);
                            cb.Click += ViewModels_Click;
                            cb.ClickChat += ViewModels_ClickChat;
                            cb.message += ViewModels_Message;
                            DataContext = cb;
                            SystemChat(x, y, isPlayer, Name);
                            CheckWin( 4, _IsBlack);
                            if (isWin == true)
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
                //  MessageBox.Show("\"");
                string Cname = chat.Substring(n+9);
                //Cname = Cname.Substring(0, Cname.Length - 2);
                n = Cname.IndexOf("\r\n");
                Cname = Cname.Substring(0, n - 1);
                //chat = chat.Trim();
                
              //  chat = chat.Substring(0, n - 2);
                n = chat.IndexOf("\r\n");
                chat = chat.Substring(0, n - 2);
               // MessageBox.Show(chat.Length.ToString() + " " + Cname.Length.ToString());
                chat = Cname  + ": " + chat;
            }
            if (chat.Contains("message"))
            {
                chat = chat.Substring(17);
                //MessageBox.Show(chat);
                int n = chat.IndexOf("\"");
                //MessageBox.Show("\"");
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
                   // tb.IsEnabled = false;
                    tb.TextWrapping = new TextWrapping();
                    
                    tb2.Text = DateTime.Now.ToString();
                    TextBlock tb3 = new TextBlock();
                    tb3.Text = "";
                    
                   // lb.Content = lb.Content.ToString().Replace(Environment.NewLine, "\n");
                    STChat.Children.Add(tb);
                    STChat.Children.Add(tb2);
                    STChat.Children.Add(tb3);
                });
            
        }

        private void worker_RunWorkerCompleted_Net(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SystemChat(PointWin / 12, PointWin % 12, 4, Player1);
                //ComputerChat(PointWin / 12, PointWin % 12);
                isPlayer = (isPlayer + 1) % 2;
                turn++;
                cb = new ChessBoard(_IsBlack, Name);
                cb.Click += ViewModels_Click;
                cb.ClickChat += ViewModels_ClickChat;
                cb.message += ViewModels_Message;
                DataContext = cb;
                
                CheckWin( 8, _IsBlack);
              //  isNext = true;
                socket.Emit("MyStepIs", JObject.FromObject(new { row = PointWin / 12, col = PointWin % 12 }));
                if (isWin == true)
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
            Computer();
           
        }

        private void worker_Net(object sender, DoWorkEventArgs e)
        {
           
        }

        private void worker_RunWorkerNetCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

           // isPlayer = 8;
           // SystemChat(PointWin / 12, PointWin % 12, isPlayer, Player1);
            isNext = true;
        }

        private void ViewModels_Message(string message)
        {
            MessageBox.Show(message);
        }

        // string message = "";
        Socket socket;
        public bool isNext = false;
        Point LastGo;
        int Index = 0;
        bool isWin = false;
        public int isPlayer = 0;
        public string Player1;
        public String Player2= "";
        private void newWindow_RaiseCustomEvent(object sender, menu.CustomEventArgs e)
        {
            // this.Title = e.Message;
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

        //private void newWindow_RaiseCustomEvent(object sender, CustomEventArgs e)
        //{
        //    this.Title = e.Message;
        //}

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // NetwordChat();
           
           ComputerChat(PointWin / 12, PointWin % 12);
            isPlayer = (isPlayer + 1) % 2;
            turn++;
            cb = new ChessBoard(_IsBlack, Name);
            cb.Click += ViewModels_Click;
            cb.ClickChat += ViewModels_ClickChat;
            cb.message += ViewModels_Message;
            DataContext = cb;
            CheckWin( 8, _IsBlack);
            isNext = true;
            if (isWin == true)
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
            Computer();
            // Sleep(500);
           // Thread.Sleep(5000);
        }

        private void ViewModels_ClickChat(string Show)
        {
            if (Show != null)
            {
                // TextRange = new TextRange(BChat.Document.ContentEnd, richTextBox1.Document.ContentEnd)
                //  TextRange range = new TextRange(BChat.Document.ContentEnd, BChat.Document.ContentEnd);
                //  range.Text = Show;
                //  range.ApplyPropertyValue(TextElement.ForegroundProperty, Colors.Red);
                //  BChat.SelectionBrush = Brushes.Red;
              //  BChat.Foreground = Brushes.Red;
              //  BChat.AppendText(Show + "\n");


                //Label lb = new Label();
                //Label lb2 = new Label();
                //lb2.Background = Brushes.Blue;
                //lb.Background = Brushes.Blue;
               
                //    lb.Content = Show;
                //    lb2.Content = DateTime.Now;
                //    STChat.Children.Add(lb);
                //    STChat.Children.Add(lb2);
                
                if (Index == 3 || Index == 4)
                {
                    Show = Show.Substring((Player1.Length + 1));
                    socket.Emit("ChatMessage", Show);
                }
                // BChat.FindName(Show + "\n");
                 // BChat.SelectionBrush = Brushes.Red;
                 if(Index ==1 || Index == 2)
                {
                    TextBlock lb = new TextBlock();
                    TextBlock lb2 = new TextBlock();
                    TextBlock lb3 = new TextBlock();
                    // lb.Background = Brushes.Red;
                    //lb2.Background = Brushes.Red;
                    lb.Text = Show;
                    STChat.Children.Add(lb);
                    lb2.Text = DateTime.Now.ToString();
                    STChat.Children.Add(lb2);
                    lb3.Text = "";
                    STChat.Children.Add(lb3);
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
            // lb.Background = Brushes.Red;
            //lb2.Background = Brushes.Red;
            lb.Text = "Computers just go (" + x.ToString() + ";" + y.ToString() ;
            STChat.Children.Add(lb);
            lb2.Text = DateTime.Now.ToString();
            STChat.Children.Add(lb2);
            lb3.Text = "";
            STChat.Children.Add(lb3);
        }
        public void NetwordChat( )
        {
            Label lb = new Label();
            lb.Background = Brushes.Red;
            lb.Content = "adasada";
            STChat.Children.Add(lb);


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
            
            
        }
        private void ViewModels_Click(int x, int y,int z,string Name)
        {

            if (z == 1 || z == 2)
            {
                setIsblack(x, y, z,Name);
                
            }


        }

       

        private void SetClickOtherPlayer(int x , int y)
        {
            
        }


        int[] _IsBlack = new int[144];
        int turn = 0;
        public object Brushs { get; private set; }
        int PointWin = -1;
       public void setIsblack(int x, int y,int z,string Name)
        {
            if (isNext == true)
            {
                if (Index == 2)
                {
                    _IsBlack[x * 12 + y] = _IsBlack[x * 12 + y] * 10 + 4;
                    isPlayer = 4;
                    cb = new ChessBoard(_IsBlack, Name);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    CheckWin( 4, _IsBlack);
                    if (isWin == true)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    PointWin = -1;
                    isPlayer = (isPlayer + 1) % 2;
                    turn++;
                    // Computer();
                    worker.RunWorkerAsync();

                }
                if (Index == 1)
                {
                    _IsBlack[x * 12 + y] = _IsBlack[x * 12 + y] * 10 + isPlayer;
                    if(isPlayer == 4)
                        cb = new ChessBoard(_IsBlack, Player2);
                    if (isPlayer == 8)
                        cb = new ChessBoard(_IsBlack, Player1);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    CheckWin(isPlayer, _IsBlack);
                    if (isWin == true && isPlayer == 4)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    if (isWin == true && isPlayer == 8)
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
                    _IsBlack[x * 12 + y] = _IsBlack[x * 12 + y] * 10 + 4;
                    isPlayer = 4;
                    cb = new ChessBoard(_IsBlack, Name);
                    cb.Click += ViewModels_Click;
                    cb.ClickChat += ViewModels_ClickChat;
                    cb.message += ViewModels_Message;
                    DataContext = cb;
                    SystemChat(x, y, isPlayer, Name);
                    CheckWin( 4, _IsBlack);
                    socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));
                    if (isWin == true)
                    {

                        MessageBox.Show("Den Win");
                        this.Hide();
                        MainWindow mn = new MainWindow();
                        mn.ShowDialog();
                        this.Close();
                    }
                    PointWin = -1;
                    isPlayer = (isPlayer + 1) % 2;
                    turn++;
                    // Computer();
                    isNext = false;
                   
                }
                
            }
           
        }
      public  void InitiIsBlack()
        {
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                    _IsBlack[i * 12 + j] = ((i + j) % 2) + 1;
        }

        public void CheckWin(  int _Player,int[] IsBlack)
        {
           
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                {
                    if (isWin == true)
                        break;
                    if (IsBlack[i * 12 + j] > 2 && IsBlack[i * 12 + j] % 10 == _Player)
                    {
                        if (isWin == true)
                            break;
                        int PointChess = i * 12 + j;
                        int Value = IsBlack[PointChess];
                        if (j < 12 - 4)
                            if ((IsBlack[PointChess + 1] % 10 == Value % 10) && (IsBlack[PointChess + 2] % 10 == Value % 10) && (IsBlack[PointChess + 3] % 10 == Value % 10) && (IsBlack[PointChess + 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8)
                            if ((IsBlack[PointChess + 12] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8 && j < 8)
                            if ((IsBlack[PointChess + 12 + 1] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3 + 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4 + 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8 && j > 3)
                            if ((IsBlack[PointChess + 12 - 1] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3 - 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4 - 4] % 10 == Value % 10))
                                isWin = true;
                    }
                }
           
        }

        public void CopyChessBoard(int[] VirtualChess)
        {
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                    VirtualChess[i * 12 + j] = _IsBlack[i * 12 + j];
        }
        public void CheckUpcomingWin(int z)
        {
            if (isWin == true)
                return;
            // checUpconmingWin
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && isWin == false; i++)
                for (int j = 0; j < 12 && isWin == false; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        CheckWin(z, VirtualChess);
                        if (isWin == true)
                        {
                           // MessageBox.Show(" sap win");
                            PointWin = i * 12 + j;
                            isWin = false;
                            return;
                        }
                    }
                }
            isWin = false;
        }
        // kiem tra  3 quan khong bi chan
        public void Checkthreeopen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for(int k = 0; k < 12 && PointTrue == -1; k++)
                            for(int l = 0; l<12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {
                                   
                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && (VirtualChess[PointChess + 4] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4] < 3) &&  (VirtualChess[PointChess - 12 ] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4 + 4] < 3) && (VirtualChess[PointChess - 12  - 1] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4 - 4] < 3) && (VirtualChess[PointChess - 12  +1] < 3))
                                            PointTrue = Value;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            PointWin = i * 12 + j;
                            //MessageBox.Show("Co 3 quan k bi chan");
                            return;
                        }
                    }
                }
        }
        // kiem tra 2* quan lien tiep 
      public  void CheckDoubleTwoOpen( int z)
        {

            int PointTrue = 0;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == 0; i++)
                for (int j = 0; j < 12 && PointTrue == 0; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12; k++)
                            for (int l = 0; l < 12 ; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {
                                    
                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 9 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 3] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue ++;
                                    if (k < 9 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 3] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue ++;
                                    if (k < 9 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 3 + 3] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue++;
                                    if (k < 9 && l > 2 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 3 - 3] < 3) && (VirtualChess[PointChess - 12  + 1] < 3))
                                            PointTrue ++;

                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 4] < 3) || (VirtualChess[PointChess - 1] < 3)))
                                            PointTrue++;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4] < 3) || (VirtualChess[PointChess - 12] < 3)))
                                            PointTrue++;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 + 4] < 3) || (VirtualChess[PointChess - 12 - 1] < 3)))
                                            PointTrue++;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 - 4] < 3) || (VirtualChess[PointChess - 12 + 1] < 3)))
                                            PointTrue++;
                                }
                            }
                        if (PointTrue > 1)
                        {
                           // MessageBox.Show("Co 2 * 2 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                        else
                            PointTrue = 0;
                    }
                }
        }


      //  Check 2* quan mgac
        //public void CheckTwoOpenVSThreeClose(int z)
        //{
        //    int PointTrue = 0;
        //    int[] VirtualChess = new int[144];
        //    CopyChessBoard(VirtualChess);
        //    for (int i = 0; i < 12 && PointTrue == 0; i++)
        //        for (int j = 0; j < 12 && PointTrue == 0; j++)
        //        {
        //            CopyChessBoard(VirtualChess);
        //            if (VirtualChess[i * 12 + j] < 3)
        //            {
        //                VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
        //                for (int k = 0; k < 12; k++)
        //                    for (int l = 0; l < 12; l++)
        //                    {
        //                        if (VirtualChess[k * 12 + l] % 10 == z)
        //                        {
        //                            int 12 = 12;
        //                            int PointChess = k * 12 + l;
        //                            int Value = VirtualChess[k * 12 + l];
        //                            if (l < 12 - 4 && l > 0)
        //                                if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] < 3) && (VirtualChess[PointChess - 1] < 3))
        //                                    PointTrue++;
        //                            if (k < 8 && k > 0)
        //                                if ((VirtualChess[PointChess + NumRow] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 2] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 3] < 3) && (VirtualChess[PointChess - NumRow] < 3))
        //                                    PointTrue++;
        //                            if (k < 8 && l < 8 && l > 0 && k > 0)
        //                                if ((VirtualChess[PointChess + NumRow + 1] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 3 + 3] < 3) && (VirtualChess[PointChess - NumRow - 1] < 3))
        //                                    PointTrue++;
        //                            if (k < 8 && l > 3 && k > 0 && l < 11)
        //                                if ((VirtualChess[PointChess + NumRow - 1] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + NumRow * 3 - 3] < 3) && (VirtualChess[PointChess - NumRow + 1] < 3))
        //                                    PointTrue++;
        //                        }
        //                    }
        //                if (PointTrue > 1)
        //                    MessageBox.Show("Co 2 * 2 quan k bi chan");
        //                else
        //                    PointTrue = 0;
        //            }
        //        }
        //}

        public void CheckOneChessOpen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {
                                    
                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 10 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 3] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l < 10 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l > 1 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 3 - 3] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue = z;
                                }
                            }
                        if (PointTrue > 0)
                        {
                           // MessageBox.Show("Co 1 duong 2 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                    }
                }
        }
        // 1 duong 3 bi chan 1 dau 
        public void CheckthreeChessClose(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {
                                    
                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 4] < 3) || (VirtualChess[PointChess - 1] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4] < 3) || (VirtualChess[PointChess - 12] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 + 4] < 3) || (VirtualChess[PointChess - 12 - 1] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 - 4] < 3) || (VirtualChess[PointChess - 12 + 1] < 3)))
                                            PointTrue = Value;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            PointWin = i * 12 + j;
                           // MessageBox.Show("Co 3 quan  bi chan");
                            return;
                        }
                    }
                }
        }
        public void CheckChessOpen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {
                                    
                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 10 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10)  && (VirtualChess[PointChess + 2] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 2] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l < 10 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l > 1 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10)  && (VirtualChess[PointChess + 12 * 2 - 2] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue = z;
                                }
                            }
                        if (PointTrue > 0)
                        {
                          //  MessageBox.Show("Co 1 duong 1 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                    }
                }
        }


        public void Computer()
        {
           // if (isWin == true)
           //     return;
             PointWin = -1;
            if(turn == 0)
            {
                _IsBlack[6 * 12 + 6] = _IsBlack[6 * 12 + 6] * 10 + 8;
                PointWin = 6 * 12 + 6;
                return;
            }
            if (turn == 1)
            {
                for (int i = 0; i < 12; i++)
                    for (int j = 0; j < 12; j++)
                    {
                        if (_IsBlack[i * 12 + j] % 10 == 4)
                        {
                            _IsBlack[i * 12 + 12 + j + 1] = _IsBlack[i * 12 + 12 + j + 1] * 10 + 8;
                            PointWin = i * 12 + 12 + j + 1;
                            return;
                        }
                    }
            }
            if (turn == 2)
            {
                if(_IsBlack[7 *12 + 5] < 3)
                {
                    _IsBlack[7 * 12 + 5] = _IsBlack[7 * 12 + 5]*10 + 8 ;
                    PointWin = 7 * 12 + 5;
                    return;
                }
                if (_IsBlack[5 * 12 + 7] < 3)
                {
                    _IsBlack[5 * 12 + 7] = _IsBlack[5 * 12 + 7]*10 + 8 ;
                    PointWin = 5 * 12 + 7;
                    return;
                }
              
            }
            if(turn == 3)
            {
                for (int i = 0; i< 12; i++)
                    for( int j = 0; j< 12; j++)
                    {
                        if(_IsBlack[i* 12 + j]% 10 == 8 )
                        {
                            if (_IsBlack[(i + 1) * 12 + j + 1] < 3)
                            {
                                _IsBlack[(i + 1) * 12 + j + 1] = _IsBlack[(i + 1) * 12 + j + 1]*10 + 8;
                                PointWin = ((i + 1) * 12 + j + 1);
                                
                                return;
                            }
                            if (_IsBlack[(i - 1) * 12 + j + 1] < 3)
                            {
                                _IsBlack[(i - 1) * 12 + j + 1] = _IsBlack[(i - 1) * 12 + j + 1]*10 + 8 ;
                                PointWin = (i - 1) * 12 + j + 1;
                                return;
                            }
                        }
                    }
            }
            if (turn == 4)
            {
                if (_IsBlack[6 * 12 + 5] < 3)
                {
                    _IsBlack[6 * 12 + 5] = _IsBlack[6 * 12 + 5]*10 + 8;
                    PointWin = 6 * 12 + 5;
                    LastGo.X = 6;
                    LastGo.Y = 5;
                    return;
                }
                if (_IsBlack[5 * 12 + 6] < 3)
                {
                    _IsBlack[5 * 12 + 6] = _IsBlack[5 * 12 + 6]*10 + 8 ;
                    PointWin = 5 * 12 + 6;
                    return;
                }

            }
            

            // checUpconmingWin

            CheckUpcomingWin(8 );
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 +8;
                
                return;
            }
            CheckUpcomingWin(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                
                return;
            }



            // chex 3 k bi chan
            Checkthreeopen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            Checkthreeopen( 4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            // 2 duong 3
            CheckDoubleTwoOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            CheckDoubleTwoOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            // 3 bi chan //2 quan k bi chan
            CheckthreeChessClose(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            CheckOneChessOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }


            CheckthreeChessClose( 4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            CheckOneChessOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            



            // 1 quan k bi chan
            CheckChessOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            CheckChessOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            // 1 quan bi chan
            for (int i = 1; i < 11; i++)
                for (int j = 1; j < 11; j++)
                {
                    bool Check = false;
                    if (_IsBlack[i * 12 + j] % 10 == 8)
                    {
                        for (int k = j; k < 11; k++)
                        {
                            if (_IsBlack[i * 12 + k] > 2)
                            {
                                Check = false;
                                break;
                            }
                            Check = true;

                        }
                        if (Check == true)
                        {
                            _IsBlack[i * 12 + j + 1] = _IsBlack[i * 12 + j + 1] * 10 + 8;
                            return;
                        }



                        for (int k = i; k < 11; k++)
                        {
                            if (_IsBlack[k * 12 + j] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + j] = _IsBlack[(i + 1) * 12 + j] * 10 + 8;
                            return;
                        }




                        for (int k = i, l = j; k < 11 && l < 11; k++, l++)
                        {
                            if (_IsBlack[k * 12 + l] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + (j + 1)] = _IsBlack[(i + 1) * 12 + j + 1] * 10 + 8;
                            return;
                        }



                        for (int k = i, l = j; k < 11 && j < 11; k++, l++)
                        {
                            if (_IsBlack[k * 12 + l] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + j - 1] = _IsBlack[(i + 1) * 12 + j -1] * 10 + 8;
                            return;
                        }

                    }
                }
        

                    while(true)
                    {
                        int n, m;
                        Random random = new Random();
                        n = random.Next(0, 11);
                        m = random.Next(0, 11);
                        if(_IsBlack[n*12 + m] < 3)
                        {
                            _IsBlack[n * 12 + m] = _IsBlack[n * 12 + m] + 8;
                   // MessageBox.Show("random");
                            return;
                        }
                    }
            //VirtualChess[1] = 1;
            // VirtualChess = _IsBlack;
            //int NumRow = 12;
            //for (int i = 0; i < 12; i++)
            //    for (int j = 0; j < 12; j++)
            //        VirtualChess[i * 12 + j] = _IsBlack[i * 12 + j];

            //int _isWin = 0;
            //for (int i = 0; i < 12; i++)
            //{
            //    if (isWin != 0)
            //        break;
            //    for (int j = 0; j < 12; j++)
            //        if (_IsBlack[i * 12 + j] > 2)
            //        {
            //            if (isWin != 0)
            //                break;
            //            int PointChess = i * 12 + j;
            //            int Value = _IsBlack[PointChess];
            //            if (j < 9)
            //                if ((_IsBlack[PointChess + 1] % 10 == Value % 10) && (_IsBlack[PointChess + 2] % 10 == Value % 10) && (_IsBlack[PointChess + 3] % 10 == Value % 10) && ((j < 8 && _IsBlack[PointChess + 4] < 3) || (j > 0 && _IsBlack[PointChess - 1] < 3)))
            //                {
            //                    isWin = 1;
            //                }

            //            if (i < 9 && isWin != 1)
            //                if ((_IsBlack[PointChess + NumRow] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 2] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 3] % 10 == Value % 10) && ((i < 8 && _IsBlack[PointChess + NumRow * 4] < 3) || (i > 0 && _IsBlack[PointChess - NumRow] < 3)))
            //                {
            //                    isWin = 1;
            //                }
            //            if (i < 9 && j < 9 && isWin != 1)
            //                if ((_IsBlack[PointChess + NumRow + 1] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 2 + 2] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 3 + 3] % 10 == Value % 10) && ((i < 8 && j < 8 && _IsBlack[PointChess + NumRow * 4 + 4] < 3) || (i > 0 && j > 0 && _IsBlack[PointChess - NumRow - 1] < 3)))
            //                {
            //                    isWin = 1;
            //                }
            //            if (i < 9 && j > 2 && isWin != 1)
            //                if ((_IsBlack[PointChess + NumRow - 1] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 2 - 2] % 10 == Value % 10) && (_IsBlack[PointChess + NumRow * 3 - 3] % 10 == Value % 10) && ((i < 8 && j > 3 && _IsBlack[PointChess + NumRow * 4 - 4] < 3) || (i > 0 && j < 8 && _IsBlack[PointChess - NumRow + 1] < 3)))
            //                {
            //                    isWin = 1;
            //                }

            //        }


            //}
            //         if (isWin == true)
            //             MessageBox.Show("sap win");
            //Check 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
