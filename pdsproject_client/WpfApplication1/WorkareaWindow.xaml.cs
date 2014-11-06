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
        public ChannelManager channelMgr;

        public WorkareaWindow(ChannelManager channelMgr) 
        {
            this.channelMgr = channelMgr;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += WorkareaWindow_KeyDown;            
        }

        public void Show(ChannelManager channelMgr)
        {
            this.channelMgr = channelMgr;
            this.Show();
        }

        private void WorkareaWindow_KeyDown(object sender, KeyEventArgs e)
        {
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
                //prendersi il nome del computer dalla lista
                Thread switchThread = new Thread(new ThreadStart(beginSwitchOperations));
                switchThread.Start();
            }
        }

        private void beginSwitchOperations()
        {
            ClipboardMgr clipboardMgr = new ClipboardMgr();
            clipboardMgr.ChannelMgr = channelMgr;
            if (clipboardMgr.GetClipboardDimensionOverFlow())
            {
                clipboardMgr.ReceiveClipboard();                
                this.channelMgr.EndConnectionToCurrentServer();
                this.channelMgr.StartNewConnection(0);//FAKE e OCCHIO GESTIONE EXCEPTIONS
                //invia clipboard
                //eventi per
                //          cambio feddback visivo in GUI control panel
                //          sblocco interceptevents : delegato piu evento che sblocca hook
            }
            else
            {
                //open window con callback si che fa receiveclipboard e invia clibpoard
                //invia struttura received files + ciclo come in form1 di invio del contenuto della cartella tmp
            }
            
        }
    }
}
