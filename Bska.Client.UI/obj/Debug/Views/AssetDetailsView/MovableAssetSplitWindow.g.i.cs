﻿#pragma checksum "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "FB688BBCEE61248B88589564E6FDFC56B307FBC99F12C46BA47C4608B6D6FE5C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Bska.Client.UI.UserControlls.MAssetDetailsUC.MAssetInfstractureUC;
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
    /// MovableAssetSplitWindow
    /// </summary>
    public partial class MovableAssetSplitWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.MovableAssetSplitWindow locwindow;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.MAssetDetailsUC.MAssetInfstractureUC.MAssetSplitUC splitUc;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/assetdetailsview/movableassetsplitwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml"
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
            this.locwindow = ((Bska.Client.UI.Views.MovableAssetSplitWindow)(target));
            
            #line 5 "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml"
            this.locwindow.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 35 "..\..\..\..\Views\AssetDetailsView\MovableAssetSplitWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.splitUc = ((Bska.Client.UI.UserControlls.MAssetDetailsUC.MAssetInfstractureUC.MAssetSplitUC)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

