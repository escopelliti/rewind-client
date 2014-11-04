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
    /// Logica di interazione per AddComputerWindow.xaml
    /// </summary>
    public partial class AddComputerWindow : Window
    {
        public AddComputerWindow()
        {
            InitializeComponent();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string computerName = this.computerNameTextBox.Text;
            if (computerName != null && !String.IsNullOrEmpty(computerName) && !string.IsNullOrWhiteSpace(computerName))
            {
                MessageBox.Show(computerName);
                //Setta un item in piu nella itemlist nella GUI e scrittura in una file di configurazione;
            }
            else
            {
                //TO BE ADDED IN PROP FILES
                MessageBox.Show("Inserisci un nome del computer valido.");
            }
        }
    }
}
