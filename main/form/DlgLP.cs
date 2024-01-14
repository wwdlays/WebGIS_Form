using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;

namespace code.form
{
    public partial class DlgLP : Form
    {
        ISimpleRenderer m_sRen;
        IUniqueValueRenderer m_UVRen;
        ISymbol[] m_Symbols;
        string[] m_Labels;
        ILayer layer;
        ILegendClass pLegendClass;
        IHookHelper m_Hookhelper;
        AxMapControl axMapControl1;
        ILayerFields m_layerfields;
        IGeoFeatureLayer geoFeatureLayer;
        
        public DlgLP(IHookHelper hookHelper,DlgSS dlgss)
        {
            InitializeComponent();
            this.m_Hookhelper = hookHelper;
            //找到当前操作的layer
            IMapControl3 m_mapControl = (IMapControl3)hookHelper.Hook;
            layer = (ILayer)m_mapControl.CustomProperty;
            //通过一系列的接口把ILayer转为ILegendClass
            IFeatureLayer pFeatureLayer = layer as IFeatureLayer;
            ILegendInfo lengendInfo = (ILegendInfo)pFeatureLayer;
            ILegendGroup legendGroup = lengendInfo.get_LegendGroup(0);
            pLegendClass = legendGroup.get_Class(0); //获取到LegendClass  
            //获取axmapcontrol
            axMapControl1 = Control.FromHandle(new IntPtr(this.m_Hookhelper.ActiveView.ScreenDisplay.hWnd)) as AxMapControl;
            geoFeatureLayer = layer as IGeoFeatureLayer;
            //为m_sRen赋初值
            if ((geoFeatureLayer.Renderer is SimpleRenderer))
            {
                m_sRen = geoFeatureLayer.Renderer as ISimpleRenderer;
            }
            this.pictureBox1.Image = dlgss.CurrentImage;
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            if (pLegendClass is UniqueValueRendererClass)
            {
                m_sRen = new SimpleRendererClass();
                m_sRen.Symbol = default;

            }
            DlgSS dlgss = new DlgSS(pLegendClass, layer);

            IStyleGalleryItem styleGalleryItem = null;
            if (dlgss.ShowDialog() == DialogResult.OK)
            {
                //m_sRen.Symbol = pLegendClass.Symbol;


                //从symbolForm中获取样式

                styleGalleryItem = dlgss.GetItem();
                if (styleGalleryItem == null)
                {
                    return;
                }

                m_sRen.Symbol = (ISymbol)styleGalleryItem.Item;
                //styleGalleryItem中的符号设置为预览的背景图片
                Bitmap b = dlgss.Sym2Bitmap(m_sRen.Symbol, 32, 32);
                pictureBox1.Image = (Image)b;
                pictureBox1.Text = "";
            }
            //DlgSS dlgss = new DlgSS(pLegendClass, layer);
            ////ISimpleRenderer simpleRenderer = (ISimpleRenderer)m_featLayer.Renderer;
            //IStyleGalleryItem styleGalleryItem = null;
            //styleGalleryItem = dlgss.GetItem(esriSymbologyStyleClass.esriStyleClassMarkerSymbols,m_sRen.Symbol);
            //if (styleGalleryItem == null)
            //{
            //    return;
            //}
            //m_sRen = new SimpleRendererClass();
            //ISymbol pSym = (ISymbol)styleGalleryItem.Item;
            //IMarkerSymbol pMarkSym = (IMarkerSymbol)pSym;
            //m_sRen.Symbol = pSym;
            //Bitmap b = sym2Bitmap(pSym, (int)pMarkSym.Size, (int)pMarkSym.Size);
            //button4.Image = (Image)b;
            ////Set the renderer into the geoFeatureLayer
            //// m_featRender = (IFeatureRenderer)simpleRenderer;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = listBox1.SelectedIndex;
            if (listBox1.SelectedIndex == 1)
            {
                setComboBoxValues();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = tabControl1.SelectedIndex;
            if (tabControl1.SelectedIndex == 1)
            {
                setComboBoxValues();
            }
        }
        private void UpdateListView(string sField)
        {
            listView1.LargeImageList = imageList1;
            ListViewItem item;
            listView1.Items.Clear();
            m_UVRen = CreateUVRen(sField);
            int vCount = m_UVRen.ValueCount;
            //m_Symbols = new ISymbol[vCount - 1];
            //m_Labels = new string[vCount - 1];

            m_Symbols = new ISymbol[vCount];
            m_Labels = new string[vCount];

            imageList1.Images.Clear();

            for (int i = 0; i < vCount; i++)
            {
                string sValue = m_UVRen.get_Value(i);
                switch (((IFeatureLayer)layer).FeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        IMarkerSymbol pSym;
                        pSym = m_UVRen.get_Symbol(sValue) as IMarkerSymbol;
                        m_Symbols[i] = pSym as ISymbol;
                        m_Labels[i] = m_UVRen.get_Label(sValue);
                        Bitmap b = sym2Bitmap((ISymbol)pSym, 50, 50);
                        imageList1.Images.Add(b);

                        item = new ListViewItem(sValue);
                        item.SubItems.Add(m_Labels[i]);
                        item.ImageIndex = i;
                        listView1.Items.Add(item);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        ILineSymbol pSym2;
                        pSym2 = m_UVRen.get_Symbol(sValue) as ILineSymbol;
                        m_Symbols[i] = pSym2 as ISymbol;
                        m_Labels[i] = m_UVRen.get_Label(sValue);
                        Bitmap b2 = sym2Bitmap((ISymbol)pSym2, 50, 50);
                        imageList1.Images.Add(b2);

                        item = new ListViewItem(sValue);
                        item.SubItems.Add(m_Labels[i]);
                        item.ImageIndex = i;
                        listView1.Items.Add(item);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        ILineSymbol pSym3;
                        pSym3 = m_UVRen.get_Symbol(sValue) as ILineSymbol;
                        m_Symbols[i] = pSym3 as ISymbol;
                        m_Labels[i] = m_UVRen.get_Label(sValue);
                        Bitmap b3 = sym2Bitmap((ISymbol)pSym3, 50, 50);
                        imageList1.Images.Add(b3);

                        item = new ListViewItem(sValue);
                        item.SubItems.Add(m_Labels[i]);
                        item.ImageIndex = i;
                        listView1.Items.Add(item);
                        break;
                }

            }

        }

        private IUniqueValueRenderer CreateUVRen(string sField)
        {
            int nnClasses = 0;
            System.Collections.IEnumerator pEnum = SortTable((IFeatureLayer)layer, sField);
            pEnum.Reset();
            while (pEnum.MoveNext())
            {
                object myObject = pEnum.Current;
                System.Math.Max(System.Threading.Interlocked.Increment(ref nnClasses), nnClasses - 1);
            }
            IColorRamp colorRamp = new RandomColorRampClass();
            colorRamp.Size = nnClasses;
            bool createRamp = true;
            colorRamp.CreateRamp(out createRamp);
            IEnumColors enumColors = colorRamp.Colors;
            enumColors.Reset();
            ISymbol pSym;
            IUniqueValueRenderer pUVRenderer = new UniqueValueRendererClass();
            pUVRenderer.FieldCount = 1;
            pUVRenderer.set_Field(0, sField);
            System.Collections.IEnumerator pEnum2 = SortTable((IFeatureLayer)layer, sField);
            string value;
            object myObj;
            while (pEnum2.MoveNext())
            {
                myObj = pEnum2.Current;
                value = myObj.ToString();

                // 根据要素类型选择合适的符号类别
                switch (((IFeatureLayer)layer).FeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        pSym = new SimpleMarkerSymbolClass();
                        ((ISimpleMarkerSymbol)pSym).Size = 8;
                        ((ISimpleMarkerSymbol)pSym).Style = esriSimpleMarkerStyle.esriSMSCircle;
                        ((ISimpleMarkerSymbol)pSym).Color = enumColors.Next();
                        ((ISimpleMarkerSymbol)pSym).Outline = true;
                        ((ISimpleMarkerSymbol)pSym).OutlineSize = 0.4;
                        break;

                    case esriGeometryType.esriGeometryPolyline:
                        pSym = new SimpleLineSymbolClass();
                        ((ISimpleLineSymbol)pSym).Width = 1;
                        ((ISimpleLineSymbol)pSym).Color = enumColors.Next();
                        break;

                    case esriGeometryType.esriGeometryPolygon:
                        pSym = new SimpleFillSymbolClass();
                        ((ISimpleFillSymbol)pSym).Outline = new SimpleLineSymbolClass() { Width = 1, Color = enumColors.Next() };
                        ((ISimpleFillSymbol)pSym).Color = enumColors.Next();
                        break;


                    default:
                        pSym = new SimpleMarkerSymbolClass();
                        break;
                }

                pUVRenderer.AddValue(value, value, pSym);
            }
            return pUVRenderer;
        }

        private System.Collections.IEnumerator SortTable(IFeatureLayer pFeatureLayer, string sFieldName)
        {
            ITableSort pTablesort = new TableSortClass();

            pTablesort.Fields = sFieldName;
            pTablesort.set_Ascending(sFieldName, true);
            pTablesort.set_CaseSensitive(sFieldName, false);
            pTablesort.Table = pFeatureLayer as ITable;

            pTablesort.Sort(null);
            ICursor pCursor = pTablesort.Rows;
            IDataStatistics pDataStatistics = new DataStatisticsClass();
            pDataStatistics.Field = sFieldName;
            pDataStatistics.Cursor = pCursor;
            return pDataStatistics.UniqueValues;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateListView(comboBox1.SelectedItem.ToString());
        }

        private void setComboBoxValues()
        {
            IField m_field;
            int fieldType;

            this.m_layerfields = layer as ILayerFields;
            for (int i = 0; i < m_layerfields.FieldCount; i++)
            {
                m_field = m_layerfields.get_Field(i);
                fieldType = (int)m_field.Type;
                if (fieldType == 7 || fieldType == 6)//esriFieldType=7表示esriFieldTypeGeometry,6表示OID
                {
                    continue;//不显示shape、OID字段
                }
                comboBox1.Items.Add(m_field.Name);
            }
            if (comboBox1.Items.Count > 0)//设置默认选择ObjectID
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_sRen != null)
            {
                pLegendClass.Symbol = m_sRen.Symbol;
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                if (m_UVRen != null)
                {
                    geoFeatureLayer.Renderer = (IFeatureRenderer)m_UVRen;
                }
            }
            IActiveView activeView = m_Hookhelper.ActiveView;
            activeView.Refresh();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_sRen != null)
            {
                pLegendClass.Symbol = m_sRen.Symbol;
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                if (m_UVRen != null)
                {
                    geoFeatureLayer.Renderer = (IFeatureRenderer)m_UVRen;
                }
            }
            IActiveView activeView = m_Hookhelper.ActiveView;
            activeView.Refresh();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_sRen != null)
            {
                pLegendClass.Symbol = m_sRen.Symbol;
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                if (m_UVRen != null)
                {
                    geoFeatureLayer.Renderer = (IFeatureRenderer)m_UVRen;
                }
            }
            IActiveView activeView = m_Hookhelper.ActiveView;
            activeView.Refresh();
        }
        private Bitmap sym2Bitmap(ISymbol sym, int width, int height)
        {
            Bitmap b = new Bitmap(width + 3, height + 3);
            IDisplayTransformation dispTrans = new DisplayTransformationClass();
            tagRECT r = new tagRECT();
            r.left = 0;
            r.top = 0;
            r.bottom = b.Height;
            r.right = b.Width;
            dispTrans.set_DeviceFrame(r);
            IEnvelope bounds = new EnvelopeClass();
            bounds.PutCoords(0, 0, b.Width, b.Height);
            dispTrans.Bounds = bounds;
            IGeometry geom = makeGeometry(sym, bounds);
            Graphics g = Graphics.FromImage(b);
            IntPtr hDC = g.GetHdc();
            sym.SetupDC(hDC.ToInt32(), dispTrans);
            sym.Draw(geom);
            sym.ResetDC();
            g.ReleaseHdc(hDC);
            return b;
        }


        private IGeometry makeGeometry(ISymbol sym, IEnvelope env)
        {

            if (sym is IMarkerSymbol)           //如果符号是点符号
            {
                return ((IArea)env).Centroid;   //返回范围中心点
            }
            else if (sym is ILineSymbol)        //如果符号为线符号
            {
                object missing = Type.Missing;  //创建一个表示缺失值的对象
                IPointCollection pc = new PolylineClass() as IPointCollection;
                pc.AddPoint(env.LowerLeft, missing, missing);
                pc.AddPoint(env.UpperRight, missing, missing);
                return (IGeometry)pc;           // 创建一个折线，包含范围的 LowerLeft 和 UpperRight 两个点
            }
            else if (sym is IFillSymbol)
            {
                ISegmentCollection sc = new PolygonClass();  //创建一个矩形（多边形），其边界由范围定义
                sc.SetRectangle(env);
                return (IGeometry)sc;
            }
            else
            {
                // todo: throw an exception
                return null;
            }

        }

    }
}
