﻿#pragma checksum "..\..\..\Views\MovableAssetDetailsWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "910E01280E252AA6D2752BE46D3FE5AA"
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
    /// MovableAssetDetailsWindow
    /// </summary>
    public partial class MovableAssetDetailsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Views.MovableAssetDetailsWindow window;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderProperty;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chLocation;
        
        #line default
        #line hidden
        
        
        #line 177 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.AutoFilteredComboBox cmbStuffs;
        
        #line default
        #line hidden
        
        
        #line 186 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.UnConsumptionUserControl unConsumptionPane;
        
        #line default
        #line hidden
        
        
        #line 187 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.InCommodityUC inCommodityPane;
        
        #line default
        #line hidden
        
        
        #line 188 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.BelongingUserControl belongingPane;
        
        #line default
        #line hidden
        
        
        #line 199 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup PopUpSelectProp;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/views/movableassetdetailswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
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
            this.window = ((Bska.Client.UI.Views.MovableAssetDetailsWindow)(target));
            
            #line 6 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
            this.window.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 50 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseCommand_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.borderProperty = ((System.Windows.Controls.Border)(target));
            
            #line 79 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
            this.borderProperty.MouseEnter += new System.Windows.Input.MouseEventHandler(this.borderProperty_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 4:
            this.chLocation = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.cmbStuffs = ((Bska.Client.UI.Controls.AutoFilteredComboBox)(target));
            return;
            case 6:
            this.unConsumptionPane = ((Bska.Client.UI.UserControlls.UnConsumptionUserControl)(target));
            return;
            case 7:
            this.inCommodityPane = ((Bska.Client.UI.UserControlls.InCommodityUC)(target));
            return;
            case 8:
            this.belongingPane = ((Bska.Client.UI.UserControlls.BelongingUserControl)(target));
            return;
            case 9:
            this.PopUpSelectProp = ((System.Windows.Controls.Primitives.Popup)(target));
            
            #line 199 "..\..\..\Views\MovableAssetDetailsWindow.xaml"
            this.PopUpSelectProp.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopUpSelectProp_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 10:
            this.cmbAllProperty = ((System.Windows.Controls.ComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

