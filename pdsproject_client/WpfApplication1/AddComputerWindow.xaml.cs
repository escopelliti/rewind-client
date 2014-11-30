using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

using MainApp;
using ConnectionModule;

namespace Views
{
    /// <summary>
    /// Logica di interazione per AddComputerWindow.xaml
    /// </summary>
    public partial class AddComputerWindow : Window
    {
        private AddComputerViewModel viewModel;
        private MainWindow mainWin;

        public AddComputerWindow(MainWindow mainWin)
        {
            InitializeComponent();
            this.mainWin = mainWin;
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

            this.Close();
            Server newServer = new Server();
            newServer.ComputerName = computerName;
            Channel channel = new Channel();
            channel.CmdPort = Convert.ToUInt16(cmdPort);
            channel.DataPort = Convert.ToUInt16(dataPort);
            channel.ipAddress = System.Net.IPAddress.Parse(ip);
            newServer.SetChannel(channel);

            mainWin.OnNewComputerConnected(this, newServer);           
        }
    }    

    public class AddComputerViewModel : INotifyPropertyChanged
    {
        private string computerName = "Nome_computer";
        private string byteIP1 = "0";
        private string byteIP2 = "0";
        private string byteIP3 = "0";
        private string byteIP4 = "0";
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
        public String ByteIp1
        {
            get
            {
                return byteIP1;
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
                if (byteIP1 != value)
                {
                    byteIP1 = value;
                    OnPropertyChanged("ByteIP1");
                }
            }
        }

        public String ByteIp2
        {
            get
            {
                return byteIP2;
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
                if (byteIP2 != value)
                {
                    byteIP2 = value;
                    OnPropertyChanged("ByteIP2");
                }
            }
        }

        public String ByteIp3
        {
            get
            {
                return byteIP3;
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
                if (byteIP3 != value)
                {
                    byteIP3 = value;
                    OnPropertyChanged("ByteIP3");
                }
            }
        }

        public String ByteIp4
        {
            get
            {
                return byteIP4;
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
                if (byteIP4 != value)
                {
                    byteIP4 = value;
                    OnPropertyChanged("ByteIP4");
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