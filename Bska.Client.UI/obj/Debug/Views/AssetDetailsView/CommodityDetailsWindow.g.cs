﻿#pragma checksum "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5F792EFEA7C966523631F615DA5E1394E0E07104"
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
using Bska.Client.UI.UserControlls;
using Bska.Client.UI.Views;
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
    /// CommodityDetailsWindow
    /// </summary>
    public partial class CommodityDetailsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.CommodityDetailsWindow Cowindow;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderProperty;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.CommodityUC commodityPane;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup PopUpSelectProp;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.AutoFilteredComboBox cmbStuffs;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/assetdetailsview/commoditydetailswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
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
            this.Cowindow = ((Bska.Client.UI.Views.CommodityDetailsWindow)(target));
            
            #line 9 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
            this.Cowindow.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Cowindow_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 35 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.borderProperty = ((System.Windows.Controls.Border)(target));
            
            #line 52 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
            this.borderProperty.MouseEnter += new System.Windows.Input.MouseEventHandler(this.borderProperty_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 4:
            this.commodityPane = ((Bska.Client.UI.UserControlls.CommodityUC)(target));
            return;
            case 5:
            this.PopUpSelectProp = ((System.Windows.Controls.Primitives.Popup)(target));
            
            #line 126 "..\..\..\..\Views\AssetDetailsView\CommodityDetailsWindow.xaml"
            this.PopUpSelectProp.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopUpSelectProp_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cmbStuffs = ((Bska.Client.UI.Controls.AutoFilteredComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

