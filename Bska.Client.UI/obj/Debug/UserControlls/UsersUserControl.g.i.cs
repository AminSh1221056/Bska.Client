﻿#pragma checksum "..\..\..\UserControlls\UsersUserControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "60B409ECF0660DCB27706313FC245CF6"
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


namespace Bska.Client.UI.UserControlls {
    
    
    /// <summary>
    /// UsersUserControl
    /// </summary>
    public partial class UsersUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainPane1;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txbPersonName;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNewUser;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDelete;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnResetPassword;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BorderAddEdit;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridEdit;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txbUsername;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txbPassword;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txbConfirmPassword;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txbPermissionUseruse;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox pbPassword;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox pbConfirmPassword;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbPermissionUserUse;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView UsersGridView;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.FilterTextUserControl FilterTextBox;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSave;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\UserControlls\UsersUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/usersusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControlls\UsersUserControl.xaml"
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
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.MainPane1 = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.txbPersonName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.btnNewUser = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.btnDelete = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.btnResetPassword = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.BorderAddEdit = ((System.Windows.Controls.Border)(target));
            return;
            case 9:
            this.gridEdit = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            this.txbUsername = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.txbPassword = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.txbConfirmPassword = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.txbPermissionUseruse = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.pbPassword = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 15:
            this.pbConfirmPassword = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 16:
            this.cmbPermissionUserUse = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 17:
            this.UsersGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            case 18:
            this.FilterTextBox = ((Bska.Client.UI.UserControlls.FilterTextUserControl)(target));
            return;
            case 19:
            this.btnSave = ((System.Windows.Controls.Button)(target));
            return;
            case 20:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
