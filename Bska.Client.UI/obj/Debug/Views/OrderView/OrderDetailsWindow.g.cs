﻿#pragma checksum "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F7B094DD4455219D189538CB2303226EA3CFD042"
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
    /// OrderDetailsWindow
    /// </summary>
    public partial class OrderDetailsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.OrderView.OrderDetailsWindow window;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderProperty;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.OrderUC.InternalOrderDetailsUC orderDetails;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup PopUpSelectProp;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbAllProperty;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/orderview/orderdetailswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
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
            this.window = ((Bska.Client.UI.Views.OrderView.OrderDetailsWindow)(target));
            
            #line 6 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
            this.window.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 31 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.borderProperty = ((System.Windows.Controls.Border)(target));
            
            #line 48 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
            this.borderProperty.MouseEnter += new System.Windows.Input.MouseEventHandler(this.borderProperty_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 4:
            this.orderDetails = ((Bska.Client.UI.UserControlls.OrderUC.InternalOrderDetailsUC)(target));
            return;
            case 5:
            this.PopUpSelectProp = ((System.Windows.Controls.Primitives.Popup)(target));
            
            #line 74 "..\..\..\..\Views\OrderView\OrderDetailsWindow.xaml"
            this.PopUpSelectProp.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopUpSelectProp_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cmbAllProperty = ((System.Windows.Controls.ComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
