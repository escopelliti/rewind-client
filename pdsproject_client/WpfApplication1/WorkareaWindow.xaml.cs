using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using CommunicationLibrary;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per WorkareaWindow.xaml
    /// </summary>
    public partial class WorkareaWindow : Window
    {
        public ChannelManager channelMgr { get; set; }
        public delegate void SetNewServerOnGUIEventHandler(Object sender, Object param);
        public event SetNewServerOnGUIEventHandler setNewServerOnGUIHandler; 


        public WorkareaWindow(ChannelManager channelMgr) 
        {
            this.channelMgr = channelMgr;
            InitializeComponent();            
            this.setNewServerOnGUIHandler += MainWindow.OnSetNewServer;
            this.setNewServerOnGUIHandler += InterceptEvents.OnSetNewServer;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += WorkareaWindow_KeyDown;            
        }

        public void OnSetNewServer(ServerEventArgs sea)
        {
            SetNewServerOnGUIEventHandler handler = setNewServerOnGUIHandler;
            if (handler != null)
            {
                handler(this, sea);
            }

        }
        
        private void WorkareaWindow_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
            string pattern = @"[0-9]";
            string input = e.Key.ToString();            
            int fixedDisplacement = 48;
            if (Regex.IsMatch(input, pattern))
            {
                if (e.Key >= Key.NumPad0)
                {
                    fixedDisplacement += fixedDisplacement;
                }                
                int serverNum = (KeyInterop.VirtualKeyFromKey(e.Key) - fixedDisplacement);
                ItemCollection items = this.computerList.Items;
                ComputerItem ci = (ComputerItem) items.GetItemAt(serverNum);
                Thread switchThread = new Thread(() => beginSwitchOperations(ci.computerID));
                switchThread.Start();                
            }
        }

        private void beginSwitchOperations(int serverNum)
        {
            ClipboardMgr clipboardMgr = new ClipboardMgr();
            clipboardMgr.ChannelMgr = channelMgr;
            if (!clipboardMgr.GetClipboardDimensionOverFlow())
            {
                clipboardMgr.ReceiveClipboard();                
                this.channelMgr.EndConnectionToCurrentServer();
                this.channelMgr.StartNewConnection(serverNum);//OCCHIO GESTIONE EXCEPTIONS
                OnSetNewServer(new ServerEventArgs(this.channelMgr.getCurrentServer()));
                clipboardMgr.SendClipboard();
                                              
            }
            else
            {
                ConfirmDataTransferWindow confirmWin = new ConfirmDataTransferWindow(serverNum, clipboardMgr, this, channelMgr);
                confirmWin.Show();               
            }
            
        }
    }
}
