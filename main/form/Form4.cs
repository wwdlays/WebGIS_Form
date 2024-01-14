using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
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

namespace code
{
    public partial class Form4 : Form
    {
        IMapControl3 mapControl;
        IGeometry geometry;
        public Form4(IMapControl3 mapcontrol, IGeometry bufGeo)
        {
            InitializeComponent();
            mapControl = mapcontrol;
            geometry = bufGeo;
        }

        private void ShowFeatureInfo_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 2;
            dataGridView1.RowHeadersWidth = 60;
            // 设置DataGridView左上角单元格的值为"序号"
            dataGridView1.TopLeftHeaderCell.Value = "序号";
            // 设置列标题
            dataGridView1.Columns[0].HeaderText = "字段";
            dataGridView1.Columns[1].HeaderText = "属性值";
            comboBox1.Items.Clear();
            // 遍历地图控件中的图层
            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                // 获取当前索引处的图层对象
                ILayer layer1 = mapControl.get_Layer(i);
                // 将图层名称添加到ComboBox中
                comboBox1.Items.Add(layer1.Name);
            }
            // 设置ComboBox的选择索引为0
            comboBox1.SelectedIndex = 0;
        }

        //根据 feature (IFeature)在 DataGridView 控件中显示要素属性
        public void ShowAttribute(IFeature feature)
        {
            IFeatureLayer pFeatureLayer = mapControl.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer; ;
            IFeatureCursor featureCursor = pFeatureLayer.Search(null, false);
            feature = featureCursor.NextFeature();
            int num = feature.Fields.FieldCount;
            dataGridView1.RowCount = num;
            int i = 0;
            for (i = 0; i < num; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = i.ToString();
                dataGridView1[0, i].Value = feature.Fields.get_Field(i).Name.ToString();
                if (feature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    string type = feature.Shape.GeometryType.ToString();
                    switch (type)
                    {
                        case "esriGeometryPoint":
                            dataGridView1[1, i].Value = "点";
                            break;
                        case "esriGeometryPolyline":
                            dataGridView1[1, i].Value = "线";
                            break;
                        case "esriGeometryPolygon":
                            dataGridView1[1, i].Value = "面";
                            break;
                    }
                }
                else
                {
                    dataGridView1[1, i].Value = feature.Value[i].ToString();
                }
            }
            this.toolStripStatusLabel1.Text = "查询的要素共有" + feature.Fields.FieldCount.ToString() + "个字段";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //定义空间条件查询过滤器，获取 FeatureCursor，通过 FeatureCursor 获取要素
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            IFeatureLayer pFeatureLayer = mapControl.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
            spatialFilter.Geometry = geometry;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor featureCursor = pFeatureLayer.Search(spatialFilter, false);
            IFeature pFeature = featureCursor.NextFeature();
            while (pFeature != null)
            {
                mapControl.FlashShape(pFeature.Shape);
                pFeature = featureCursor.NextFeature();
            }
            mapControl.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            ShowAttribute(pFeature);
        }
    }
}
