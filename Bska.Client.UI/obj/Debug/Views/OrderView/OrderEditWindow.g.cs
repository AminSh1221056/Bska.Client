﻿#pragma checksum "..\..\..\..\Views\OrderView\OrderEditWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8448DAC68FE8DFABA975489690AB2F704C19280E7D62BC0C9ADC30E1AB384CB7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Bska.Client.UI.Converters;
using Bska.Client.UI.UserControlls.OrderUC;
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


namespace Bska.Client.UI.Views.OrderView {
    
    
    /// <summary>
    /// OrderEditWindow
    /// </summary>
    public partial class OrderEditWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.OrderView.OrderEditWindow window;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.HelpToolbar helpToolPane;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid rightb1;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.OrderUC.OrderDetailsListUC internalOrderList;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.OrderUC.DisplacementOrderDetailsUC displacementOrderList;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.OrderUC.InternalOrderDetailsUC internalOrdrUc;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStoreOrderfinal;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/orderview/ordereditwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
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
            this.window = ((Bska.Client.UI.Views.OrderView.OrderEditWindow)(target));
            
            #line 6 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
            this.window.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 6 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
            this.window.Loaded += new System.Windows.RoutedEventHandler(this.window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 39 "..\..\..\..\Views\OrderView\OrderEditWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.helpToolPane = ((Bska.Client.UI.UserControlls.Ribon.HelpToolbar)(target));
            return;
            case 4:
            this.rightb1 = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.internalOrderList = ((Bska.Client.UI.UserControlls.OrderUC.OrderDetailsListUC)(target));
            return;
            case 6:
            this.displacementOrderList = ((Bska.Client.UI.UserControlls.OrderUC.DisplacementOrderDetailsUC)(target));
            return;
            case 7:
            this.internalOrdrUc = ((Bska.Client.UI.UserControlls.OrderUC.InternalOrderDetailsUC)(target));
            return;
            case 8:
            this.btnStoreOrderfinal = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

