﻿#pragma checksum "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4550F767F5B301B247C5671887E31D5492E8E4246CD5DF84DDB66BFF0EFF9C51"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// PersianCalendar
    /// </summary>
    public partial class PersianCalendar : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 108 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle borderRectangle;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button titleButton;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button previousButton;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button nextButton;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.UniformGrid monthUniformGrid;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.UniformGrid yearUniformGrid;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.UniformGrid decadeUniformGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/globaltoolsuc/persiancalendar.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
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
            this.borderRectangle = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 2:
            this.titleButton = ((System.Windows.Controls.Button)(target));
            
            #line 109 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
            this.titleButton.Click += new System.Windows.RoutedEventHandler(this.titleButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.previousButton = ((System.Windows.Controls.Button)(target));
            
            #line 110 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
            this.previousButton.Click += new System.Windows.RoutedEventHandler(this.previousButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.nextButton = ((System.Windows.Controls.Button)(target));
            
            #line 111 "..\..\..\..\UserControlls\GlobalToolsUC\PersianCalendar.xaml"
            this.nextButton.Click += new System.Windows.RoutedEventHandler(this.nextButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.monthUniformGrid = ((System.Windows.Controls.Primitives.UniformGrid)(target));
            return;
            case 6:
            this.yearUniformGrid = ((System.Windows.Controls.Primitives.UniformGrid)(target));
            return;
            case 7:
            this.decadeUniformGrid = ((System.Windows.Controls.Primitives.UniformGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

