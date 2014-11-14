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

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per FullScreenRemoteServerControl.xaml
    /// </summary>
    public partial class FullScreenRemoteServerControl : Window
    {
        private Server currentServer;
        private List<Server> computerList;
        private InterceptEvents interceptEvent;
        public List<Hotkey> hotkeyList;
        public delegate void SwitchServerEventHandler(Object sender, Object param);
        public event SwitchServerEventHandler SwitchServeHandler;
        

        public FullScreenRemoteServerControl(InterceptEvents ie, List<Hotkey> hotkeyList, Server currentServer, List<Server> computerList)
        {
            this.currentServer = currentServer;
            this.computerList = computerList;
            this.interceptEvent = ie;
            this.hotkeyList = hotkeyList;
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
                            CommandBindings.Add(new CommandBinding(settings, My_first_event_handler));
                            // Istanzio il delegato dell'evento
                            SwitchServeHandler = new SwitchServerEventHandler(interceptEvent.OnSwitch);
                            break;

                        case Hotkey.OPEN_PANEL_CMD:
                            CommandBindings.Add(new CommandBinding(settings, My_second_event_handler));
                        
                            break;
                    }
                }
                catch (Exception err)
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
                }
            }
            this.currentServerNameLabel.Content = currentServer;
            this.connectedComputerList.ItemsSource = GetComputerNameArrayFromServer();
        }

        private List<String> GetComputerNameArrayFromServer()
        {
            List<String> connComputers = new List<string>();
            foreach (Server s in computerList)
            {
                connComputers.Add(s.ComputerName);
            }
            return connComputers;
        }

        private void My_first_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            //handler code goes here.
            OnSwitch(new EventArgs());
        }

        protected virtual void OnSwitch(EventArgs eventArgs)
        {
            SwitchServerEventHandler handler = SwitchServeHandler;
            if (handler != null)
            {
                // Invoco il delegato 
                handler(this, eventArgs);
            }
        }

        private void My_second_event_handler(object sender, RoutedEventArgs e)
        {
            //handler code goes here. 
            MessageBox.Show("Alt+B key pressed");
        }
    }
}
