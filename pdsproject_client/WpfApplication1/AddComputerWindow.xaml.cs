using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private AddComputerViewModel viewModel;
        
        public AddComputerWindow()
        {
            InitializeComponent();
            viewModel = new AddComputerViewModel();
            DataContext = viewModel;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string computerName = this.computerNameTextBox.Text;
            string byte1 = this.byteIp1.Text;
            string byte2 = this.byteIp2.Text;
            string byte3 = this.byteIp3.Text;
            string byte4 = this.byteIp4.Text;
            string ip = byte1 + "." + byte2 + "." + byte3 + "." + byte4;
            string cmdPort = this.cmdPortTextBox.Text;
            string dataPort = this.dataPortTextBox.Text;
            

            //mettiamo nella UI queste info;
        }
    }    

    public class AddComputerViewModel : INotifyPropertyChanged
    {
        private string computerName = "Nome_computer";
        private string byteIP = "0";
        private string cmdPort = "12000";
        private string dataPort = "12001";

        public AddComputerViewModel()
        {
               
        }

        public string Name
        {
            get
            {
                return computerName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {                    
                    throw new Exception("Name can not be empty.");
                }
                if (value.Contains(" "))
                {                    
                    throw new Exception("name can not contains ");
                }
                if (computerName != value)
                {
                    computerName = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public String ByteIp
        {
            get
            {
                return byteIP;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new Exception("can not be empty.");
                }
                if (value.Contains(" "))
                {
                    throw new Exception("no empty space");
                }
                string pattern = @"^[0-9]+$";
                if (!Regex.IsMatch(value, pattern)) 
                {
                    throw new Exception("only numbers accepted");
                }
                if (byteIP != value)
                {
                    byteIP = value;
                    OnPropertyChanged("ByteIP");
                }
            }
        }

        public String DataPort
        {
            get
            {
                return dataPort;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new Exception("can not be empty.");
                }
                if (value.Contains(" "))
                {
                    throw new Exception("no empty space");
                }
                if (Convert.ToInt64(value) < 10000)
                {
                    throw new Exception("port not allowed");
                }
                if (Convert.ToInt64(value) > 65535)
                {
                    throw new Exception("port not allowed");
                }
                if ((value).Equals(this.cmdPort))
                {
                    throw new Exception("same ports");
                }
                string pattern = @"^[0-9]+$";
                if (!Regex.IsMatch(value, pattern))
                {
                    throw new Exception("only numbers accepted");
                }
                if (dataPort != value)
                {
                    dataPort = value;
                    OnPropertyChanged("DataPort");
                }
            }
        }

        public String CmdPort
        {
            get
            {
                return cmdPort;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new Exception("can not be empty.");
                }
                if (value.Contains(" "))
                {
                    throw new Exception("no empty space");
                }
                if ((value).Equals(this.dataPort))
                {
                    throw new Exception("same ports");
                }
                if (Convert.ToInt64(value) < 10000)
                {
                    throw new Exception("port not allowed");
                }
                if (Convert.ToInt64(value) > 65535)
                {
                    throw new Exception("port not allowed");
                }
                string pattern = @"^[0-9]+$";
                if (!Regex.IsMatch(value, pattern))
                {
                    throw new Exception("only numbers accepted");
                }
                if (cmdPort != value)
                {
                    cmdPort = value;
                    OnPropertyChanged("CmdPort");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}