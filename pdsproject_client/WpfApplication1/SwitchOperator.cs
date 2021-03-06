﻿using System;

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
            
            DisableCommandOperation();
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
                CloseProgressBar();
                channelMgr.ResetTokenGen();
                return;
            }
            StartProgressBar();
            if (!dimensionOverflow)
            {
                mainWin.Dispatcher.Invoke(new Action(() => mainWin.Close()));
                try
                {
                    clipboardMgr.ReceiveClipboard();
                }
                catch (Exception)
                {
                    CloseProgressBar();
                    OnEndConnectionToServer(new ServerEventArgs(channelMgr.GetCurrentServer()));
                    channelMgr.EndConnectionToCurrentServer();
                    channelMgr.StartNewConnection(computerID);
                    OnSetNewServer(new ServerEventArgs(channelMgr.GetCurrentServer())); 
                }
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

        private void DisableCommandOperation()
        {
            mainWin.fullScreenWin.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.CommandBindings.Remove(mainWin.fullScreenWin.SwitchCmdBinding);
                mainWin.fullScreenWin.CommandBindings.Remove(mainWin.fullScreenWin.RemotePasteCmdBinding);

            }));
            mainWin.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.computerList.IsEnabled = false;
            }));
        }

        private void CloseProgressBar()
        {
            mainWin.fullScreenWin.clipboardTransferLabel.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferLabel.Visibility = System.Windows.Visibility.Hidden;
            }));
            mainWin.fullScreenWin.clipboardTransferProgressBar.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.clipboardTransferProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }));
            mainWin.fullScreenWin.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.fullScreenWin.CommandBindings.Add(mainWin.fullScreenWin.SwitchCmdBinding);
                mainWin.fullScreenWin.CommandBindings.Add(mainWin.fullScreenWin.RemotePasteCmdBinding);
            }));
            mainWin.Dispatcher.Invoke(new Action(() =>
            {
                mainWin.computerList.IsEnabled = true;
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
