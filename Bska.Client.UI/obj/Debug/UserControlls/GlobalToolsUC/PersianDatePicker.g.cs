﻿#pragma checksum "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "339D4D8DEB5BF25E04CFCD78643713A01B0E4BB8DD7F14763DADC43CE036D301"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace Bska.Client.UI.UserControlls {
    
    
    /// <summary>
    /// PersianDatePicker
    /// </summary>
    public partial class PersianDatePicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 77 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button openCalendarButton;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox dateTextBox;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup persianCalnedarPopup;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid persianCalendarGrid;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.PersianCalendar persianCalendar;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/globaltoolsuc/persiandatepicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
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
            this.openCalendarButton = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
            this.openCalendarButton.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.dateTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 84 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
            this.dateTextBox.LostFocus += new System.Windows.RoutedEventHandler(this.dateTextBox_LostFocus);
            
            #line default
            #line hidden
            
            #line 84 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
            this.dateTextBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.dateTextBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.persianCalnedarPopup = ((System.Windows.Controls.Primitives.Popup)(target));
            
            #line 86 "..\..\..\..\UserControlls\GlobalToolsUC\PersianDatePicker.xaml"
            this.persianCalnedarPopup.Opened += new System.EventHandler(this.persianCalnedarPopup_Opened);
            
            #line default
            #line hidden
            return;
            case 4:
            this.persianCalendarGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.persianCalendar = ((Bska.Client.UI.UserControlls.PersianCalendar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

