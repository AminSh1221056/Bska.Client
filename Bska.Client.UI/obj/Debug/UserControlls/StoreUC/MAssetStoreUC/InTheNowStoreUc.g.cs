﻿#pragma checksum "..\..\..\..\..\UserControlls\StoreUC\MAssetStoreUC\InTheNowStoreUc.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7E3B8B31840AE47F87CCC2C44184D767EA771BA0CCCF33B0CC5A73D064142202"
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


namespace Bska.Client.UI.UserControlls.MAssetStoreUC {
    
    
    /// <summary>
    /// InTheNowStoreUc
    /// </summary>
    public partial class InTheNowStoreUc : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\..\UserControlls\StoreUC\MAssetStoreUC\InTheNowStoreUc.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.MAssetStoreUC.InTheNowStoreUc inTheNowUc;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\UserControlls\StoreUC\MAssetStoreUC\InTheNowStoreUc.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.Controls.SortableListView assetGridView;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/storeuc/massetstoreuc/inthenowstoreuc.xam" +
                    "l", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\UserControlls\StoreUC\MAssetStoreUC\InTheNowStoreUc.xaml"
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
            this.inTheNowUc = ((Bska.Client.UI.UserControlls.MAssetStoreUC.InTheNowStoreUc)(target));
            return;
            case 2:
            this.assetGridView = ((Bska.Client.UI.Controls.SortableListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

