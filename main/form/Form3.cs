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
    public partial class Form3 : Form
    {
        IMapControl3 mapControl; // 声明一个IMapControl3类型的变量mapControl

        public Form3(IMapControl3 mapcontrol)
        {
            InitializeComponent();
            mapControl = mapcontrol; // 将传入的参数mapcontrol赋值给变量mapControl

            if (mapControl.LayerCount > 0)
            {
                // 默认显示第一个图层
                ILayer layer = mapControl.get_Layer(0);
                comboBox1.Text = layer.Name;

                // 遍历图层显示到复合框列表
                for (int i = 0; i < mapControl.LayerCount; i++)
                {
                    ILayer layer1 = mapControl.get_Layer(i);
                    comboBox1.Items.Add(layer1.Name);
                }
                comboBox1.SelectedIndex = 0;

                // 获取选中的图层的要素图层（FeatureLayer）
                IFeatureLayer featureLayer = mapControl.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
                IFeatureClass featureClass = featureLayer.FeatureClass; // 获取要素类（FeatureClass）
                IFields fields = featureClass.Fields; // 获取字段集合

                // 将字段名称添加到comboBox2复合框，并选择第一个
                for (int j = 0; j < fields.FieldCount; j++)
                {
                    if (fields.Field[j].Name != featureClass.ShapeFieldName)
                    {
                        comboBox2.Items.Add(fields.Field[j].Name);
                    }
                }
                comboBox2.SelectedIndex = 0;

                string filedname = comboBox2.SelectedItem.ToString(); // 获取选择的字段名称
                IField field = featureClass.Fields.Field[featureClass.FindField(filedname)]; // 获取字段对象
                ITable table = featureClass as ITable;
                ICursor cursor = table.Search(null, false); // 查询功能
                IDataStatistics statistics = new DataStatisticsClass(); // 数据统计功能
                statistics.Cursor = cursor; // 设置数据统计功能的游标
                statistics.Field = field.Name; // 设置数据统计功能的字段名称

                System.Collections.IEnumerator enumerator = statistics.UniqueValues; // 获取唯一值的枚举器
                while (enumerator.MoveNext())
                {
                    string value = enumerator.Current.ToString(); // 获取当前唯一值
                    comboBox4.Items.Add(value); // 将唯一值添加到comboBox4复合框
                    comboBox4.SelectedIndex = 0;
                }
            }
            else
            {
                // 处理没有图层的情况，例如显示错误消息
                MessageBox.Show("没有可用的图层。");
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获得FeatureLayer图层
            IFeatureLayer FeatureLayer = mapControl.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
            //清除上次查询结果
            mapControl.Map.ClearSelection();
            IActiveView pActiveView = mapControl.Map as IActiveView;
            //pQueryFilter的实例化 
            IQueryFilter queryFilter = new QueryFilterClass();
            //设置查询过滤条件 
            queryFilter.WhereClause = comboBox2.Text + comboBox3.Text + comboBox4.Text;
            MessageBox.Show("Show:" + comboBox2.Text + comboBox3.Text + comboBox4.Text);
            //调用 Search 方法得到featureCursor
            IFeatureCursor featureCursor = FeatureLayer.Search(queryFilter, false);
            //获取遍历到的要素 
            IFeature feature = featureCursor.NextFeature();

            while (feature != null)
            {
                mapControl.Map.SelectFeature(FeatureLayer, feature); //选择要素 
                feature = featureCursor.NextFeature();
            }
            //刷新图层
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            pActiveView.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //获得当前图层
            IFeatureLayer featureLayer = mapControl.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
            //获取图层要素
            IFeatureClass featureClass = featureLayer.FeatureClass;
            if (featureClass == null)
            {
                return;
            }
            IGeoDataset geoDataset = (IGeoDataset)featureClass;
            IGeometry geometryBag = Activator.CreateInstance(Type.GetTypeFromProgID("esriGeometry.GeometryBag")) as IGeometry;
            //添加元素到包前指定坐标系.
            geometryBag.SpatialReference = geoDataset.SpatialReference;
            IQueryFilter queryFilter = new QueryFilterClass();
            //设置过滤器对象的属性
            queryFilter.WhereClause = comboBox2.Text + comboBox3.Text + comboBox4.Text;
            //遍历要素类中所有要素
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            //接口转换到 IGeometryCollection
            IGeometryCollection geometryCollection = (IGeometryCollection)geometryBag;
            IFeature currentFeature = featureCursor.NextFeature();
            while (currentFeature != null)
            {
                //在几何图形集末尾添加要素的几何图形,AddGeometry 的后两个参数为 Type.missing(之前和之后的 几何对象)
                //so the currentFeature.Shape IGeometry is added to the end of the geometryCollection.
                object missing = Type.Missing;
                geometryCollection.AddGeometry(currentFeature.Shape, missing, missing);
                currentFeature = featureCursor.NextFeature();
            }
            //将接口转换为IGeometry
            geometryBag = (IGeometry)geometryCollection;
            //获取 Envelope 属性，根据此属性设置地图控件的范围(Extent属性)
            mapControl.ActiveView.Extent = geometryBag.Envelope;
            mapControl.Refresh();

        }

    }
}
