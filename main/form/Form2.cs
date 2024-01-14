using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using System.IO;
using System.Collections;

namespace code
{
    public partial class Form2 : Form
    {
        public string path;
        IWorkspace mgbWorkSpace;
        IHookHelper m_hookHelper;
        IRasterWorkspaceEx _rasterWorkspace;
        public Form2(IHookHelper hook)
        {
            InitializeComponent();
            m_hookHelper = hook;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 定义一个字符串数组，存储不同的文件类型
            string[] fileTypes = new string[] { "文件地理数据库 (*.mdb)|*.mdb", "要素类 (*.shp)|*.shp", "栅格数据 (*.tif;*.img;*.png;*.jpg)|*.tif;*.img;*.png;*.jpg" };
            // 使用string.Join方法，用"|"分隔符连接数组元素，作为Filter属性的值
            openFileDialog.Filter = string.Join("|", fileTypes) + "|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            string path = openFileDialog.FileName; // 声明path变量的类型，用FileName获取文件路径
            label1.Text = path;
            // 使用Path.GetExtension方法，获取文件的扩展名
            string extension = Path.GetExtension(path);
            // 使用switch语句，根据不同的扩展名执行不同的操作
            switch (extension)
            {
                case ".mdb":
                    listBox1.Items.Clear(); // 每次打开前清空items
                    listBox2.Items.Clear();
                    IWorkspaceFactory pIWorkspaceFactory = new AccessWorkspaceFactoryClass(); // 用AccessWorkspaceFactoryClass打开mdb文件
                    mgbWorkSpace = pIWorkspaceFactory.OpenFromFile(path, 0);
                    this.radioButton1.Checked = true;
                    break;
                case ".shp":
                    // 打开要素类文件的代码
                    break;
                case ".tif":
                case ".img":
                case ".png":
                    // 打开栅格数据文件的代码
                    break;
                default:
                    // 如果文件类型不匹配，就返回
                    return;
            }
        }


        private void GeoDatasetNames(IWorkspace workspace, esriDatasetType datasetType, ListBox listBox)
        {

            // 定义工作区下子集名称集合的指针
            IEnumDatasetName enumDatasetName = workspace.DatasetNames[esriDatasetType.esriDTFeatureDataset];
            IDatasetName datasetName = enumDatasetName.Next();

            while (datasetName != null)
            {
                listBox.Items.Add(datasetName.Name);
                datasetName = enumDatasetName.Next();
            }
            // listBox默认选择第一个数据集
            if (listBox.Items.Count > 0)
            { listBox.SelectedIndex = 0; }
        }
        private void getFeatClassNames(IFeatureDataset featDataset)
        {
            // 数据集下子集的指针
            IEnumDataset enumDataset = featDataset.Subsets;
            IDataset dataset = enumDataset.Next();
            while (dataset != null)
            {

                if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                {
                    this.listBox2.Items.Add(dataset.Name);
                }
                dataset = enumDataset.Next();
            }
        }
        private IFeatureDataset getFeatDataset(string sName, IWorkspace workspace)
        {
            IFeatureWorkspace feaWorkspace = workspace as IFeatureWorkspace;
            try
            {
                return feaWorkspace.OpenFeatureDataset(sName);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #region addRadter
        private IRasterDataset GetRasterUsingNameFromWorkspace(IRasterWorkspaceEx workspace, string datasetName)
        {
            return _rasterWorkspace.OpenRasterDataset(datasetName);
        }
        private void LoadRasterLayer(IMapControl3 map, IRasterDataset data)
        {
            IRasterLayer layer = new RasterLayerClass();
            layer.CreateFromDataset(data);
            map.AddLayer(layer);

        }
        private ArrayList GetRasterNames()
        {
            IEnumDatasetName enumDatasetName = ((IWorkspace)_rasterWorkspace).get_DatasetNames(esriDatasetType.esriDTRasterDataset);
            ArrayList list = new ArrayList();
            IDatasetName name = enumDatasetName.Next();
            while (name != null)
            {
                list.Add(name.Name);
                name = enumDatasetName.Next();
            }
            return list;

        }
        #endregion

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                listBox1.Visible = true;
                listBox2.Visible = true;
                if (listBox1.Items != null)
                {
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                }
                GeoDatasetNames(mgbWorkSpace, esriDatasetType.esriDTFeatureDataset, listBox1);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //显示独立要素类
            if (radioButton2.Checked == true)
            {
                listBox1.Visible = false;
                listBox2.Visible = true;

                // 工作区下要素类名称并添加到listBox2，隐藏listBox1
                listBox2.Items.Clear();
                GeoDatasetNames(mgbWorkSpace, esriDatasetType.esriDTFeatureClass, listBox2);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //栅格数据
            if (radioButton3.Checked == true)
            {
                listBox2.Visible = false;
                listBox1.Visible = true;

                listBox1.Items.Clear();
                //加载栅格数据
                button1.PerformClick();

            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 根据选择的listBox1的要素集中的要素类显示到listBox中
            string datasetName = this.listBox1.SelectedItem.ToString();
            listBox2.Items.Clear();
            IFeatureDataset featureDataset = getFeatDataset(datasetName, mgbWorkSpace);
            if (featureDataset == null) return;
            else
            {
                getFeatClassNames(featureDataset);
                listBox2.SelectedIndex = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IFeatureClass feaClass;
            IFeatureLayer feaLayer = new FeatureLayerClass();
            IRasterLayer rasLayer = new RasterLayerClass();
            //  要素数据集
            if (radioButton1.Checked == true)
            {
                string featureDatasetName = listBox1.SelectedItem.ToString();
                string featrueName = this.listBox2.SelectedItem.ToString();
                IFeatureDataset featureDataset = getFeatDataset(featureDatasetName, this.mgbWorkSpace);
                IFeatureWorkspace feaWorkspace = featureDataset.Workspace as IFeatureWorkspace;
                feaClass = feaWorkspace.OpenFeatureClass(featrueName);
                feaLayer.FeatureClass = feaClass;
                feaLayer.Name = feaClass.AliasName;
                m_hookHelper.FocusMap.AddLayer(feaLayer as ILayer);
                m_hookHelper.ActiveView.Refresh();
            } // 要素类
            else if (radioButton2.Checked == true)
            {
                return;
            }
            else if (radioButton3.Checked == true)
            {
                //加载栅格
                return;
            }
            else
            {
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
