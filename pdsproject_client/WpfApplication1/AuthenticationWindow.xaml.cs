using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Security.Cryptography;

using ConnectionModule;
using MainApp;

namespace Views
{
    /// <summary>
    /// Logica di interazione per AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        private Server toAuthenticate;
        private ChannelManager channelMgr;
        private Authentication.AuthenticationMgr authMgr;
        private MainWindow mainWin;

        public AuthenticationWindow(Server toAuthenticate, ChannelManager channelMgr, MainWindow mainWin)
        {
            InitializeComponent();
            FocusManager.SetFocusedElement(container, pswBox);
            this.toAuthenticate = toAuthenticate;
            this.channelMgr = channelMgr;
            this.mainWin = mainWin;
            authMgr = new Authentication.AuthenticationMgr(channelMgr, mainWin);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            mainWin.Dispatcher.Invoke(new Action(() =>
               {
                   try
                   {
                       int index = mainWin.computerItemList.IndexOf(mainWin.FocusedComputerItem);
                       mainWin.computerItemList.Remove(mainWin.FocusedComputerItem);
                       mainWin.FocusedComputerItem.IsCheckboxChecked = false;
                       mainWin.computerItemList.Insert(index, mainWin.FocusedComputerItem);
                   }
                   catch (Exception)
                   {
                       MessageBox.Show("C'è stato un problema nella rimozione di " + mainWin.FocusedComputerItem.Name + ".", "Ops...", MessageBoxButton.OK, MessageBoxImage.Error);
                       this.Close();
                   }
                   
               }));
            this.Close();
        }        

        private void StartAuthentication(String hashString)
        {
            bool auth = false;
            try
            {
                auth = authMgr.Authenticate(toAuthenticate, hashString);
            }
            catch (Exception)
            {
                mainWin.ExitFromApplication();
            }
            if (!auth)
            {
                this.errorLabel.Dispatcher.Invoke(new Action(() =>
                {
                    this.errorLabel.Foreground = Brushes.Red;
                }));
                return;
            }
            this.Dispatcher.Invoke(new Action(() => this.Close()));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            String psw = this.pswBox.Password;
            if (psw != String.Empty && psw != null)
            {
                StringBuilder stringBuilder = null;
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(psw);
                    SHA256Managed hashstring = new SHA256Managed();
                    byte[] hash = hashstring.ComputeHash(bytes);
                    stringBuilder = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        stringBuilder.AppendFormat("{0:X2}", b);
                    }
                }
                catch (Exception)
                {
                    BrushConverter bc = new BrushConverter();
                    this.pswBox.Background = (Brush)bc.ConvertFrom("#FFFF7F7F");
                    return;
                }
                
                string hashString = stringBuilder.ToString();
                Thread authThread = new Thread(() => StartAuthentication(hashString));
                authThread.Start();                
            }
            else
            {                
                BrushConverter bc = new BrushConverter();
                this.pswBox.Background = (Brush) bc.ConvertFrom("#FFFF7F7F");                
            }
        }
    }
}
