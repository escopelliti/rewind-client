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
        private ProgressBar progress;

        public ConfirmDataTransferWindow(int computerID, ClipboardMgr clipboardMgr, SwitchOperator switchOp, ChannelManager channelMgr)
        {
            InitializeComponent();
            this.computerID = computerID;
            this.clipboardMgr = clipboardMgr;
            this.channelMgr = channelMgr;
            this.switchOp = switchOp;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            Thread startConnection = new Thread(() => StartProgressBar());
            startConnection.IsBackground = true;
            startConnection.Start();
            
            clipboardMgr.ReceiveClipboard();
            switchOp.OnEndConnectionToServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            this.channelMgr.EndConnectionToCurrentServer();
            this.channelMgr.StartNewConnection(computerID);//FAKE e OCCHIO GESTIONE EXCEPTIONS
            switchOp.OnSetNewServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));            
            clipboardMgr.SendClipboard();

            //Se non funziona così...
            progress.Close();
            // ...prova...
            //CloseProgressBar();
            
        }

        private void CloseProgressBar()
        {
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is ProgressBar)
                {
                    if (win.IsActive)
                    {
                        ((ProgressBar)win).Close();
                        break;
                    }
                }
            }
        }

        private void StartProgressBar()
        {
            this.progress = new ProgressBar();
            progress.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
