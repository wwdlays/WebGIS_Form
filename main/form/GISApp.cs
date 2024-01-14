using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using code.form;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using code.toolbarcontrol;
using ESRI.ArcGIS.Geodatabase;

namespace code
{
    
    public partial class GISApp : Form
    {
        private IHookHelper m_hookHelper = null;
        private IToolbarMenu m_menuMap;
        public IFeatureLayer _editingLayer;
        private IToolbarMenu m_menuLayer;
        private IMapControl3 m_mapControl;
        IWorkspaceEdit _editSpan;
        IMapControl3 mapControl;

        public GISApp()
        {
            InitializeComponent();
            mapControl = axMapControl1.Object as IMapControl3;
            axToolbarControl1.AddItem(new CreateNewDocument(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new OpenDocument(axMapControl1), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new SaveAsDocument(axMapControl1), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new ZoomIn(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new FullExtent(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new addlayer(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new shuxing(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new kj(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new CreateGDB(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new CreateFC(), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new cmdEditStart(axMapControl1, _editingLayer, _editSpan, this), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new cmdEditStop(axMapControl1), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);
            axToolbarControl1.AddItem(new ToolAD(_editSpan, _editingLayer, axMapControl1), -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconAndText);

            m_mapControl = (IMapControl3)axMapControl1.Object;
            m_menuLayer = new ToolbarMenuClass();
            m_menuLayer.AddItem(new cmdLP(), -1, -1, false, ESRI.ArcGIS.SystemUI.esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new RemoveLayers(), -1, 0, false, ESRI.ArcGIS.SystemUI.esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.SetHook(m_mapControl);
        }

        public void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();
            m_hookHelper.Hook = hook;
        }

        private void loadLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(m_hookHelper);
            form2.Show();
            Form3 form3 = new Form3(mapControl);
            form3.Show();
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            IToolbarMenu mapPopMenu = null;
            mapPopMenu = new ToolbarMenu();
            if(e.button == 2)
            {
                //地图视窗右键菜单功能
                mapPopMenu.AddItem(new ControlsSelectFeaturesTool(), -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);//选择要素工具
                mapPopMenu.AddItem(new ControlsZoomToSelectedCommand(), -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapZoomToLastExtentBackCommand(), -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapZoomToLastExtentForwardCommand(), -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.SetHook(mapControl); 
                mapPopMenu.PopupMenu(e.x, e.y, mapControl.hWnd);//弹出显示菜单

            }
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            object unk = null;
            object data = null;
            ILayer layer = null;
            //右击图层名
            if (e.button == 2)
            {
                //定义被选中的对象
                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref unk, ref data);
                //确保对象被选中
                if (item == esriTOCControlItem.esriTOCControlItemMap)
                    axTOCControl1.SelectItem(map, null);
                else
                    axTOCControl1.SelectItem(layer, null);
                m_mapControl.CustomProperty = layer;
                m_menuLayer.PopupMenu(e.x, e.y, axTOCControl1.hWnd);
            }
        }

        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap basicMap = null;
            ILayer layer = null;
            object unk = null;
            object data = null;

            //HitTest(鼠标点击的X坐标，鼠标点击的Y坐标，esriTOCControlItem枚举常量，绑定MapControl的IBasicMap接口
            //被点击的图层，TOCControl的LegendGroup对象，LegendClass在LegendGroup中的Index)
            axTOCControl1.HitTest(e.x, e.y, ref itemType, ref basicMap, ref layer, ref unk, ref data);
            if (e.button == 1)
            {
                if (itemType == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    //取得图例
                    ILegendClass pLegendClass = ((ILegendGroup)unk).get_Class((int)data);
                    //创建符号选择器SymbolSelector实例
                    DlgSS SymbolSelectorFrm = new DlgSS(pLegendClass, layer);
                    if (SymbolSelectorFrm.ShowDialog() == DialogResult.OK)
                    {
                        axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                        pLegendClass.Symbol = SymbolSelectorFrm.pSymbol;
                        axMapControl1.Refresh();
                        axTOCControl1.Update();
                    }
                }
            }
        }
    }
}
