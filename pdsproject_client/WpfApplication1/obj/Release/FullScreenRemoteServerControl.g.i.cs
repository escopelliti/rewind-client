﻿#pragma checksum "..\..\FullScreenRemoteServerControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C924E329C8DA45E2296A59783203E180"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.18444
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Views {
    
    
    /// <summary>
    /// FullScreenRemoteServerControl
    /// </summary>
    public partial class FullScreenRemoteServerControl : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Views.FullScreenRemoteServerControl fullScreenRemoteServerControl;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label blockCaptureLabel;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label controlPanelShortcutLabel;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label switchServerShortcutLabel;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label remotePasteShortcutLabel;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label currentServerNameLabel;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView connectedComputerList;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label clipboardTransferLabel;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\FullScreenRemoteServerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar clipboardTransferProgressBar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RewindClient;component/fullscreenremoteservercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\FullScreenRemoteServerControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.fullScreenRemoteServerControl = ((Views.FullScreenRemoteServerControl)(target));
            return;
            case 2:
            this.blockCaptureLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.controlPanelShortcutLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.switchServerShortcutLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.remotePasteShortcutLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.currentServerNameLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.connectedComputerList = ((System.Windows.Controls.ListView)(target));
            return;
            case 8:
            this.clipboardTransferLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.clipboardTransferProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

