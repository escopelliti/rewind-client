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

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace WpfApplication1
{

    
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private bool exit = false;

        public MainWindow()
        {
            InitializeComponent();

            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem1 });

            // Initialize menuItem1 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            

            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon("..\\..\\Computers.ico");
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);            

            MyNotifyIcon.ContextMenu = this.contextMenu1;

            List<ComputerItem> items = new List<ComputerItem>();
            items.Add(new ComputerItem() { Name = "TEST_PC", ComputerStateImage = "connComputer.png", computerNum = "1", focusedImage = "tick.png" });
            items.Add(new ComputerItem() { Name = "poppo", ComputerStateImage = "connComputer.png", computerNum = "2" });
            items.Add(new ComputerItem() { Name = "TEST_PC", ComputerStateImage = "connComputer.png", computerNum = "1" });
            items.Add(new ComputerItem() { Name = "poppo", ComputerStateImage = "connComputer.png", computerNum = "2" });
            items.Add(new ComputerItem() { Name = "TEST_PC", ComputerStateImage = "connComputer.png", computerNum = "1" });
            items.Add(new ComputerItem() { Name = "poppo", ComputerStateImage = "connComputer.png", computerNum = "2" });
            items.Add(new ComputerItem() { Name = "TEST_PC", ComputerStateImage = "connComputer.png", computerNum = "1" });
            items.Add(new ComputerItem() { Name = "poppo", ComputerStateImage = "connComputer.png", computerNum = "2" });
            items.Add(new ComputerItem() { Name = "TEST_PC", ComputerStateImage = "connComputer.png", computerNum = "1" });
               
            
            computerList.ItemsSource = items;

            WorkareaWindow w = new WorkareaWindow();            
            w.computerList.ItemsSource = items;
            w.Show();
            

            //FullScreenRemoteServerControl f = new FullScreenRemoteServerControl();
            //f.Show();
            //List<String> list = new List<String>();
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //list.Add("POPPO");
            //f.connectedComputerList.ItemsSource = list;
            //f.currentServerName.Content = "INSIDEMYHEAD";
            //f.controlPanelShortcutLabel.Content = "CTRL + E";
            //f.switchServerShortcutLabel.Content = "CTRL + R";
        }


        //public void StartTimer()
        //{
        //    System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //    myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500); 
        //    myDispatcherTimer.Tick += new EventHandler(Each_Tick);            
        //    myDispatcherTimer.Start();
        //}

        //public void Each_Tick(object o, EventArgs sender)
        //{
        //    MyNotifyIcon.Visible = !MyNotifyIcon.Visible;         
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // TO BE CHANGED BUT IT WORKS
            bool isWindowOpen = false;

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is AddComputerWindow)
                {
                    isWindowOpen = true;
                    win.Activate();
                }
            }

            if (!isWindowOpen)
            {
                AddComputerWindow w = new AddComputerWindow();
                w.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!exit)
            {
                this.WindowState = WindowState.Minimized;
                e.Cancel = true;
            }            
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            exit = true;
            this.Close();
        }


        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;            
        }

        //TO BE CHANGED
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                MyNotifyIcon.BalloonTipText = "Minimized the app ";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // AT FIRST IT ASKS YOU A PSW
            //CODE TO SWITCH SERVER OR MAKE CURRENT THAT SPECIFIC SERVER
        }
    }
}
