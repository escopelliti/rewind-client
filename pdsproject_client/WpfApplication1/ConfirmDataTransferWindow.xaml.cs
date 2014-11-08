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

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per ConfirmDataTransferWindow.xaml
    /// </summary>
    public partial class ConfirmDataTransferWindow : Window
    {
        private WorkareaWindow win;
        private ChannelManager channelMgr;
        private int computerID;
        private ClipboardMgr clipboardMgr;

        public ConfirmDataTransferWindow(int computerID, ClipboardMgr clipboardMgr, WorkareaWindow win, ChannelManager channelMgr)
        {
            InitializeComponent();
            this.computerID = computerID;
            this.clipboardMgr = clipboardMgr;
            this.channelMgr = channelMgr;
            this.win = win;            
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            clipboardMgr.ReceiveClipboard();
            this.channelMgr.EndConnectionToCurrentServer();
            this.channelMgr.StartNewConnection(computerID);//FAKE e OCCHIO GESTIONE EXCEPTIONS
            win.OnSetNewServer(new ServerEventArgs(this.channelMgr.getCurrentServer()));
            clipboardMgr.SendClipboard();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
