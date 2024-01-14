using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace code.toolbarcontrol
{
    /// <summary>
    /// Summary description for ToolAD.
    /// </summary>
    [Guid("ce65059c-81c8-4b81-b253-bf83c53cac13")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("code.toolbarcontrol.ToolAD")]
    public sealed class ToolAD : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Register(regKey);
            MxCommands.Register(regKey);
            SxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Unregister(regKey);
            MxCommands.Unregister(regKey);
            SxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        AxMapControl _ax;
        public ToolAD(IWorkspaceEdit edit, IFeatureLayer layer, AxMapControl ax)
        {
            //
            // TODO: Define values for the public properties
            //
            _ax = ax;
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";   //localizable text 
            base.m_toolTip = "添加要素";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = @"..\\..\\img\\CF.bmp";
                base.m_bitmap = new Bitmap(bitmapResourceName);
                string cursorFilePath = @"D:\YNU\Subject\senior\gis软件开发\program\ex6\main\cur\ToolAD.cur";
                base.m_cursor = new System.Windows.Forms.Cursor(cursorFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            // Test the hook that calls this command and disable if nothing valid
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {

        }
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            var editingLayer = EditEnvSingleton.EditingLayer;
            // TODO:  Add EditTool.OnMouseDown implementation
            EditEnvSingleton.EditSpan.StartEditOperation();
            EditEnvSingleton.EditSpan.StartEditing(true);
            var feature = editingLayer.FeatureClass.CreateFeature();
            int indexOfGeometry = editingLayer.FeatureClass.Fields.FindField("geometry");
            int indexOfName = editingLayer.FeatureClass.Fields.FindField("Name");
            feature.set_Value(indexOfName, "testFeature");
            esriGeometryType type = editingLayer.FeatureClass.ShapeType;
            if (type == esriGeometryType.esriGeometryPoint)
            {
                IPoint newPt = _ax.ToMapPoint(X, Y);
                feature.Shape = newPt;
                feature.Store();
            }
            else if (type == esriGeometryType.esriGeometryPolyline)
            {
                IPolyline line = (IPolyline)_ax.TrackLine();
                feature.Shape = line;
                feature.Store();
            }

            else if (type == esriGeometryType.esriGeometryPolygon)
            {
                IPolygon line = (IPolygon)_ax.TrackPolygon();
                feature.Shape = line;
                feature.Store();
            }
            _ax.ActiveView.Refresh();

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {

        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {

        }
        #endregion
    }
}
