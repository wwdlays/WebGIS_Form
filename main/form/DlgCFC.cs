using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;

namespace code
{
    public partial class DlgCFC : Form
    {
        IMapControl3 mapControl = null;
        IWorkspace workspace = null;
        IFeatureClass featureClass = null;
        public DlgCFC(IMapControl3 mapControl3)
        {
            InitializeComponent();
            mapControl = mapControl3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //打开mdb文件
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mdb|*.mdb";//文件名筛选
            openFileDialog.CheckFileExists = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sFilePath = openFileDialog.FileName;
                label7.Text = sFilePath;
                IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
                workspace = workspaceFactory.OpenFromFile(sFilePath, 0);
            }
            else
            {
                string path = openFileDialog.FileName; ;
                MessageBox.Show(path + " 不是AccessGDB！");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //创建要素集
            ISpatialReference spatialReference = mapControl.SpatialReference;
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            string featureClassname = textBox1.Text;
            featureClass = CreateFeatureClassWithSR(featureClassname, featureWorkspace, spatialReference);
            MessageBox.Show("创建成功！", "提示", MessageBoxButtons.OK);
            IFeatureLayer featureLayer = new FeatureLayerClass();
            IDataset dataset = featureClass as IDataset;
            featureLayer.FeatureClass = featureWorkspace.OpenFeatureClass(dataset.Name);
            featureLayer.Name = featureLayer.FeatureClass.AliasName;
            mapControl.Map.AddLayer(featureLayer);
            mapControl.ActiveView.Refresh();
        }
        public IFeatureClass CreateFeatureClassWithSR(string featureClassName, IFeatureWorkspace featureWorkspace, ISpatialReference spatialReference)
        {
            // 实例化 FeatureClassDescription 以获取字段. 
            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
            IFields fields = ocDescription.RequiredFields;
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            // 添加 Name 字段. 
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = "Name";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.AddField(field);

            // 找到 Shape 字段，获取 GeometryDef 以设置空间体系
            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
            field = fields.Field[shapeFieldIndex]; //或 get_Field(idx)
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            if (radioButton1.Checked == true)
            {
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            }
            else if (radioButton2.Checked == true)
            {
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryLine;
            }
            else if (radioButton3.Checked == true)
            {
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            }
            geometryDefEdit.SpatialReference_2 = spatialReference;

            // In this example, only the required fields from the class description are used as fields for the feature class. If additional fields are added, use IFieldChecker to validate them. Use IFieldChecker to create a validated fields collection. 
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // 在工作区中创建要素类
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, validatedFields, ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, fcDescription.ShapeFieldName, "");
            return featureClass;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = "字段名称";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            string length = textBox3.Text;
            fieldEdit.Length_2 = int.Parse(length);//将string类型强制转换为int型
            dataGridView1.Rows.Add(textBox2.Text, comboBox1.SelectedItem.ToString(), textBox3.Text);
        }

        private void DlgCFC_Load(object sender, EventArgs e)
        {
            //设置表格基本数据
            dataGridView1.ColumnCount = 3;
            dataGridView1.RowHeadersWidth = 30;//设置包含行标题的列宽
            dataGridView1.TopLeftHeaderCell.Value = "序号";
            dataGridView1.Columns[0].HeaderText = "字段名称";
            dataGridView1.Columns[1].HeaderText = "字段类型";
            dataGridView1.Columns[2].HeaderText = "字段长度";

            //要素类型
            IPoint point = new PointClass();
            IGeometry geometry = point;
            IMultipoint multipoint = new MultipointClass();
            IGeometry geometry3 = multipoint;
            ILine line = new LineClass();
            IGeometry geometry1 = line;
            IPolyline polyline = new PolylineClass();
            IGeometry geometry2 = polyline;
            IPolygon polygon = new PolygonClass();
            IGeometry geometry4 = polygon;
            IRing ring = new RingClass();
            IGeometry geometry5 = ring;

            //字段类型
            comboBox1.Items.Add(geometry.GeometryType.ToString());
            comboBox1.Items.Add(geometry1.GeometryType.ToString());
            comboBox1.Items.Add(geometry2.GeometryType.ToString());
            comboBox1.Items.Add(geometry3.GeometryType.ToString());
            comboBox1.Items.Add(geometry4.GeometryType.ToString());
            comboBox1.Items.Add(geometry5.GeometryType.ToString());
        }
    }
}
