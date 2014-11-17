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
using CommunicationLibrary;
using System.Threading;
using System.Security.Cryptography;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        private Server toAuthenticate;
        private ChannelManager channelMgr;
        private Authentication.AuthenticationMgr authMgr;

        public AuthenticationWindow(Server toAuthenticate, ChannelManager channelMgr, MainWindow mainWin)
        {
            InitializeComponent();
            this.toAuthenticate = toAuthenticate;
            this.channelMgr = channelMgr;
            authMgr = new Authentication.AuthenticationMgr(channelMgr, mainWin);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartAuthentication(String hashString)
        {            
            bool auth = authMgr.Authenticate(toAuthenticate, hashString);
            if (!auth)
            {
                this.errorLabel.Dispatcher.Invoke(new Action(() =>
                {
                    this.errorLabel.Foreground = Brushes.Red;
                }));
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            String psw = this.pswBox.Password;
            if (psw != String.Empty && psw != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(psw);
                SHA256Managed hashstring = new SHA256Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.AppendFormat("{0:X2}", b);
                }
                string hashString = stringBuilder.ToString();
                Thread authThread = new Thread(() => StartAuthentication(hashString));
                authThread.Start();
                this.Close();
            }
            else
            {                
                BrushConverter bc = new BrushConverter();
                this.pswBox.Background = (Brush) bc.ConvertFrom("#FFFF7F7F");                
            }
        }
    }
}
