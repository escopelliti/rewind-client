using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using ClientConfiguration;
using KeyboardMouseController;

namespace Views
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
            try
            {
                Configuration config = confMgr.ReadConfiguration();
                Hotkey h = config.hotkeyList.Find(x => x.Command == Hotkey.SWITCH_SERVER_CMD);
                SwitchServerKeyLabel.Content = h.Key;
                SwitchServeComboBox.SelectedItem = h.KModifier;
                h = config.hotkeyList.Find(x => x.Command == Hotkey.OPEN_PANEL_CMD);
                OpenPanelKeyLabel.Content = h.Key;
                OpenPanelComboBox.SelectedItem = h.KModifier;
                h = config.hotkeyList.Find(x => x.Command == Hotkey.REMOTE_PAST_CMD);
                RemotePasteKeyLabel.Content = h.Key;
                RemotePasteComboBox.SelectedItem = h.KModifier;
            }
            catch (Exception)
            {
                MessageBox.Show("La modifica delle scorciatoie non è disponibile al momento. Riprova al riavvio dell'applicazione.", "Ops...", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            
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
            RemotePasteComboBox.ItemsSource = ModifierList;
        }

        private void ButtonChangeComputerClick(object sender, RoutedEventArgs e)
        {
            this.KeyDown += ButtonChangeComputer_KeyDown;
            this.KeyDown -= ButtonOpenPanel_KeyDown;
            this.KeyDown -= ButtonRemotePaste_KeyDown;
        }

        private void ButtonChangeComputer_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void ButtonOpenPanelClick(object sender, RoutedEventArgs e)
        {
            this.KeyDown += ButtonOpenPanel_KeyDown;
            this.KeyDown -= ButtonChangeComputer_KeyDown;
            this.KeyDown -= ButtonRemotePaste_KeyDown;
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

        private void ButtonRemotePasteClick(object sender, RoutedEventArgs e)
        {
            this.KeyDown += ButtonRemotePaste_KeyDown;
            this.KeyDown -= ButtonChangeComputer_KeyDown;
            this.KeyDown -= ButtonOpenPanel_KeyDown;
        }

        private void ButtonRemotePaste_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsValidCharacheter(e.Key))
            {
                this.KeyDown -= ButtonRemotePaste_KeyDown;
                RemotePasteKeyLabel.Content = e.Key;
            }
            else
            {
                System.Windows.MessageBox.Show
                    ("Carattere non consentito, premi un altro tasto", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonDefaultClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                Configuration stdConfig = confMgr.createStdConfiguration();
                Hotkey h = stdConfig.hotkeyList.Find(x => x.Command == Hotkey.SWITCH_SERVER_CMD);
                SwitchServerKeyLabel.Content = h.Key;
                SwitchServeComboBox.SelectedItem = h.KModifier;
                h = stdConfig.hotkeyList.Find(x => x.Command == Hotkey.OPEN_PANEL_CMD);
                OpenPanelKeyLabel.Content = h.Key;
                OpenPanelComboBox.SelectedItem = h.KModifier;
                h = stdConfig.hotkeyList.Find(x => x.Command == Hotkey.REMOTE_PAST_CMD);
                RemotePasteKeyLabel.Content = h.Key;
                RemotePasteComboBox.SelectedItem = h.KModifier;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show
                    ("Problema nel ripristino delle impostazioni. Riprova più tardi.", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }            
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {

            if (IsValidConfiguration())
            {
                Hotkey newSwitchServerHotkey = new Hotkey
                    ((ModifierKeys)SwitchServeComboBox.SelectedItem, (Key)SwitchServerKeyLabel.Content, Hotkey.SWITCH_SERVER_CMD);

                Hotkey newOpenPanelHotkey = new Hotkey
                    ((ModifierKeys)OpenPanelComboBox.SelectedItem, (Key)OpenPanelKeyLabel.Content, Hotkey.OPEN_PANEL_CMD);

                Hotkey newRemotePasteHotkey = new Hotkey
                    ((ModifierKeys)RemotePasteComboBox.SelectedItem, (Key)RemotePasteKeyLabel.Content, Hotkey.REMOTE_PAST_CMD);

                Configuration newConf = new Configuration();
                newConf.hotkeyList.Add(newSwitchServerHotkey);
                newConf.hotkeyList.Add(newOpenPanelHotkey);
                newConf.hotkeyList.Add(newRemotePasteHotkey);
                ConfigurationManager confMgr = new ConfigurationManager();
                confMgr.WriteConfigFile(newConf);

                this.Close();
                System.Windows.MessageBox.Show ("Le modifiche saranno disponibili al prossimo avvio dell'applicazione", "Informazione", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsValidConfiguration()
        {
            if (SwitchServeComboBox.SelectedItem.Equals(OpenPanelComboBox.SelectedItem) &&
                SwitchServeComboBox.SelectedItem.Equals(RemotePasteComboBox.SelectedItem) &&
                OpenPanelComboBox.SelectedItem.Equals(RemotePasteComboBox.SelectedItem) &&
                SwitchServerKeyLabel.Content.Equals(OpenPanelKeyLabel.Content) &&
                SwitchServerKeyLabel.Content.Equals(RemotePasteKeyLabel) &&
                OpenPanelKeyLabel.Content.Equals(RemotePasteKeyLabel))
            {
                System.Windows.MessageBox.Show ("Non puoi scegliere due shortcut uguali", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            return true;
        }
    }
}