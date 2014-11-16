using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationLibrary;

namespace WpfApplication1
{
    public class SwitchOperator
    {
        public delegate void SetNewServerOnGUIEventHandler(Object sender, Object param);
        public event SetNewServerOnGUIEventHandler setNewServerOnGUIHandler;
        public delegate void ResetServerStateOnGUIEventHandler(Object sender, Object param);
        public event ResetServerStateOnGUIEventHandler resetServerStateOnGUIHandler;
        private MainWindow mainWin;

        public SwitchOperator(MainWindow mainWin)
        {
            this.mainWin = mainWin;
            this.resetServerStateOnGUIHandler += mainWin.OnEndConnectionToServer;
            this.setNewServerOnGUIHandler += mainWin.OnSetNewServer;
            this.setNewServerOnGUIHandler += InterceptEvents.OnSetNewServer;
        }

        public void SwitchOperations(int computerID,ChannelManager channelMgr)
        {
            ClipboardMgr clipboardMgr = new ClipboardMgr();
            clipboardMgr.ChannelMgr = channelMgr;
            if (!clipboardMgr.GetClipboardDimensionOverFlow())
            {
                clipboardMgr.ReceiveClipboard();
                OnEndConnectionToServer(new ServerEventArgs(channelMgr.GetCurrentServer()));
                channelMgr.EndConnectionToCurrentServer();
                channelMgr.StartNewConnection(computerID);//OCCHIO GESTIONE EXCEPTIONS
                OnSetNewServer(new ServerEventArgs(channelMgr.GetCurrentServer()));                
                clipboardMgr.SendClipboard();
                channelMgr.ResetTokenGen();                         
            }
            else
            {
                ConfirmDataTransferWindow confirmWin = new ConfirmDataTransferWindow(computerID, clipboardMgr, this, channelMgr);
                confirmWin.Show();
                System.Windows.Threading.Dispatcher.Run();
            }            
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
