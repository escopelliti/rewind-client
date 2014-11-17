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
        private MainWindow mainWin;
        private bool switchFlag = false;

        public WorkareaWindow(ChannelManager channelMgr, MainWindow mainWin) 
        {
            this.channelMgr = channelMgr;
            InitializeComponent();            
            this.KeyDown += WorkareaWindow_KeyDown;
            this.mainWin = mainWin;
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
                switchFlag = true;
                int serverNum = (KeyInterop.VirtualKeyFromKey(e.Key) - fixedDisplacement);
                ItemCollection items = this.computerList.Items;
                ComputerItem ci = (ComputerItem) items.GetItemAt(serverNum);
                SwitchOperator switchOp = new SwitchOperator(mainWin);
                Thread switchThread = new Thread(() => switchOp.SwitchOperations(ci.ComputerID, channelMgr));
                switchThread.SetApartmentState(ApartmentState.STA);
                switchThread.IsBackground = true;
                switchThread.Start();                
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!switchFlag)
            {
                InterceptEvents.RestartCapture();
            }
        }
    }
}
