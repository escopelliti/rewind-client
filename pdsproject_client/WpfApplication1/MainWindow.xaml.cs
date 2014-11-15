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
using CommunicationLibrary;
using System.Windows.Interop;

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Collections.ObjectModel;

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
        private Discovery.ServiceDiscovery sd;
        public ObservableCollection<ComputerItem> computerItemList { get; set; }
        public ChannelManager channelMgr { get; set; }

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
            MyNotifyIcon.Icon = new System.Drawing.Icon(@"../../resources/images/Computers.ico");
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);

            MyNotifyIcon.ContextMenu = this.contextMenu1;
            computerItemList = new ObservableCollection<ComputerItem>();            
            computerList.ItemsSource = computerItemList;
            
            ConfigurationManager ConfigurationMgr = new ConfigurationManager();
            List<Hotkey> l = ConfigurationMgr.ReadConfiguration().hotkeyList;     
            channelMgr = new ChannelManager();
            Server s = new Server();
            s.ComputerName = "bernoulli";
            Channel c = new Channel();
            c.CmdPort = 12000;
            c.DataPort = 12001;
            s.SetChannel(c);
            channelMgr.AssignChannel(s);
            channelMgr.AddServer(s);
            Server s1 = new Server();
            s1.ComputerName = "New-Pc-Portable";
            Channel c1 = new Channel();
            c1.CmdPort = 12000;
            c1.DataPort = 12001;
            s1.SetChannel(c1);
            channelMgr.AssignChannel(s1);
            channelMgr.AddServer(s1);
            channelMgr.SetCurrentServer(s1);

            InterceptEvents ie = new InterceptEvents(channelMgr);
            OpenFullScreenWindow(ie, l, channelMgr);              
            StartDiscovery();
            
        }

        private void StartDiscovery()
        {
            sd = new Discovery.ServiceDiscovery(this);            
        }

        public void OnNewComputerConnected(Object sender, Object param) {

            Server server = (Server)param;            
            int lastComputerNum = -1;
            if (this.computerItemList.Count != 0)
            {
                lastComputerNum = this.computerItemList.Last<ComputerItem>().ComputerNum;
            }
            lastComputerNum += 1;
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {                
                this.computerItemList.Add(new ComputerItem() { Name = server.ComputerName, ComputerStateImage = @"resources/images/off.png", ComputerNum = lastComputerNum });
            }));
        }

        private void OpenFullScreenWindow(InterceptEvents ie, List<Hotkey> hotkeyList, ChannelManager channelMgr)
        {
            FullScreenRemoteServerControl fullScreenWin = new FullScreenRemoteServerControl(ie, hotkeyList, channelMgr.GetCurrentServer(), channelMgr.ConnectedServer);
            fullScreenWin.Show();
           
        }
        
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

                //TO BE CHANGED
                //MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                //MyNotifyIcon.BalloonTipText = "Minimized the app ";
                //MyNotifyIcon.ShowBalloonTip(400);
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

        public static void OnSetNewServer(Object obj, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server server = sea.Server;

            //illuminami o dammi feedback su questo nuovo server 
        }

        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            ComputerItem toRemove = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            if (toRemove != null)
            {
                this.computerList.Dispatcher.Invoke(new Action(() =>
                {
                    this.computerItemList.Remove(toRemove);
                }));
            }
        }
    }
}
