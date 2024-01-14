using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
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
    public partial class DlgES : Form
    {
        public IFeatureLayer Layer { get; set; }
        public AxMapControl _ax;
        public IWorkspaceEdit EditSpan;
        public IFeatureLayer EditLayer;
        public DlgES(AxMapControl ax)
        {
            InitializeComponent();
            _ax = ax;
            List<IFeatureLayer> layers = new List<IFeatureLayer>();
            for (int i = 0; i < ax.LayerCount; i++)
            {
                listBox1.Items.Add(ax.get_Layer(i).Name);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Layer = (IFeatureLayer)_ax.get_Layer(listBox1.SelectedIndex);
            EditEnvSingleton.EditingLayer = Layer;
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

    }
}
