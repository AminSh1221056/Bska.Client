﻿#pragma checksum "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "30692F0647A999B53E09916E814DD12FA250310A"
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
using Bska.Client.UI.Helper;
using Bska.Client.UI.UserControlls;
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


namespace Bska.Client.UI.Views.StuffHonestView {
    
    
    /// <summary>
    /// OrderManagePage
    /// </summary>
    public partial class OrderManagePage : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.StuffHonestView.OrderManagePage honestOrderpage;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRefresh;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.OrderToolbar orderToolPane;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.HelpToolbar helpToolPane;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/stuffhonestview/concreteuc/ordermanagepage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\StuffHonestView\ConcreteUC\OrderManagePage.xaml"
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
            this.honestOrderpage = ((Bska.Client.UI.Views.StuffHonestView.OrderManagePage)(target));
            return;
            case 2:
            this.btnRefresh = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.orderToolPane = ((Bska.Client.UI.UserControlls.Ribon.OrderToolbar)(target));
            return;
            case 4:
            this.helpToolPane = ((Bska.Client.UI.UserControlls.Ribon.HelpToolbar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

