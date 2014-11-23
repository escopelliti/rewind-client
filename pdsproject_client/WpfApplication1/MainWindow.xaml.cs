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
        public ComputerItem FocusedComputerItem { get; set; }
        private List<Server> serverList;
        public FullScreenRemoteServerControl fullScreenWin;
        private ConfigurationManager configurationMgr;
        private InterceptEvents ie = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            InitTrayIcon();
            // I server su cui è attiva l'applicazione socperti dal modulo di discovery vengono aggiunti alla lista computerItemList
            computerItemList = new ObservableCollection<ComputerItem>();
            computerList.ItemsSource = computerItemList;
            serverList = new List<Server>();
            configurationMgr = new ConfigurationManager();
            channelMgr = new ChannelManager();
            StartDiscovery();                      
        }

        private void InitTrayIcon()
        {
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
        }

        private void StartDiscovery()
        {
            sd = new Discovery.ServiceDiscovery(this);            
        }

        public void OnNewComputerConnected(Object sender, Object param) {

            Server newServer = (Server)param;
            Server oldServer = serverList.Find(x => x.ComputerName.Equals(newServer.ComputerName));
            if (oldServer != null)
            {
                this.serverList.Remove(oldServer);
                this.serverList.Add(newServer);
                return;
            }
            serverList.Add(newServer);            
            int lastComputerNum = -1;
            if (this.computerItemList.Count != 0)
            {
                lastComputerNum = this.computerItemList.Last<ComputerItem>().ComputerNum;
            }
            lastComputerNum += 1;
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {                
                this.computerItemList.Add(new ComputerItem() { Name = newServer.ComputerName, ComputerStateImage = @"resources/images/off.png", ComputerNum = lastComputerNum, IsCheckboxEnabled=true});
            }));
        }

        private void OpenFullScreenWindow(InterceptEvents ie, List<Hotkey> hotkeyList, ChannelManager channelMgr)
        {
            List<String> computerStringList = new List<string>();
            foreach (Server s in channelMgr.ConnectedServer)
            {
                computerStringList.Add(s.ComputerName);
            }
            fullScreenWin = new FullScreenRemoteServerControl(ie, hotkeyList, channelMgr.GetCurrentServer(), computerStringList, this);
            fullScreenWin.Show();
            this.WindowState = WindowState.Minimized;
        }
        
        private void OpenWorkareaWindow()
        {
            if (channelMgr.GetCurrentServer() != null)
            {
                WorkareaWindow wk = new WorkareaWindow(channelMgr, this);
                wk.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Non hai ancora un computer attivo. Selezionane uno!", "Ops...", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        public void OnSwitch(object sender, object param)
        {
            InterceptEvents.StopCapture();
            OpenWorkareaWindow();
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddComputerWindow w = new AddComputerWindow(this);
            w.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!exit)
            {
                if (channelMgr.GetCurrentServer() != null)
                {
                    InterceptEvents.RestartCapture();
                }
                this.WindowState = WindowState.Minimized;
                e.Cancel = true;
                return;
            }            
            this.sd.Stop();
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

        public void OnEndConnectionToServer(Object obj, Object param)
        {
            ServerEventArgs sea = (ServerEventArgs)param;
            Server server = sea.Server;
            ComputerItem oldComputerItem = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            if (oldComputerItem != null)
            {
                this.computerList.Dispatcher.Invoke(new Action(() =>
                {
                    this.computerItemList.Remove(oldComputerItem);
                    oldComputerItem.ComputerStateImage = @"resources/images/off.png";
                    oldComputerItem.IsCheckboxEnabled = true;

                    this.computerItemList.Add(oldComputerItem);
                }));
            }
        }

        public void OnSetNewServer(Object obj, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server server = sea.Server;
            ComputerItem newComputerItem = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            if (newComputerItem != null)
            {
                SetServerActive(newComputerItem);
            }            
        }

        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            serverList.Remove(server);
            if (computerItemList.Count != 0)
            {
                ComputerItem toRemove = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
                if (toRemove != null)
                {
                    this.computerList.Dispatcher.Invoke(new Action(() =>
                    {
                        this.computerItemList.Remove(toRemove);
                    }));
                }
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is FullScreenRemoteServerControl)
                    {
                        if (win.IsActive)
                        {
                            ((FullScreenRemoteServerControl)win).RemoveServerFromList(server);
                            break;
                        }
                    }
                }
            }
        }

        private void SetActiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FocusedComputerItem.IsCheckboxChecked)
            {
                System.Windows.MessageBox.Show
                    ("Connettiti al computer prima di continuare", "Attenzione!", MessageBoxButton.OK,MessageBoxImage.Exclamation);
                return;
            }

            SetServerActive(FocusedComputerItem);

            Server currentServer = channelMgr.GetCurrentServer();
            Server s = serverList.Find(x => x.ComputerName == FocusedComputerItem.Name);

            if (currentServer == null)
            {
                Thread startConnection = new Thread(() => StartNewConnection(s.ServerID));
                startConnection.IsBackground = true;
                startConnection.Start();
            }
            else
            {
                if (FocusedComputerItem.Name == currentServer.ComputerName)
                {
                    System.Windows.MessageBox.Show
                    ("Il focus è già attivo su questo computer", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                //verifico che il computer su cui voglio attivare il focus è connesso
                SwitchOperator switchOp = new SwitchOperator(this);
                Thread switchThread = new Thread(() => switchOp.SwitchOperations(FocusedComputerItem.ComputerID, channelMgr));
                switchThread.SetApartmentState(ApartmentState.STA);
                switchThread.IsBackground = true;
                switchThread.Start();
                this.WindowState = WindowState.Minimized;
            }
            //al momento con c'è alcun focus settato sui server
            bool isWindowOpened = false;
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).UpdateCurrentServer(s);
                        isWindowOpened = true;
                        break;
                    }
                }
            }

            if(!isWindowOpened)
            {
                List<Hotkey> l = configurationMgr.ReadConfiguration().hotkeyList;
                if (ie != null)
                {
                    InterceptEvents.RestartCapture();
                }
                else
                {
                    ie = new InterceptEvents(channelMgr);
                }               
                OpenFullScreenWindow(ie, l, channelMgr);
            }
        }

        private void StartNewConnection(int p)
        {
            channelMgr.StartNewConnection(FocusedComputerItem.ComputerID);
            this.Dispatcher.Invoke(new Action(() => WindowState = WindowState.Minimized));
        }

        private void connectCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Server s = serverList.Find(x => x.ComputerName == FocusedComputerItem.Name);

            if (s.GetChannel().GetCmdSocket() == null || s.GetChannel().GetDataSocket() == null)
            {
                try
                {
                    channelMgr.AssignCmdChannel(s);
                    channelMgr.AddServer(s);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Il computer sembra non rispondere!", "Ops...", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.computerList.Dispatcher.Invoke(new Action(() =>
                    {
                        int index = this.computerItemList.IndexOf(FocusedComputerItem);
                        this.computerItemList.Remove(FocusedComputerItem);
                        this.FocusedComputerItem.IsCheckboxChecked = false;
                        this.computerItemList.Insert(index, FocusedComputerItem);
                    }));
                    return;
                }
            }

            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(FocusedComputerItem);
                this.computerItemList.Remove(FocusedComputerItem);
                FocusedComputerItem.ComputerID = s.ServerID;
                this.FocusedComputerItem.IsCheckboxChecked = true;
                this.computerItemList.Insert(index,FocusedComputerItem);
            }));
            
            AuthenticationWindow a = new AuthenticationWindow(s, channelMgr, this);
            a.ShowDialog();
                                
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).AddServerToList(s);
                        break;
                    }
                }
            }  
        }

        private void connectCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(FocusedComputerItem);
                this.computerItemList.Remove(FocusedComputerItem);
                this.FocusedComputerItem.IsCheckboxChecked = false;
                this.computerItemList.Insert(index, FocusedComputerItem);
            }));   
            
            Server s = serverList.Find(x => x.ComputerName == FocusedComputerItem.Name);
            channelMgr.DeleteServer(s, System.Net.Sockets.SocketShutdown.Send);

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).RemoveServerFromList(s);
                        break;
                    }
                }
            }
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FocusedComputerItem = (ComputerItem)(sender as ListBoxItem).Content;            
        }    

        private void ButtonModifyClick(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is ModifyHotkeyWindow)
                {
                    isWindowOpen = true;
                    win.Activate();
                }
            }

            if (!isWindowOpen)
            {
                ModifyHotkeyWindow w = new ModifyHotkeyWindow();
                w.Show();
            }
        }    
    
        public void Forbidden(Server s)
        {
 	        ComputerItem ci = this.computerItemList.Where(x => x.Name == s.ComputerName).First<ComputerItem>();
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(ci);
                this.computerItemList.Remove(ci);
                this.FocusedComputerItem.IsCheckboxChecked = false;
                this.computerItemList.Insert(index, ci);
            }));          
        }

        public void Permitted(Server toAuthenticate)
        {
            Server s = this.serverList.Find(x => x.ComputerName == toAuthenticate.ComputerName);
            if (s != null)
            {
                s.Authenticated = toAuthenticate.Authenticated;
            }
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(FocusedComputerItem);
                this.computerItemList.Remove(FocusedComputerItem);
                this.FocusedComputerItem.IsCheckboxChecked = true;
                this.computerItemList.Insert(index, FocusedComputerItem);
            }));
        }

        private void ButtonExitClick(object sender, RoutedEventArgs e)
        {
            Thread exit = new Thread(() => ExitFromApplication());
            exit.IsBackground = true;
            exit.Start();
            
        }

        public void ExitFromApplication()
        {
            if (channelMgr.GetCurrentServer() != null)
            {
                channelMgr.EndConnectionToCurrentServer();
                channelMgr.SetCurrentServer(null);
            }
            
            Server[] toRemove = new Server[channelMgr.ConnectedServer.Count];
            channelMgr.ConnectedServer.CopyTo(toRemove,0);

            foreach (Server s in toRemove)
            {
                channelMgr.DeleteServer(s, System.Net.Sockets.SocketShutdown.Both);
            }
            this.Dispatcher.Invoke(new Action(() => { System.Windows.Application.Current.Shutdown(); }));
        }

        private void ButtonInfoClick(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWnd = new InfoWindow();
            infoWnd.Show();
        }

        public void SetServerActive(ComputerItem newComputerItem)
        {
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(newComputerItem);
                this.computerItemList.Remove(newComputerItem);
                newComputerItem.ComputerStateImage = @"resources/images/connComputer.png";
                newComputerItem.IsCheckboxEnabled = false;
                this.computerItemList.Insert(index, newComputerItem);
            }));
        }
    }
}

