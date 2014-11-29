using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CommunicationLibrary;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per FullScreenRemoteServerControl.xaml
    /// </summary>
    public partial class FullScreenRemoteServerControl : Window
    {
        private Server currentServer;
        private ObservableCollection<String> computerList;
        private InterceptEvents interceptEvent;
        public List<Hotkey> hotkeyList;
        public delegate void SwitchServerEventHandler(Object sender, Object param);
        public event SwitchServerEventHandler SwitchServeHandler;
        public MainWindow MainWin{get;set;}

        public FullScreenRemoteServerControl(InterceptEvents ie, List<Hotkey> hotkeyList, Server currentServer, List<String> computerList, MainWindow mainWin)
        {
            this.currentServer = currentServer;
            this.DataContext = this;
            // Forse non è supportato il costruttore così fatto DA PROVARE
            this.computerList = new ObservableCollection<String>(computerList);
            this.interceptEvent = ie;
            this.hotkeyList = hotkeyList;
            this.MainWin = mainWin;
            InitializeComponent();
            InitGUI();
            RegisterHotkey();
        }

        private void MinimizedControlPanel()
        {
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is MainWindow)
                {
                    if (win.IsVisible)
                    {
                        win.WindowState = WindowState.Minimized;
                        break;
                    }
                }
            }
        }

        private void RegisterHotkey()
        {
            foreach (Hotkey h in hotkeyList)
            {
                try
                {
                    RoutedCommand settings = new RoutedCommand();
                    settings.InputGestures.Add(new KeyGesture(h.Key, h.KModifier));
                    switch (h.Command)
                    {
                        case Hotkey.SWITCH_SERVER_CMD:
                            CommandBindings.Add(new CommandBinding(settings, Switch_Server_Event_Handler));
                            // Istanzio il delegato dell'evento
                            SwitchServeHandler = new SwitchServerEventHandler(MainWin.OnSwitch);
                            break;

                        case Hotkey.OPEN_PANEL_CMD:
                            CommandBindings.Add(new CommandBinding(settings, Open_Panel_Event_Handler));
                            break;

                        case Hotkey.REMOTE_PAST_CMD:
                            CommandBindings.Add(new CommandBinding(settings, Send_Remote_Past_Request));
                            break;
                    }
                }
                catch (Exception)
                {
                    //handle exception error
                }
            }
        }

       
        private void InitGUI()
        {
            foreach (Hotkey hotkey in hotkeyList)
            {
                switch (hotkey.Command)
                {
                    case Hotkey.SWITCH_SERVER_CMD:
                        this.switchServerShortcutLabel.Content = hotkey.KModifier + " + " + hotkey.Key;
                        break;
                    case Hotkey.OPEN_PANEL_CMD:
                        this.controlPanelShortcutLabel.Content = hotkey.KModifier + " + " + hotkey.Key;
                        break;
                    case Hotkey.REMOTE_PAST_CMD:
                        this.remotePasteShortcutLabel.Content = hotkey.KModifier + " + " + hotkey.Key;
                        break;
                }
            }
            this.currentServerNameLabel.Content = currentServer.ComputerName;
            this.connectedComputerList.ItemsSource = this.computerList;            
        }

        private void Switch_Server_Event_Handler(object sender, ExecutedRoutedEventArgs e)
        {
            OnSwitch(new EventArgs());
        }

        protected virtual void OnSwitch(EventArgs eventArgs)
        {
            SwitchServerEventHandler handler = SwitchServeHandler;
            if (handler != null)
            {                
                handler(this, eventArgs);
            }
        }

        private void Send_Remote_Past_Request(object sender, RoutedEventArgs e)
        {
            MainWin.channelMgr.SendRequest(Protocol.ProtocolUtils.REMOTE_PASTE, string.Empty);
        }


        private void Open_Panel_Event_Handler(object sender, RoutedEventArgs e)
        {
            InterceptEvents.StopCapture();
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is MainWindow)
                {
                    win.WindowState = System.Windows.WindowState.Normal;                    
                }
            }
        }

        public void AddServerToList(Server s)
        {
            this.connectedComputerList.Dispatcher.Invoke(new Action(() =>
            {
                this.computerList.Add(s.ComputerName);
            }));

        }

        public void RemoveServerFromList(Server s)
        {
            this.connectedComputerList.Dispatcher.Invoke(new Action(() =>
            {
                this.computerList.Remove(s.ComputerName);
            }));
        }

        public void UpdateCurrentServer(Object sender, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server s = sea.Server;
            currentServer = s;
            this.currentServerNameLabel.Dispatcher.Invoke(new Action(() =>
            {
                this.currentServerNameLabel.Content = currentServer.ComputerName;

            }));

        }      
    }
}
