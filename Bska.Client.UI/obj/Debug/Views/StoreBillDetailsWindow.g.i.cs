﻿#pragma checksum "..\..\..\Views\StoreBillDetailsWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E86676BB8667CE0D97E47603322A2AE2DD6CA648"
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
using Bska.Client.UI.UserControlls.DraftUC;
using Bska.Client.UI.UserControlls.StoreBillUC;
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


namespace Bska.Client.UI.Views {
    
    
    /// <summary>
    /// StoreBillDetailsWindow
    /// </summary>
    public partial class StoreBillDetailsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.StoreBillDetailsWindow billDetailsWindow;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.DraftUC.StoreDonationDraftUC donationPane;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.DraftUC.StoreOwnedDraftUC ownedPane;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.DraftUC.StoreTransferDraftUC transferPane;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.DraftUC.StoreTrustDraftUC trustPane;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.DraftUC.StorePruchaseDraftUC purchasePane;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\Views\StoreBillDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.StoreBillUC.StoreBillEditUC sbEditUc;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/storebilldetailswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\StoreBillDetailsWindow.xaml"
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
            this.billDetailsWindow = ((Bska.Client.UI.Views.StoreBillDetailsWindow)(target));
            
            #line 6 "..\..\..\Views\StoreBillDetailsWindow.xaml"
            this.billDetailsWindow.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.StoreBillDetailsWindow_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\Views\StoreBillDetailsWindow.xaml"
            this.billDetailsWindow.DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.billDetailsWindow_DataContextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 37 "..\..\..\Views\StoreBillDetailsWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.donationPane = ((Bska.Client.UI.UserControlls.DraftUC.StoreDonationDraftUC)(target));
            return;
            case 4:
            this.ownedPane = ((Bska.Client.UI.UserControlls.DraftUC.StoreOwnedDraftUC)(target));
            return;
            case 5:
            this.transferPane = ((Bska.Client.UI.UserControlls.DraftUC.StoreTransferDraftUC)(target));
            return;
            case 6:
            this.trustPane = ((Bska.Client.UI.UserControlls.DraftUC.StoreTrustDraftUC)(target));
            return;
            case 7:
            this.purchasePane = ((Bska.Client.UI.UserControlls.DraftUC.StorePruchaseDraftUC)(target));
            return;
            case 8:
            this.sbEditUc = ((Bska.Client.UI.UserControlls.StoreBillUC.StoreBillEditUC)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
