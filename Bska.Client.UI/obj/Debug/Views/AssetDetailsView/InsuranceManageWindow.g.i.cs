﻿#pragma checksum "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C1B5D1ED2776E297474A652048256EB41D2903C5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Bska.Client.Common;
using Bska.Client.UI.API;
using Bska.Client.UI.Controls;
using Bska.Client.UI.Converters;
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
    /// InsuranceManageWindow
    /// </summary>
    public partial class InsuranceManageWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.InsuranceManageWindow mAssetinsuranceWin;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgstuff1;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonImage;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView PropertyGridView;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/assetdetailsview/insurancemanagewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
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
            this.mAssetinsuranceWin = ((Bska.Client.UI.Views.InsuranceManageWindow)(target));
            
            #line 8 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
            this.mAssetinsuranceWin.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.mAssetinsuranceWin_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 40 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.imgstuff1 = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.ButtonImage = ((System.Windows.Controls.Button)(target));
            
            #line 128 "..\..\..\..\Views\AssetDetailsView\InsuranceManageWindow.xaml"
            this.ButtonImage.Click += new System.Windows.RoutedEventHandler(this.ButtonImage_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.PropertyGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
