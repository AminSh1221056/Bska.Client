﻿#pragma checksum "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "13D247B326BDCB2FFE1A64EDD9540F63D8C284F9"
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


namespace Bska.Client.UI.UserControlls {
    
    
    /// <summary>
    /// UserRoleUserControl
    /// </summary>
    public partial class UserRoleUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.UserRoleUserControl userControl;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderUserList;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView organizTree;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView StoreGridView;
        
        #line default
        #line hidden
        
        
        #line 222 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup PopUpSelectUser;
        
        #line default
        #line hidden
        
        
        #line 225 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbOrderNo;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/manageruc/userroleusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
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
            this.userControl = ((Bska.Client.UI.UserControlls.UserRoleUserControl)(target));
            return;
            case 2:
            this.borderUserList = ((System.Windows.Controls.Border)(target));
            
            #line 86 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
            this.borderUserList.MouseEnter += new System.Windows.Input.MouseEventHandler(this.borderUserList_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 3:
            this.organizTree = ((System.Windows.Controls.TreeView)(target));
            return;
            case 4:
            this.StoreGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            case 5:
            this.PopUpSelectUser = ((System.Windows.Controls.Primitives.Popup)(target));
            
            #line 222 "..\..\..\..\UserControlls\ManagerUC\UserRoleUserControl.xaml"
            this.PopUpSelectUser.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopUpSelectUser_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cmbOrderNo = ((System.Windows.Controls.ComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

