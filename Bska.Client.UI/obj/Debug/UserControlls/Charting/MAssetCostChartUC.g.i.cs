﻿#pragma checksum "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "BAB03669BCF3B2BBA5F120CA4C1C310D86DD63B3849B0421BB41862B6F9A4E86"
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
using Bska.Client.UI.UserControlls;
using Bska.Client.UI.UserControlls.Charting.ChartingPane;
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


namespace Bska.Client.UI.UserControlls.Charting {
    
    
    /// <summary>
    /// MAssetCostChartUC
    /// </summary>
    public partial class MAssetCostChartUC : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 32 "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chCommodity;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Ribon.GlobalToolbar globalRiboon;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button reportbtn;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Bska.Client.UI.UserControlls.Charting.ChartingPane.ColumnChartingUC chartingPane;
        
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
            System.Uri resourceLocater = new System.Uri("/Bska.Client.UI;component/usercontrolls/charting/massetcostchartuc.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControlls\Charting\MAssetCostChartUC.xaml"
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
            this.chCommodity = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 2:
            this.globalRiboon = ((Bska.Client.UI.UserControlls.Ribon.GlobalToolbar)(target));
            return;
            case 3:
            this.reportbtn = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.chartingPane = ((Bska.Client.UI.UserControlls.Charting.ChartingPane.ColumnChartingUC)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

