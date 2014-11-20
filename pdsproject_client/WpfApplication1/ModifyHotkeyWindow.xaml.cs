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
    /// Logica di interazione per ModifyHotkeyWindow.xaml
    /// </summary>
    public partial class ModifyHotkeyWindow : Window
    {
        private ConfigurationManager confMgr;
  
        public ModifyHotkeyWindow()
        {
            confMgr = new ConfigurationManager();
            InitializeComponent();
            InitializeComboBox();
            InitializeGui();

        }

        private void InitializeGui()
        {
            Configuration config = confMgr.ReadConfiguration();
            Hotkey h = config.hotkeyList.Find(x => x.Command == Hotkey.SWITCH_SERVER_CMD);
            SwitchServerKeyLabel.Content = h.Key;
            SwitchServeComboBox.SelectedItem = h.KModifier;
            h = config.hotkeyList.Find(x => x.Command == Hotkey.OPEN_PANEL_CMD);
            OpenPanelKeyLabel.Content = h.Key;
            OpenPanelComboBox.SelectedItem = h.KModifier;
        }

        private void InitializeComboBox()
        {
            List<ModifierKeys> ModifierList = new List<ModifierKeys>();
            ModifierList.Add(ModifierKeys.Alt);
            ModifierList.Add(ModifierKeys.Control);
            ModifierList.Add(ModifierKeys.Shift);
            ModifierList.Add(ModifierKeys.Windows);
            SwitchServeComboBox.ItemsSource = ModifierList;
            OpenPanelComboBox.ItemsSource = ModifierList;
        }

        private void ButtonChangeComputerClick(object sender, RoutedEventArgs e)
        {
            this.KeyDown += ButtonChangeComputer_KeyDown;
            this.KeyDown -= ButtonOpenPanel_KeyDown;
        }

        private void ButtonChangeComputer_KeyDown(object sender, KeyEventArgs e)
        {
            //filtro caratteri non consentiti
            if (IsValidCharacheter(e.Key))
            {
                this.KeyDown -= ButtonChangeComputer_KeyDown;
                SwitchServerKeyLabel.Content = e.Key;
            }
            else 
            {
                System.Windows.MessageBox.Show
                    ("Carattere non consentito, premi un altro tasto", "Attenzione!", MessageBoxButton.OK,MessageBoxImage.Exclamation); 
            }

        }

        private bool IsValidCharacheter(Key k)
        {
            if (k == Key.LeftCtrl || k == Key.RightCtrl ||
                k == Key.LeftShift || k == Key.RightShift ||
                k == Key.LWin || k == Key.RWin ||
                k == Key.LeftAlt || k == Key.RightAlt) 
            {
                return false;
            }
            return true;
        }

        private void ButtonOpenPanelClick(object sender, RoutedEventArgs e)
        {
            this.KeyDown += ButtonOpenPanel_KeyDown;
            this.KeyDown -= ButtonChangeComputer_KeyDown;
        }

        private void ButtonOpenPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsValidCharacheter(e.Key))
            {
                this.KeyDown -= ButtonOpenPanel_KeyDown;
                OpenPanelKeyLabel.Content = e.Key;
            }
            else
            {
                System.Windows.MessageBox.Show
                    ("Carattere non consentito, premi un altro tasto", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation); 
            }
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonDefaultClick(object sender, RoutedEventArgs e) 
        {
            Configuration stdConfig = confMgr.createStdConfiguration();
            Hotkey h = stdConfig.hotkeyList.Find(x=>x.Command==Hotkey.SWITCH_SERVER_CMD);
            SwitchServerKeyLabel.Content = h.Key;
            SwitchServeComboBox.SelectedItem = h.KModifier;
            h = stdConfig.hotkeyList.Find(x => x.Command == Hotkey.OPEN_PANEL_CMD);
            OpenPanelKeyLabel.Content = h.Key;
            OpenPanelComboBox.SelectedItem = h.KModifier;
        }


        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {

            if (IsValidConfiguration())
            {
                Hotkey newSwitchServerHotkey = new Hotkey
                    ((ModifierKeys)SwitchServeComboBox.SelectedItem, (Key)SwitchServerKeyLabel.Content, Hotkey.SWITCH_SERVER_CMD);

                Hotkey newOpenPanelHotkey = new Hotkey
                    ((ModifierKeys)OpenPanelComboBox.SelectedItem, (Key)OpenPanelKeyLabel.Content, Hotkey.OPEN_PANEL_CMD);

                Configuration newConf = new Configuration();
                newConf.hotkeyList.Add(newSwitchServerHotkey);
                newConf.hotkeyList.Add(newOpenPanelHotkey);
                ConfigurationManager confMgr = new ConfigurationManager();
                confMgr.UpdateConfigFile(newConf);

                this.Close();
                System.Windows.MessageBox.Show ("Le modifiche saranno disponibili al prossimo avvio dell'applicazione", "Informazione", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private bool IsValidConfiguration()
        {
            if (SwitchServeComboBox.SelectedItem.Equals(OpenPanelComboBox.SelectedItem) &&
                SwitchServerKeyLabel.Content.Equals(OpenPanelKeyLabel.Content))
            {
                System.Windows.MessageBox.Show ("Non puoi scegliere due shortcut uguali", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;

            }
            return true;
        }

    }
}