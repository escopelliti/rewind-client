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
        private ComputerItem focusedComputerItem;
        private List<Server> serverList;
        public FullScreenRemoteServerControl fullScreenWin;

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

            computerItemList.Add(new ComputerItem() { Name = "Test-PC", ComputerStateImage = "resources/images/connComputer.png", ComputerNum = 1, ComputerID = 0, IsCheckboxEnabled = true });
            //Carichiamo quella attivo con l'immagine corretta e con i relativi tasti disabilitati
            computerItemList.Add(new ComputerItem() { Name = "Test", ComputerStateImage = "resources/images/connComputer.png", ComputerNum = 1, ComputerID = 0, IsCheckboxEnabled = false });

            computerList.ItemsSource = computerItemList;
            
            serverList = new List<Server>();
            
            ConfigurationManager ConfigurationMgr = new ConfigurationManager();
            List<Hotkey> l = ConfigurationMgr.ReadConfiguration().hotkeyList;     
            //channelMgr = new ChannelManager();
            //InterceptEvents ie = new InterceptEvents(channelMgr);
            //OpenFullScreenWindow(ie, l, channelMgr);              
            //StartDiscovery();
            
        }

        private void StartDiscovery()
        {
            sd = new Discovery.ServiceDiscovery(this);            
        }

        public void OnNewComputerConnected(Object sender, Object param) {

            Server server = (Server)param;
            serverList.Add(server);

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
            fullScreenWin = new FullScreenRemoteServerControl(/*ie,*/ hotkeyList, channelMgr.GetCurrentServer(), channelMgr.ConnectedServer);
            fullScreenWin.Show();
           
        }

        private void SetActiveButton_Click(object sender, RoutedEventArgs e)
        {
            // AT FIRST IT ASKS YOU A PSW
            //CODE TO SWITCH SERVER OR MAKE CURRENT THAT SPECIFIC SERVER
            Server currentServer = channelMgr.GetCurrentServer();
            if (currentServer == null)
            {
                if (!focusedComputerItem.IsCheckboxChecked) 
                {
                    System.Windows.MessageBox.Show("Connettiti al computer prima di continuare","Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            //    channelMgr.SetCurrentServer
            }
            if (focusedComputerItem.Name == currentServer.ComputerName)
            {
                return;
            }

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

        public static void OnSetNewServer(Object obj, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server server = sea.Server;

            //illuminami o dammi feedback su questo nuovo server 
        }

        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            serverList.Remove(server);
            ComputerItem toRemove = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            if (toRemove != null)
            {
                this.computerList.Dispatcher.Invoke(new Action(() =>
                {
                    this.computerItemList.Remove(toRemove);
                }));
            }
        }

        private void connectCheckbox_Checked(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("ON");

            string listItemContents_str = string.Empty;
            this.focusedComputerItem.IsCheckboxChecked = true;
            for (int i = 0; i < computerList.SelectedItems.Count; i++)
            {
                System.Windows.Controls.ListBoxItem item = (System.Windows.Controls.ListBoxItem)computerList.SelectedItems[i];
                listItemContents_str = item.ToString();
            }

            Server s = serverList.Find(x => x.ComputerName == listItemContents_str);
            channelMgr.AssignChannel(s);
            channelMgr.AddServer(s);

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        fullScreenWin.UpdateList(s);
                        break;
                    }
                    else 
                    {
                        break;
                    }
                }
            }


            
            
        }

        private void connectCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.focusedComputerItem.IsCheckboxChecked = false;
            Console.WriteLine("OFF");
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            focusedComputerItem = (ComputerItem)(sender as ListBoxItem).Content;
        }    
    }
}
