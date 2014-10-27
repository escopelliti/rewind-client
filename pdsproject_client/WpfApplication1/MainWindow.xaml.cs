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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<ComputerItem> items = new List<ComputerItem>();
            items.Add(new ComputerItem() { Name = "TEST_PC", Shortcut = "CTRL + 1", ComputerStateImage = "plus.png" });            

            computerList.ItemsSource = items;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 w = new Window1();
            w.Show();
        }
    }
}
