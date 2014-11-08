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
        private List<Hotkey> hotkeyList;
        private Server currentServer;
        private List<Server> computerList;

        public FullScreenRemoteServerControl(List<Hotkey> hotkeyList, Server currentServer, List<Server> computerList)
        {
            this.hotkeyList = hotkeyList;
            this.currentServer = currentServer;
            this.computerList = computerList;
            InitializeComponent();
            InitGUI();
        }

        private void InitGUI()
        {
            foreach (Hotkey hotkey in hotkeyList)
            {
                switch (hotkey.Command)
                {
                    case HotkeyManager.SWITCH_SERVER_CMD:
                        this.switchServerShortcutLabel.Content = hotkey.KModifier + " + " + hotkey.Key;
                        break;
                    case HotkeyManager.OPEN_PANEL_CMD:
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
    }
}
