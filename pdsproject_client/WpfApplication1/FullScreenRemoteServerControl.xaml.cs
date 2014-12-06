using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using ConnectionModule;
using KeyboardMouseController.HookMgr;
using MainApp;
using KeyboardMouseController;
using GenericDataStructure;

namespace Views
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
        public CommandBinding SwitchCmdBinding { get;set;}
        public CommandBinding RemotePasteCmdBinding { get; set; }


        public FullScreenRemoteServerControl(InterceptEvents ie, List<Hotkey> hotkeyList, Server currentServer, List<String> computerList, MainWindow mainWin)
        {
            this.currentServer = currentServer;
            this.DataContext = this;            
            this.computerList = new ObservableCollection<String>(computerList);
            this.interceptEvent = ie;
            this.hotkeyList = hotkeyList;
            this.MainWin = mainWin;
            InitializeComponent();
            InitGUI();
            RegisterHotkey();
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
                            SwitchCmdBinding = new CommandBinding(settings,Switch_Server_Event_Handler);
                            CommandBindings.Add(SwitchCmdBinding);
                            // Istanzio il delegato dell'evento
                            SwitchServeHandler = new SwitchServerEventHandler(MainWin.OnSwitch);
                            break;

                        case Hotkey.OPEN_PANEL_CMD:
                            CommandBindings.Add(new CommandBinding(settings, Open_Panel_Event_Handler));
                            break;

                        case Hotkey.REMOTE_PAST_CMD:
                            RemotePasteCmdBinding = new CommandBinding(settings, Send_Remote_Past_Request);
                            CommandBindings.Add(RemotePasteCmdBinding);
                            break;

                        case Hotkey.BLOCK_CAPTURE:
                            CommandBindings.Add(new CommandBinding(settings, Block_Capture_Event_Handler));
                            break;

                        default:
                            break;

                            
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Scorciatoie non disponibili per la sessione attuale!", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    case Hotkey.BLOCK_CAPTURE:
                        this.blockCaptureLabel.Content = hotkey.KModifier + " + " + hotkey.Key;
                        break;
                    default:
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

        private void OnSwitch(EventArgs eventArgs)
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
            InterceptEvents.ResetKModifier();
        }

        private void Block_Capture_Event_Handler(object sender, ExecutedRoutedEventArgs e)
        {
            InterceptEvents.SwitchBlock();
            InterceptEvents.ResetKModifier();
        }

        private void Open_Panel_Event_Handler(object sender, RoutedEventArgs e)
        {
            InterceptEvents.StopCapture();
            try
            {
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is MainWindow)
                    {
                        win.WindowState = System.Windows.WindowState.Normal;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("C'è stato un problema. Prova a riavviare l'applicazione. ", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                //nothing to do
                return;
            }
            
        }

        public void AddServerToList(Server s)
        {
            if (!s.Authenticated) 
            { 
                return; 
            }

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
