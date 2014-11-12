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
            MyNotifyIcon.Icon = new System.Drawing.Icon("..\\..\\Computers.ico");
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);

            MyNotifyIcon.ContextMenu = this.contextMenu1;

            //leggi da file di conf i server;
            //capisci quali sono quelli accesi;
            //quelli accesi li passi a channelmgr (gli assegnerà un id);
            //assegno l'id a computerItem per quelli accesi solamente (con immagine differente);
            //do la lista di computer item al form;

            List<ComputerItem> items = new List<ComputerItem>();
            items.Add(new ComputerItem() { Name = "TEST_PC", computerNum = "0"});
            items.Add(new ComputerItem() { Name = "INSIDEMYHEAD", ComputerStateImage = "connComputer.png", focusedImage = "tick.png", computerNum = "1", computerID = 0 });
            items.Add(new ComputerItem() { Name = "NEW_PC_PORTABLE", ComputerStateImage = "connComputer.png", computerNum = "2", computerID = 1 });
            computerList.ItemsSource = items;

            /////////////////////////////////////////////////////////////
            //Server enrico = new Server();
            //enrico.ComputerName = "INSIDEMYHEAD";
            //enrico.DataPort = 12001;
            //enrico.CmdPort = 12000;
            
            //Server alessandra = new Server();
            //alessandra.ComputerName = "bernoulli";
            //alessandra.CmdPort = 12000;
            //alessandra.DataPort = 12001;
            //Server alberto = new Server();
            //alberto.CmdPort = 12000;
            //alberto.DataPort = 12001;
            //alberto.ComputerName = "NEW_PC_PORTABLE";

            //ChannelManager cm = new ChannelManager();
            //// TO DO ... Creazione dei server dal file di configurazione del client
            ////cm.addServer(alessandra);
            //cm.addServer(enrico);
            //cm.addServer(alberto);
            //cm.setCurrentServer(enrico);            
            
            InterceptEvents ie = new InterceptEvents(/*cm*/);
            // TO DO ... Aprire il file di cinfigurazione e ricavare la lista di hotkey da registrare!!!!!
            
            List <Hotkey> l = new List<Hotkey>();
            l.Add(new Hotkey(ModifierKeys.Alt, Key.A, Hotkey.SWITCH_SERVER_CMD));
            l.Add(new Hotkey(ModifierKeys.Alt, Key.B, Hotkey.OPEN_PANEL_CMD));
            OpenFullScreenWondows(ie, l);

            //Discovery.ServiceDiscovery sd = new Discovery.ServiceDiscovery();
           
        }

        private void OpenFullScreenWondows(InterceptEvents ie, List<Hotkey> hotkeyList)
        {
            FullScreenRemoteServerControl fullScreenWin = new FullScreenRemoteServerControl(ie, hotkeyList/*, channelMgr.getCurrentServer(), channelMgr.ConnectedServer*/);
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
                MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                MyNotifyIcon.BalloonTipText = "Minimized the app ";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // AT FIRST IT ASKS YOU A PSW
            //CODE TO SWITCH SERVER OR MAKE CURRENT THAT SPECIFIC SERVER
        }

        public static void OnSetNewServer(Object obj, Object ea)
        {
            ServerEventArgs sea = (ServerEventArgs)ea;
            Server server = sea.Server;

            //illuminami o dammi feedback su questo nuovo server 
        }
    }
}
