using System;

using ConnectionModule;
using GenericDataStructure;
using Views;
using MainApp;
using Clipboard;
using KeyboardMouseController.HookMgr;

namespace Switch
{
    public class SwitchOperator
    {
        public delegate void SetNewServerOnGUIEventHandler(Object sender, Object param);
        public event SetNewServerOnGUIEventHandler setNewServerOnGUIHandler;
        public delegate void ResetServerStateOnGUIEventHandler(Object sender, Object param);
        public event ResetServerStateOnGUIEventHandler resetServerStateOnGUIHandler;
        public MainWindow mainWin { get; set; }

        public SwitchOperator(MainWindow mainWin)
        {
            this.mainWin = mainWin;
            this.resetServerStateOnGUIHandler += mainWin.OnEndConnectionToServer;
            this.setNewServerOnGUIHandler += mainWin.OnSetNewServer;
            this.setNewServerOnGUIHandler += mainWin.fullScreenWin.UpdateCurrentServer;            
            this.setNewServerOnGUIHandler += InterceptEvents.OnSetNewServer;
        }

        public void ExecSwitch(int computerID,ChannelManager channelMgr)
        {

            ClipboardMgr clipboardMgr = new ClipboardMgr();
            clipboardMgr.ChannelMgr = channelMgr;
            bool dimensionOverflow = false;
            try
            {
                dimensionOverflow = clipboardMgr.GetClipboardDimensionOverFlow();
            }
            catch (Exception) 
            {
                OnEndConnectionToServer(new ServerEventArgs(channelMgr.GetCurrentServer()));
                channelMgr.EndConnectionToCurrentServer();
                channelMgr.StartNewConnection(computerID);
                OnSetNewServer(new ServerEventArgs(channelMgr.GetCurrentServer()));
                channelMgr.ResetTokenGen();
                return;
            }
            StartProgressBar();
            if (!dimensionOverflow)
            {
                mainWin.Dispatcher.Invoke(new Action(() => mainWin.Close())); 
                clipboardMgr.ReceiveClipboard();
                OnEndConnectionToServer(new ServerEventArgs(channelMgr.GetCurrentServer()));
                channelMgr.EndConnectionToCurrentServer();
                channelMgr.StartNewConnection(computerID);
                OnSetNewServer(new ServerEventArgs(channelMgr.GetCurrentServer()));                
                clipboardMgr.SendClipboard();
                CloseProgressBar();
                channelMgr.ResetTokenGen();                     
            }
            else
            {
                mainWin.Dispatcher.Invoke(new Action(() => mainWin.Close()));
                ConfirmDataTransferWindow confirmWin = new ConfirmDataTransferWindow(computerID, clipboardMgr, this, channelMgr);
                confirmWin.Show();                
                System.Windows.Threading.Dispatcher.Run();
            }
            
        }

        private void CloseProgressBar()
        {
            mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferLabel.Visibility = System.Windows.Visibility.Hidden;
            }));
            mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        private void StartProgressBar()
        {
            mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferLabel.Visibility = System.Windows.Visibility.Visible;
            }));
            mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferProgressBar.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        public void OnSetNewServer(ServerEventArgs sea)
        {
            SetNewServerOnGUIEventHandler handler = setNewServerOnGUIHandler;
            if (handler != null)
            {
                handler(this, sea);
            }
        }

        public void OnEndConnectionToServer(ServerEventArgs sea)
        {
            ResetServerStateOnGUIEventHandler handler = resetServerStateOnGUIHandler;
            if (handler != null)
            {
                handler(this, sea);
            }
        }
    }
}
