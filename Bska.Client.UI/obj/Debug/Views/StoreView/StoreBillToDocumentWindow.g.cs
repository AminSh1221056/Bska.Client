﻿#pragma checksum "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CFE903A7925C04184CAA52AF42F9F733F0CD7BE9DBE202F5D9E47FB13EABC25C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Bska.Client.UI.Controls;
using Bska.Client.UI.Converters;
using Bska.Client.UI.UserControlls.Ribon;
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


namespace Bska.Client.UI.Views.StoreView {
    
    
    /// <summary>
    /// StoreBillToDocumentWindow
    /// </summary>
    public partial class StoreBillToDocumentWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.StoreView.StoreBillToDocumentWindow billTodocWindow;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.StoreDocumentToolbar storeToolPane;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelbtn;
        
        #line default
        #line hidden
        
        
        #line 157 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.GlobalToolbar globalToolPane;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.HelpToolbar helpToolPane;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView assetGridView;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/storeview/storebilltodocumentwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.billTodocWindow = ((Bska.Client.UI.Views.StoreView.StoreBillToDocumentWindow)(target));
            
            #line 9 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
            this.billTodocWindow.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.billTodocWindow_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 88 "..\..\..\..\Views\StoreView\StoreBillToDocumentWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.storeToolPane = ((Bska.Client.UI.UserControlls.Ribon.StoreDocumentToolbar)(target));
            return;
            case 4:
            this.cancelbtn = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.globalToolPane = ((Bska.Client.UI.UserControlls.Ribon.GlobalToolbar)(target));
            return;
            case 6:
            this.helpToolPane = ((Bska.Client.UI.UserControlls.Ribon.HelpToolbar)(target));
            return;
            case 7:
            this.assetGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

