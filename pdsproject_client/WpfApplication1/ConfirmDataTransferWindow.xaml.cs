using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Interop;
using System.Drawing;

using ConnectionModule;
using Switch;
using Clipboard;
using GenericDataStructure;
using System.Windows.Media.Imaging;

namespace Views
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
        private bool exit;

        public ConfirmDataTransferWindow(int computerID, ClipboardMgr clipboardMgr, SwitchOperator switchOp, ChannelManager channelMgr)
        {
            InitializeComponent();
            img.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle,Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions()); 
            this.computerID = computerID;
            this.clipboardMgr = clipboardMgr;
            this.channelMgr = channelMgr;
            this.switchOp = switchOp;
            exit = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Thread startConnection = new Thread(() => Switch());
            startConnection.SetApartmentState(ApartmentState.STA);
            startConnection.IsBackground = true;
            startConnection.Start();
            exit = false;
            this.Close();
        }

        private void Switch()
        {
            try
            {
                clipboardMgr.ReceiveClipboard();
            }
            catch (Exception)
            {
                CloseProgressBar();
                switchOp.OnEndConnectionToServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
                this.channelMgr.EndConnectionToCurrentServer();
                this.channelMgr.StartNewConnection(computerID);
                switchOp.OnSetNewServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            }            
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
            switchOp.mainWin.fullScreenWin.clipboardTransferProgressBar.Dispatcher.Invoke(new Action(() =>
            {
                switchOp.mainWin.fullScreenWin.clipboardTransferProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }));
            switchOp.mainWin.fullScreenWin.Dispatcher.Invoke(new Action(() =>
            {
                switchOp.mainWin.fullScreenWin.CommandBindings.Add(switchOp.mainWin.fullScreenWin.SwitchCmdBinding);
                switchOp.mainWin.fullScreenWin.CommandBindings.Add(switchOp.mainWin.fullScreenWin.RemotePasteCmdBinding);
            }));
            switchOp.mainWin.Dispatcher.Invoke(new Action(() =>
            {
                switchOp.mainWin.computerList.IsEnabled = true;
            }));
        }        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseProgressBar();
            exit = false;
            this.Close();
            switchOp.OnEndConnectionToServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
            this.channelMgr.EndConnectionToCurrentServer();
            this.channelMgr.StartNewConnection(computerID);
            switchOp.OnSetNewServer(new ServerEventArgs(this.channelMgr.GetCurrentServer()));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (exit)
            {
                CloseProgressBar();
            }
            
        }
    }
}
