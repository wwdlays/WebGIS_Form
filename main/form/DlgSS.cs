using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace code.form
{
    public partial class DlgSS : Form
    {
        private ILegendClass pLegendClass;
        private ILayer pLayer;
        private IStyleGalleryItem pStyleGalleryItem;
        public ISymbol pSymbol;
        public Image pSymbolImage;
        public DlgSS(ILegendClass tempLegendClass, ILayer tempLayer)
        {
            InitializeComponent();
            this.pLegendClass = tempLegendClass;
            this.pLayer = tempLayer;
        }
        private void SetFeatureClassStyle(esriSymbologyStyleClass symbologyStyleClass)
        {
            this.axSymbologyControl1.StyleClass = symbologyStyleClass;
            ISymbologyStyleClass pSymbologyStyleClass = this.axSymbologyControl1.GetStyleClass(symbologyStyleClass);
            if (this.pLegendClass != null)
            {
                IStyleGalleryItem currentStyleGalleryItem = new ServerStyleGalleryItem();
                currentStyleGalleryItem.Name = "当前符号";
                currentStyleGalleryItem.Item = pLegendClass.Symbol;
                pSymbologyStyleClass.AddItem(currentStyleGalleryItem, 0);
                this.pStyleGalleryItem = currentStyleGalleryItem;
            }
            pSymbologyStyleClass.SelectItem(0);
        }

        private string ReadRegistry(string sKey)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey, true);
            if (rk == null) return "";
            return (string)rk.GetValue("InstallDir");
        }

        private void DlgSS_Load(object sender, EventArgs e)
        {
            //获得ArcGIS安装路径
            string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;
            //载入ESRI.ServerStyle文件到SymbologyControl
            // axSymbologyControl.LoadStyleFile("D:/Program Files(x86)/ArcGIS/Desktop10.8/Styles/ESRI.ServerStyle");
            //确定图层的类型
            axSymbologyControl1.LoadStyleFile(sInstall + "\\Styles\\ESRI.ServerStyle");
            IGeoFeatureLayer pGeoFeatureLayer = (IGeoFeatureLayer)pLayer;
            switch (((IFeatureLayer)pLayer).FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    SetFeatureClassStyle(esriSymbologyStyleClass.esriStyleClassMarkerSymbols);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    SetFeatureClassStyle(esriSymbologyStyleClass.esriStyleClassLineSymbols);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    SetFeatureClassStyle(esriSymbologyStyleClass.esriStyleClassFillSymbols);
                    break;
                case esriGeometryType.esriGeometryMultiPatch:
                    SetFeatureClassStyle(esriSymbologyStyleClass.esriStyleClassFillSymbols);
                    break;
            }
        }

        private void PreviewImage()
        {
            stdole.IPictureDisp picture = axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).PreviewItem(pStyleGalleryItem, pictureBox1.Width, pictureBox1.Height);
            System.Drawing.Image image = System.Drawing.Image.FromHbitmap(new System.IntPtr(picture.Handle));
            pictureBox1.Image = image;
        }

        public Bitmap Sym2Bitmap(ISymbol sym, int width, int height)
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
            IGeometry geom = MakeGeometry(sym, bounds);
            Graphics g = Graphics.FromImage(b);
            IntPtr hDC = g.GetHdc();
            sym.SetupDC(hDC.ToInt32(), dispTrans);
            sym.Draw(geom);
            sym.ResetDC();
            g.ReleaseHdc(hDC);
            return b;
        }
        private IGeometry MakeGeometry(ISymbol sym, IEnvelope env)
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

        private void button1_Click(object eventSender, EventArgs e)
        {
            pSymbol = (ISymbol)pStyleGalleryItem.Item;
            pSymbolImage = pictureBox1.Image;
            // 将选中的符号应用到图层
            IGeoFeatureLayer geoFeatureLayer = pLayer as IGeoFeatureLayer;
            if (geoFeatureLayer != null)
            {
                ISimpleRenderer simpleRenderer = new SimpleRendererClass();
                simpleRenderer.Symbol = pSymbol;
                geoFeatureLayer.Renderer = (IFeatureRenderer)simpleRenderer;
            }

            // 刷新地图以显示新的符号
            IActiveView activeView = pLayer as IActiveView;
            if (activeView != null)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }

            this.Close();
        }
        private void button2_Click(object eventSender, EventArgs e)
        {
            this.Close();
        }

        private void axSymbologyControl1_OnItemSelected(object sender, ISymbologyControlEvents_OnItemSelectedEvent e)
        {
            pStyleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;
            Color color;
            IRgbColor rgbColor;
            switch (axSymbologyControl1.StyleClass)
            {
                case esriSymbologyStyleClass.esriStyleClassMarkerSymbols:
                    rgbColor = ((IMarkerSymbol)pStyleGalleryItem.Item).Color as IRgbColor;
                    color = ColorTranslator.FromOle(rgbColor.RGB);
                    numericUpDown1.Value = (decimal)((IMarkerSymbol)pStyleGalleryItem.Item).Size;
                    button3.BackColor = color;
                    break;
                case esriSymbologyStyleClass.esriStyleClassLineSymbols:
                    rgbColor = ((ILineSymbol)pStyleGalleryItem.Item).Color as IRgbColor;
                    color = ColorTranslator.FromOle(rgbColor.RGB);
                    numericUpDown1.Value = (decimal)((ILineSymbol)pStyleGalleryItem.Item).Width;
                    button3.BackColor = color;
                    break;
                case esriSymbologyStyleClass.esriStyleClassFillSymbols:
                    rgbColor = ((IFillSymbol)pStyleGalleryItem.Item).Color as IRgbColor;
                    color = ColorTranslator.FromOle(rgbColor.RGB);
                    button3.BackColor = color;
                    break;
            }
            PreviewImage();
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (axSymbologyControl1.StyleClass == esriSymbologyStyleClass.esriStyleClassMarkerSymbols)
            {
                ((IMarkerSymbol)pStyleGalleryItem.Item).Size = (double)numericUpDown1.Value;
                PreviewImage();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor = colorDialog.Color;
                switch (axSymbologyControl1.StyleClass)
                {
                    case esriSymbologyStyleClass.esriStyleClassMarkerSymbols:
                        ((IMarkerSymbol)pStyleGalleryItem.Item).Color = ConvertColorToIColor(colorDialog.Color);
                        break;
                    case esriSymbologyStyleClass.esriStyleClassLineSymbols:
                        ((ILineSymbol)pStyleGalleryItem.Item).Color = ConvertColorToIColor(colorDialog.Color);
                        break;
                    case esriSymbologyStyleClass.esriStyleClassFillSymbols:
                        ((IFillSymbol)pStyleGalleryItem.Item).Color = ConvertColorToIColor(colorDialog.Color);
                        break;
                }
            }
            PreviewImage();
        }
        public IColor ConvertColorToIColor(Color color)
        {
            IColor pColor = new RgbColorClass();
            pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            return pColor;
        }
        public IStyleGalleryItem GetItem(esriSymbologyStyleClass styleClass, ISymbol symbol)
        {
            IStyleGalleryItem m_styleGalleryItem;
            m_styleGalleryItem = new ServerStyleGalleryItem();

            m_styleGalleryItem.Item = symbol;

            //获取并设置样式类
            axSymbologyControl1.StyleClass = styleClass;
            ISymbologyStyleClass symbologyStyleClass = axSymbologyControl1.GetStyleClass(styleClass);

            IMarkerSymbol pMarkSym;
            pMarkSym = (IMarkerSymbol)symbol;
            int dSize = (int)pMarkSym.Size;
            Bitmap b = Sym2Bitmap((ISymbol)pMarkSym, dSize, dSize);
            numericUpDown1.Value = dSize;
            button3.Image = (Image)b;
            this.ShowDialog();
            return m_styleGalleryItem;

        }
        public IStyleGalleryItem GetItem()
        {
            // Ensure that an item is selected
            if (axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).GetSelectedItem() != null)
            {
                return (IStyleGalleryItem)axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).GetSelectedItem();
            }
            else
            {
                MessageBox.Show("Please select a symbol before calling GetSelectedItem.", "No Symbol Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
        // 在DlgSS窗体中
        public Image CurrentImage
        {
            get
            {
                return pictureBox1.Image;
            }
        }

    }
}
