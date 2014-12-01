using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Threading;
using System.Collections.ObjectModel;

using ConnectionModule;
using Views.ViewsPOCO;
using Views;
using ClientConfiguration;
using KeyboardMouseController.HookMgr;
using Discovery;
using KeyboardMouseController;
using GenericDataStructure;
using Switch;

namespace MainApp
{

    
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenu menu;
        private System.Windows.Forms.MenuItem menuItem;
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
            computerItemList = new ObservableCollection<ComputerItem>();
            computerList.ItemsSource = computerItemList;
            serverList = new List<Server>();
            configurationMgr = new ConfigurationManager();
            channelMgr = new ChannelManager();
            StartDiscovery();                      
        }

        private void InitTrayIcon()
        {
            this.menu = new System.Windows.Forms.ContextMenu();
            this.menuItem = new System.Windows.Forms.MenuItem();
            
            this.menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem });
            
            this.menuItem.Index = 0;
            this.menuItem.Text = "Exit";
            this.menuItem.Click += new System.EventHandler(this.menuItem_Click);
            
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = new System.Drawing.Icon(@"../../resources/images/Computers.ico");
            trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            trayIcon.ContextMenu = this.menu;
        }

        private void StartDiscovery()
        {
            sd = new Discovery.ServiceDiscovery(this);            
        }

        public void OnNewComputerConnected(Object sender, Object param) {

            Server newServer = (Server)param;
            Server oldServer = null;
            try
            {
               oldServer = serverList.Find(x => x.ComputerName.Equals(newServer.ComputerName));
            }
            catch (ArgumentNullException)
            {
                oldServer = null;
            }
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
                if (!s.Authenticated)
                    continue;
                computerStringList.Add(s.ComputerName);
            }
            fullScreenWin = new FullScreenRemoteServerControl(ie, hotkeyList, channelMgr.GetCurrentServer(), computerStringList, this);
            fullScreenWin.Show();
            this.WindowState = WindowState.Minimized;
        }
        
        private void OpenWorkareaWindow()
        {
            if (channelMgr.ConnectedServer.Count >= 2)
            {
                WorkareaWindow wk = new WorkareaWindow(channelMgr, this);
                wk.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Connetti un altro computer dal pannello di controllo!", "Ops...", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
                    InterceptEvents.ResetKModifier();
                    InterceptEvents.RestartCapture();
                }
                this.WindowState = WindowState.Minimized;
                e.Cancel = true;
                return;
            }            
            this.sd.Stop();
        }

        private void menuItem_Click(object Sender, EventArgs e)
        {
            exit = true;
            this.Close();
        }

        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;              
                trayIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                trayIcon.Visible = false;
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
            ComputerItem newComputerItem = null;
            try
            {
               newComputerItem = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            }
            catch (ArgumentNullException) {
                newComputerItem = null;
            }
            catch (Exception)
            {
                newComputerItem = null;
            }
            
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
                ComputerItem toRemove = null;
                try
                {
                    toRemove = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
                }
                catch (Exception)
                {
                    toRemove = null;
                }
                if (toRemove != null)
                {
                    this.computerList.Dispatcher.Invoke(new Action(() =>
                    {
                        this.computerItemList.Remove(toRemove);
                    }));
                }
                try
                {
                    foreach (Window win in System.Windows.Application.Current.Windows)
                    {
                        if (win is FullScreenRemoteServerControl)
                        {
                            if (win.IsVisible)
                            {
                                ((FullScreenRemoteServerControl)win).RemoveServerFromList(server);
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //nothing to do; error on retrieving current opened windows;
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
                
                Thread connectionControlThread = new Thread(() => channelMgr.OpenControlConnection());
                connectionControlThread.Start();

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
                Thread switchThread = new Thread(() => switchOp.ExecSwitch(FocusedComputerItem.ComputerID, channelMgr));
                switchThread.SetApartmentState(ApartmentState.STA);
                switchThread.IsBackground = true;
                switchThread.Start();
                this.WindowState = WindowState.Minimized;
            }
            //al momento con c'è alcun focus settato sui server
            bool isWindowOpened = false;
            try
            {
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is FullScreenRemoteServerControl)
                    {
                        if (win.IsVisible)
                        {
                            ((FullScreenRemoteServerControl)win).UpdateCurrentServer(this, new ServerEventArgs(s));
                            isWindowOpened = true;
                            break;
                        }
                    }
                }

                if (!isWindowOpened)
                {
                    List<Hotkey> hotkeyList = configurationMgr.ReadConfiguration().hotkeyList;
                    if (ie != null)
                    {
                        InterceptEvents.RestartCapture();
                    }
                    else
                    {
                        ie = new InterceptEvents(channelMgr);
                    }
                    OpenFullScreenWindow(ie, hotkeyList, channelMgr);
                }
            }
            catch (Exception)
            {
                //System.Windows.MessageBox.Show
                //    ("C'è stato qualche problema nell'iniziare la sessione. E' consigliabile riavviare l'applicazione.", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
                    if (s.GetChannel().GetCmdSocket() == null)
                    {
                        channelMgr.AssignCmdChannel(s);
                    }                    
                    channelMgr.AddServer(s);
                }
                catch (Exception)
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
                try
                {
                    this.computerItemList.Insert(index, FocusedComputerItem);
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.computerItemList.Insert(0, FocusedComputerItem);
                }
                
            }));
            
            AuthenticationWindow a = new AuthenticationWindow(s, channelMgr, this);
            a.ShowDialog();

            try
            {
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is FullScreenRemoteServerControl)
                    {
                        if (win.IsVisible)
                        {
                            ((FullScreenRemoteServerControl)win).AddServerToList(s);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //error retrieving current windows
                return;
            }              
        }

        private void connectCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(FocusedComputerItem);
                this.computerItemList.Remove(FocusedComputerItem);
                this.FocusedComputerItem.IsCheckboxChecked = false;
                try
                {
                    this.computerItemList.Insert(index, FocusedComputerItem);                    
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.computerItemList.Insert(0, FocusedComputerItem);
                }
            }));   
            
            Server s = serverList.Find(x => x.ComputerName == FocusedComputerItem.Name);
            channelMgr.DeleteServer(s, System.Net.Sockets.SocketShutdown.Send);

            try
            {
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is FullScreenRemoteServerControl)
                    {
                        if (win.IsVisible)
                        {
                            ((FullScreenRemoteServerControl)win).RemoveServerFromList(s);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //error retrieving current windows
                return;
            } 
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FocusedComputerItem = (ComputerItem)(sender as ListBoxItem).Content;            
        }    

        private void ButtonModifyClick(object sender, RoutedEventArgs e)
        {
            ModifyHotkeyWindow w = new ModifyHotkeyWindow();
            w.ShowDialog();
        }    
    
        public void Forbidden(Server s)
        {
            ComputerItem ci = null;
            try
            {
                ci = this.computerItemList.Where(x => x.Name == s.ComputerName).First<ComputerItem>();                
            }
            catch (ArgumentNullException)
            {
                ci = null;
            }
            catch (InvalidOperationException)
            {
                ci = null;
            }
            if (ci == null)
            {
                return;
            }
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(ci);
                this.computerItemList.Remove(ci);
                this.FocusedComputerItem.IsCheckboxChecked = false;
                try
                {
                    this.computerItemList.Insert(index, ci);
                }
                catch (Exception)
                {
                    this.computerItemList.Insert(0, ci);
                }
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
                try
                {
                    this.computerItemList.Insert(index, FocusedComputerItem);
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.computerItemList.Insert(0, FocusedComputerItem);
                }
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
            try
            {
                if (channelMgr.GetCurrentServer() != null)
                {
                    channelMgr.EndConnectionToCurrentServer();
                    channelMgr.SetCurrentServer(null);
                }

                Server[] toRemove = new Server[channelMgr.ConnectedServer.Count];
                channelMgr.ConnectedServer.CopyTo(toRemove, 0);

                foreach (Server s in toRemove)
                {
                    channelMgr.DeleteServer(s, System.Net.Sockets.SocketShutdown.Both);
                }
                this.Dispatcher.Invoke(new Action(() => { System.Windows.Application.Current.Shutdown(); }));
            }
            catch (Exception)
            {
                Environment.Exit(-1);
            }
        }

        private void ButtonInfoClick(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWnd = new InfoWindow();
            infoWnd.ShowDialog();
        }

        public void SetServerActive(ComputerItem newComputerItem)
        {
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                int index = this.computerItemList.IndexOf(newComputerItem);
                this.computerItemList.Remove(newComputerItem);
                newComputerItem.ComputerStateImage = @"resources/images/connComputer.png";
                newComputerItem.IsCheckboxEnabled = false;
                try
                {
                    this.computerItemList.Insert(index, newComputerItem);
                }
                catch (Exception)
                {
                    this.computerItemList.Insert(0, newComputerItem);
                }
            }));
        }
    }
}

