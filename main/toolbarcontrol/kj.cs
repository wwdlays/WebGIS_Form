using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace code
{
    /// <summary>
    /// Summary description for kj.
    /// </summary>
    [Guid("5ce470fa-1858-46f6-b776-0ef14231c288")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("code.kj")]
    public sealed class kj : BaseTool
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
        private IGlobeHookHelper m_globeHookHelper = null;
        private ISceneHookHelper m_sceneHookHelper = null;
        IMapControl3 mapControl;
        bool isUsing = false;

        public kj()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";   //localizable text 
            base.m_toolTip = "空间查询";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = @"D:\YNU\Subject\senior\gis软件开发\program\ex6\main\img\kj.bmp";
                base.m_bitmap = new Bitmap(bitmapResourceName);
                string cursorFilePath = @"D:\YNU\Subject\senior\gis软件开发\program\ex6\main\cur\kj.cur";
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
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add myIdentifyTool.OnCreate implementation
            if (m_hookHelper.Hook is IToolbarControl)
            {
                mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;

            }            //如果container是ToolbarControl从钩子获得MapControl
            //如果container是MapControl从钩子获得
            else if (m_hookHelper.Hook is IMapControl3)
            {
                mapControl = (IMapControl3)m_hookHelper.Hook;
            }
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add myIdentifyTool.OnClick implementation
            // 判断当前工具是否正在使用
            if (this.isUsing == false)
            { this.mapControl.CurrentTool = this; this.isUsing = true; }
            else
            {
                this.mapControl.CurrentTool = null; this.isUsing = false;
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add myIdentifyTool.OnMouseDown implementation

            if (this.isUsing == false) return;
            //生成点和缓冲区
            IPoint pMouseDownPoint;
            pMouseDownPoint = new PointClass();
            //pMouseDownPoint = Activator.CreateInstance(Type.GetTypeFromProgID("esriGeometry.Point")) as IPoint;
            pMouseDownPoint = mapControl.ToMapPoint(X, Y);
            pMouseDownPoint.SpatialReference = mapControl.Map.SpatialReference;
            IGeometry bufGeo;
            ITopologicalOperator bufferPoint = pMouseDownPoint as ITopologicalOperator;
            bufGeo = bufferPoint.Buffer(mapControl.ActiveView.Extent.Width / 250);


            Form4 form4 = new Form4(mapControl, bufGeo);
            form4.Show();
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            if (m_hookHelper != null)
            {
                //TODO: Add Map/PageLayout related logic
            }
            else if (m_sceneHookHelper != null)
            {
                //TODO: Add Scene related logic
            }
            else if (m_globeHookHelper != null)
            {
                //TODO: Add Globe related logic
            }
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (m_hookHelper != null)
            {
                //TODO: Add Map/PageLayout related logic
            }
            else if (m_sceneHookHelper != null)
            {
                //TODO: Add Scene related logic
            }
            else if (m_globeHookHelper != null)
            {
                //TODO: Add Globe related logic
            }
        }
        #endregion
    }
}
