﻿#pragma checksum "..\..\..\..\..\Views\StuffHonestView\Windows\DisplacementIndentWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A7BF6B7367DF27C1D98CAFB6CE1BAFF3ECB38A4836D4C89EFDF6B8521233DE57"
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


namespace Bska.Client.UI.Views.StuffHonestView {
    
    
    /// <summary>
    /// DisplacementIndentWindow
    /// </summary>
    public partial class DisplacementIndentWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\..\..\Views\StuffHonestView\Windows\DisplacementIndentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.StuffHonestView.DisplacementIndentWindow indentWindow;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/stuffhonestview/windows/displacementindentwindow." +
                    "xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\StuffHonestView\Windows\DisplacementIndentWindow.xaml"
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
            this.indentWindow = ((Bska.Client.UI.Views.StuffHonestView.DisplacementIndentWindow)(target));
            
            #line 7 "..\..\..\..\..\Views\StuffHonestView\Windows\DisplacementIndentWindow.xaml"
            this.indentWindow.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.indentWindow_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 31 "..\..\..\..\..\Views\StuffHonestView\Windows\DisplacementIndentWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

