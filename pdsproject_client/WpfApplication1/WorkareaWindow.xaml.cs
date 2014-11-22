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
using System.Collections.ObjectModel;

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
        private ObservableCollection<ComputerItem> computerItemList;
        private ComputerItem focusedComputerItem;

        public WorkareaWindow(ChannelManager channelMgr, MainWindow mainWin) 
        {
            this.channelMgr = channelMgr;
            InitializeComponent();            
            this.KeyDown += WorkareaWindow_KeyDown;
            this.mainWin = mainWin;
            this.computerItemList = channelMgr.GetComputerItemList();
            this.computerList.ItemsSource = this.computerItemList;
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
                try
                {
                    ItemCollection items = this.computerList.Items;
                    ComputerItem ci = (ComputerItem)items.GetItemAt(serverNum);
                    if (ci == null)
                        return;                    
                    SwitchOperator switchOp = new SwitchOperator(mainWin);
                    Thread switchThread = new Thread(() => switchOp.SwitchOperations(ci.ComputerID, channelMgr));
                    switchThread.SetApartmentState(ApartmentState.STA);
                    switchThread.IsBackground = true;
                    switchThread.Start();
                    switchFlag = true;
                    this.Close();
                }
                catch (InvalidOperationException ex)
                {
                    return;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    return;
                }

            }
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            focusedComputerItem = (ComputerItem)(sender as ListBoxItem).Content;
        } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!switchFlag)
            {
                InterceptEvents.RestartCapture();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (focusedComputerItem != null) 
            {
                SwitchOperator switchOp = new SwitchOperator(mainWin);
                Thread switchThread = new Thread(() => switchOp.SwitchOperations(focusedComputerItem.ComputerID, channelMgr));
                switchThread.SetApartmentState(ApartmentState.STA);
                switchThread.IsBackground = true;
                switchThread.Start();
            }
        }
    }
}
