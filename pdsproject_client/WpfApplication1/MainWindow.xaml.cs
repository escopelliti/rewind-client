﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunicationLibrary;
using System.Windows.Interop;

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Collections.ObjectModel;

namespace WpfApplication1
{

    
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private bool exit = false;
        private Discovery.ServiceDiscovery sd;
        public ObservableCollection<ComputerItem> computerItemList { get; set; }
        public ChannelManager channelMgr { get; set; }
        private ComputerItem focusedComputerItem;
        private List<Server> serverList;
        public FullScreenRemoteServerControl fullScreenWin;
        private ConfigurationManager configurationMgr;

        public MainWindow()
        {
            InitializeComponent();

            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem1 });

            // Initialize menuItem1 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(@"../../resources/images/Computers.ico");
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            MyNotifyIcon.ContextMenu = this.contextMenu1;
            computerItemList = new ObservableCollection<ComputerItem>();
            computerList.ItemsSource = computerItemList;

            // I server su cui è attiva l'applicazione socperti dal modulo di discovery vengono aggiunti alla lista computerItemList
            

            //Carichiamo quella attivo con l'immagine corretta e con i relativi tasti disabilitati



            serverList = new List<Server>();

            configurationMgr = new ConfigurationManager();                
            channelMgr = new ChannelManager();
            StartDiscovery();
            //InterceptEvents ie = new InterceptEvents(channelMgr);
            //OpenFullScreenWindow(ie, l, channelMgr);              
            
        }

        private void StartDiscovery()
        {
            sd = new Discovery.ServiceDiscovery(this);            
        }

        public void OnNewComputerConnected(Object sender, Object param) {

            Server server = (Server)param;
            serverList.Add(server);

            int lastComputerNum = -1;
            if (this.computerItemList.Count != 0)
            {
                lastComputerNum = this.computerItemList.Last<ComputerItem>().ComputerNum;
            }
            lastComputerNum += 1;
            this.computerList.Dispatcher.Invoke(new Action(() =>
            {                
                this.computerItemList.Add(new ComputerItem() { Name = server.ComputerName, ComputerStateImage = @"resources/images/off.png", ComputerNum = lastComputerNum, IsCheckboxEnabled=true});
            }));
        }

        private void OpenFullScreenWindow(InterceptEvents ie, List<Hotkey> hotkeyList, ChannelManager channelMgr)
        {
            fullScreenWin = new FullScreenRemoteServerControl(ie, hotkeyList, channelMgr.GetCurrentServer(), channelMgr.ConnectedServer);
            fullScreenWin.Show();
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // TO BE CHANGED BUT IT WORKS
            bool isWindowOpen = false;

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is AddComputerWindow)
                {
                    isWindowOpen = true;
                    win.Activate();
                }
            }

            if (!isWindowOpen)
            {
                AddComputerWindow w = new AddComputerWindow();
                w.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!exit)
            {
                this.WindowState = WindowState.Minimized;
                e.Cancel = true;
            }            
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            exit = true;
            this.Close();
        }

        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        //TO BE CHANGED
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;

                //TO BE CHANGED
                //MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                //MyNotifyIcon.BalloonTipText = "Minimized the app ";
                //MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        public static void OnSetNewServer(Object obj, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server server = sea.Server;

            //illuminami o dammi feedback su questo nuovo server 
        }

        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            serverList.Remove(server);
            ComputerItem toRemove = this.computerItemList.Where(x => x.Name == server.ComputerName).First<ComputerItem>();
            if (toRemove != null)
            {
                this.computerList.Dispatcher.Invoke(new Action(() =>
                {
                    this.computerItemList.Remove(toRemove);
                }));
            }
        }


        private void SetActiveButton_Click(object sender, RoutedEventArgs e)
        {
            // AT FIRST IT ASKS YOU A PSW
            //CODE TO SWITCH SERVER OR MAKE CURRENT THAT SPECIFIC SERVER
            if (!focusedComputerItem.IsCheckboxChecked)
            {
                System.Windows.MessageBox.Show
                    ("Connettiti al computer prima di continuare", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Server currentServer = channelMgr.GetCurrentServer();
            Server s = serverList.Find(x => x.ComputerName == focusedComputerItem.Name);

            if (currentServer == null)
            {
                //channelMgr.AssignChannel(s);
                //channelMgr.AddServer(s);
                channelMgr.SetCurrentServer(s);
            }
            else
            {
                if (focusedComputerItem.Name == currentServer.ComputerName)
                {
                    // L'utente vuole attivare il focus su un server su cui già è attivo il focus
                    return;
                }

                //verifico che il computer su cui voglio attivare il focus è connesso
                SwitchOperator switchOp = new SwitchOperator();
                Thread switchThread = new Thread(() => switchOp.SwitchOperations(focusedComputerItem.ComputerID, channelMgr));
                switchThread.SetApartmentState(ApartmentState.STA);
                switchThread.IsBackground = true;
                switchThread.Start();
            }
           


            //al momento con c'è alcun focus settato sui server
           
            bool isWindowOpened = false;
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).UpdateCurrentServer(s);
                        isWindowOpened = true;
                        break;
                    }
                }
            }

            if(!isWindowOpened)
            {
                List<Hotkey> l = configurationMgr.ReadConfiguration().hotkeyList;
                InterceptEvents ie = new InterceptEvents(channelMgr);
                OpenFullScreenWindow(ie, l, channelMgr);
            }

            this.computerList.Dispatcher.Invoke(new Action(() =>
            {
                this.computerItemList.Remove(focusedComputerItem);
                focusedComputerItem.ComputerStateImage = @"resources/images/connComputer.png";
                focusedComputerItem.IsCheckboxEnabled = false;
                this.computerItemList.Add(focusedComputerItem);
            }));
            //AGGIORNARE UI - TO BE TESTED!!!!!!!
            
       
        }

        private void connectCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            this.focusedComputerItem.IsCheckboxChecked = true;
            Server s = serverList.Find(x => x.ComputerName == focusedComputerItem.Name);
            channelMgr.AssignChannel(s);
            channelMgr.AddServer(s);
            //testare se aggiorna effettivamente l'elemento nella UI
            focusedComputerItem.ComputerID = s.ServerID;
            

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).AddServerToList(s);
                        break;
                    }
                }
            }
        }

        private void connectCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.focusedComputerItem.IsCheckboxChecked = false;            
            Server s = serverList.Find(x => x.ComputerName == focusedComputerItem.Name);
            channelMgr.DeleteServer(s);

            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win is FullScreenRemoteServerControl)
                {
                    if (win.IsActive)
                    {
                        ((FullScreenRemoteServerControl)win).RemoveServerToList(s);
                        break;
                    }
                }
            }
        }

        private void ListBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            focusedComputerItem = (ComputerItem)(sender as ListBoxItem).Content;
        }    
    }
}

//  controllo dei channel per non fare piu volte l'assign channel
//  aggiornare le interfacce con l'invoke
//  switch server prima di fare offf