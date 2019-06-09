﻿#pragma checksum "..\..\OrderMainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "FFB9BE47C33CB91C97C9AA1C304152B9B6F013F795D3F92CC07811CAFFC99957"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Windows.Themes;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace Bska.Client.UI {
    
    
    /// <summary>
    /// OrderMainWindow
    /// </summary>
    public partial class OrderMainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.OrderMainWindow window;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border HeaderBorder;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainPane;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridMainArea;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame frame;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExpand;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnContract;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LeftPane;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border LeftBorder;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridSmallArea;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\OrderMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LbxMenu;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/ordermainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\OrderMainWindow.xaml"
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
            this.window = ((Bska.Client.UI.OrderMainWindow)(target));
            
            #line 6 "..\..\OrderMainWindow.xaml"
            this.window.Loaded += new System.Windows.RoutedEventHandler(this.window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.HeaderBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.MainPane = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 5:
            this.gridMainArea = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.frame = ((System.Windows.Controls.Frame)(target));
            return;
            case 7:
            this.btnExpand = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.btnContract = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.LeftPane = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            this.LeftBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 11:
            this.gridSmallArea = ((System.Windows.Controls.Grid)(target));
            return;
            case 12:
            this.LbxMenu = ((System.Windows.Controls.ListBox)(target));
            
            #line 105 "..\..\OrderMainWindow.xaml"
            this.LbxMenu.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.LbxMenu_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

