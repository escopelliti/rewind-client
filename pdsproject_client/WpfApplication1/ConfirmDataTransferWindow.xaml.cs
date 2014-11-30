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
using System.Windows.Interop;
using System.Drawing;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per ConfirmDataTransferWindow.xaml
    /// </summary>
 
    public partial class ConfirmDataTransferWindow : Window
    {
        private SwitchOperator switchOp;
        private ChannelManager channelMgr;
        private int computerID;
        private ClipboardMgr clipboardMgr;

        public ConfirmDataTransferWindow(int computerID, ClipboardMgr clipboardMgr, SwitchOperator switchOp, ChannelManager channelMgr)
        {
            InitializeComponent();
            img.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle,Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions()); 
            this.computerID = computerID;
            this.clipboardMgr = clipboardMgr;
            this.channelMgr = channelMgr;
            this.switchOp = switchOp;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Thread startConnection = new Thread(() => Switch());
            startConnection.SetApartmentState(ApartmentState.STA);
            startConnection.IsBackground = true;
            startConnection.Start();
            this.Close();
        }

        private void Switch()
        {            
            clipboardMgr.ReceiveClipboard();
            switchOp.OnEndConnectionToServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            this.channelMgr.EndConnectionToCurrentServer();
            this.channelMgr.StartNewConnection(computerID);
            switchOp.OnSetNewServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            clipboardMgr.SendClipboard();
            CloseProgressBar();
            channelMgr.ResetTokenGen();                         
        }

        private void CloseProgressBar()
        {
            switchOp.mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                switchOp.mainWin.fullScreenWin.clipboardTransferLabel.Visibility = System.Windows.Visibility.Hidden;
            }));
            switchOp.mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                switchOp.mainWin.fullScreenWin.clipboardTransferProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }));
        }        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseProgressBar();
            this.Close();
            switchOp.OnEndConnectionToServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            this.channelMgr.EndConnectionToCurrentServer();
            this.channelMgr.StartNewConnection(computerID);
            switchOp.OnSetNewServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
        }
    }
}
