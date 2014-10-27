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
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string computerName = this.computerNameTextBox.Text;
            if (computerName != null && !String.IsNullOrEmpty(computerName) && !string.IsNullOrWhiteSpace(computerName))
            {
                MessageBox.Show(computerName);
                //routine di creazione grafica del server nella GUI e scrittura in una file di configurazione;
            }
            else
            {
                //TO BE ADDED IN PROP FILES
                MessageBox.Show("Inserisci un nome del computer valido.");
            }
        }
    }
}
