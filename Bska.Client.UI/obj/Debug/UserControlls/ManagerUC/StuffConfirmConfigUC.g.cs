﻿#pragma checksum "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "350B4366C41CEC05845CCAD61415E5421F61033D817DE7450209A8B7F32AEFBA"
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


namespace Bska.Client.UI.UserControlls.ManagerUC {
    
    
    /// <summary>
    /// StuffConfirmConfigUC
    /// </summary>
    public partial class StuffConfirmConfigUC : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.ManagerUC.StuffConfirmConfigUC stuffConfigUc;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView StuffView;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid organizStuffTree;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView SbEditGridView;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/manageruc/stuffconfirmconfiguc.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
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
            this.stuffConfigUc = ((Bska.Client.UI.UserControlls.ManagerUC.StuffConfirmConfigUC)(target));
            return;
            case 2:
            this.StuffView = ((System.Windows.Controls.TreeView)(target));
            
            #line 35 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
            this.StuffView.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.StuffView_SelectedItemChanged);
            
            #line default
            #line hidden
            
            #line 35 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
            this.StuffView.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.StuffView_PreviewMouseDown);
            
            #line default
            #line hidden
            
            #line 35 "..\..\..\..\UserControlls\ManagerUC\StuffConfirmConfigUC.xaml"
            this.StuffView.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.StuffView_PreviewMouseMove);
            
            #line default
            #line hidden
            return;
            case 3:
            this.organizStuffTree = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.SbEditGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

