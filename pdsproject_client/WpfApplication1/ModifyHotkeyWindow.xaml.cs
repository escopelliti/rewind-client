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
            
        public ModifyHotkeyWindow()
        {
            InitializeComponent();
            InitizlizeComboBox();

        }

        private void InitizlizeComboBox()
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
            ConfigurationManager confMgr = new ConfigurationManager();
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
                System.Windows.MessageBox.Show
                    ("Le mofiche saranno disponibili al prossimo avvio dell'applicazione", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private bool IsValidConfiguration() {
            bool IsValidConf = true;
            IsValidConf = IsValidComboBoxField(this.OpenPanelComboBox, IsValidConf);
            IsValidConf = IsValidComboBoxField(this.SwitchServeComboBox, IsValidConf);
            IsValidConf = IsValidLabelFiled(this.SwitchServerKeyLabel, IsValidConf);
            IsValidConf = IsValidLabelFiled(this.OpenPanelKeyLabel, IsValidConf);
            return IsValidConf;
        }

        private bool IsValidComboBoxField(ComboBox comboBox, bool isValidConf)
        {

            if (comboBox.SelectedItem == null)
            {
                BrushConverter bc = new BrushConverter();
                comboBox.Background = (Brush)bc.ConvertFrom("#FFFF7F7F");
                return false;
            }
            else
            {
                BrushConverter bc = new BrushConverter();
                comboBox.Background = (Brush)bc.ConvertFrom("#FFCDCDCD");
                return isValidConf;
            }
        }

        private bool IsValidLabelFiled(Label label, bool isValidConf)
        {
            if (label.Content == null)
            {
                BrushConverter bc = new BrushConverter();
                label.Background = (Brush)bc.ConvertFrom("#FFFF7F7F");
                return false;
            }
            else
            {
                BrushConverter bc = new BrushConverter();
                label.Background = Brushes.White;
                return isValidConf;
            }

        }
    }
}