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
    /// Logica di interazione per ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow(string computerName)
        {
            InitializeComponent();
            this.computerNameLabel.Content = computerName;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string computerName = this.computerNameLabel.Content.ToString();
            string psw = this.passwordBox.Password;
            string dataPort = (string) this.dataPortComboBox.SelectedItem;
            string cmdPort = (string)this.cmdPortComboBox.SelectedItem;
            if (psw != null && psw != String.Empty && psw != "")
            {
                if (dataPort != cmdPort)
                {
                    CryptoManager cm = new CryptoManager(psw);
                    byte[] pswDigest = cm.GetHashFromString(); //da inviare
                    //parte il flusso per switchare e poi connettersi
                }
                else
                {
                    MessageBox.Show(this, "Non scegliere due numeri di porta uguali.", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                this.passwordBox.BorderBrush = Brushes.Red;                
            }
            //da qui si prendono i dati sulle porte
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
