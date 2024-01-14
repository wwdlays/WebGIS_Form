using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace code
{
    public partial class DlgCGDB : Form
    {
        IMapControl3 mapControl;
        
        public DlgCGDB(IMapControl3 mapControl3)
        {
            InitializeComponent();
            mapControl = mapControl3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            folder.ShowNewFolderButton = true;
            label4.Text = folder.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;

            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("数据库名称未命名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //根据不同方法创建不同类型数据库
                try
                {
                    if (radioButton1.Checked)
                    {
                        CreateAccessWorkspace();
                    }
                    if (radioButton2.Checked)
                    {
                        CreateFileGdbWorkspace();
                    }
                    if (radioButton3.Checked)
                    {
                        CreateShapefileWorkspace();
                    }
                    MessageBox.Show("数据库创建成功！", "提示", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
                
        }
        //IWorkspaceFactory workspaceFactory;
        public IWorkspace CreateAccessWorkspace()
        {
            //创建个人地理数据库
            // 创建 AccessWorkspaceFactory 工作区工厂实例
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspaceName workspaceName = workspaceFactory.Create(label4.Text, textBox1.Text, null, 0);
            // 通过接口转换到 IName 接口，调用 Open 方法得到工作区对象实例。
            IName Name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)Name.Open();
            return workspace;
        }

        public IWorkspace CreateFileGdbWorkspace()
        {
            //创建基于文件的地理数据库
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
            IWorkspaceName workspaceName = workspaceFactory.Create(label4.Text, textBox1.Text, null, 0);
            IName Name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)Name.Open();
            return workspace;
        }

        public IWorkspace CreateShapefileWorkspace()
        {
            //创建shapefile地理数据库
            ShapefileWorkspaceFactory shapefileWorkspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspaceFactory2 workspaceFactory = (IWorkspaceFactory2)shapefileWorkspaceFactory;
            IWorkspaceName workspaceName = workspaceFactory.Create(label4.Text, textBox1.Text, null, 0);
            IName Name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)Name.Open();
            return workspace;
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // 创建一个对话框让用户选择文件夹
        //        FolderBrowserDialog folderDialog = new FolderBrowserDialog();
        //        if (folderDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            string path = folderDialog.SelectedPath;

        //            // 检查用户是否实际选择了一个文件夹
        //            if (Directory.Exists(path))
        //            {
        //                // 弹出一个确认对话框
        //                if (MessageBox.Show("您确定要删除这个文件夹及其所有内容吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //                {
        //                    Directory.Delete(path, true);
        //                    MessageBox.Show("文件夹已删除！");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建一个对话框让用户选择文件
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = fileDialog.FileName;

                    // 检查用户是否实际选择了一个文件
                    if (File.Exists(path))
                    {
                        // 弹出一个确认对话框
                        if (MessageBox.Show("确定要删除工作区吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            File.Delete(path);
                            MessageBox.Show("工作区已删除！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
