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
    /// Summary description for ZoomIn.
    /// </summary>
    [Guid("3f5e8f54-92bf-4003-b273-d9b233a77f8d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("code.ZoomIn")]
    public sealed class ZoomIn : BaseTool
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
        private IPoint m_point;
        private Boolean m_isMouseDown;
        private INewEnvelopeFeedback m_feedBack;
        public ZoomIn()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";   //localizable text 
            base.m_toolTip = "放大图层";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            m_hookHelper = new HookHelperClass();
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = @"D:\YNU\Subject\senior\gis软件开发\program\ex6\main\img\ZoomIn.bmp";
                base.m_bitmap = new Bitmap(bitmapResourceName);
                string cursorFilePath = @"D:\YNU\Subject\senior\gis软件开发\program\ex6\main\cur\ZoomIn.cur";
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
            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        //public override void OnClick()
        //{
        //    if(m_hookHelper != null)
        //    {
        //        return;
        //    }
        //}

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (m_hookHelper.ActiveView == null)
            {
                return;
            }
            if (m_hookHelper.ActiveView is IPageLayout)
            {
                IPoint pPoint = (IPoint)(m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y));
                IMap pMap = m_hookHelper.ActiveView.HitTestMap(pPoint);
                if(pMap == null)
                {
                    return;
                }
                if(pMap != m_hookHelper.FocusMap)
                {
                    m_hookHelper.ActiveView.FocusMap = pMap;
                    m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics,null,null);
                }
            }
            IActiveView pActiveView = (IActiveView)m_hookHelper.FocusMap;
            m_point = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            m_isMouseDown = true;
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            if (m_hookHelper != null)
            {
                if (!m_isMouseDown)
                {
                    return;
                }
                IActiveView pActiveView = (IActiveView)m_hookHelper.FocusMap;
                if(m_feedBack == null)
                {
                    m_feedBack = new NewEnvelopeFeedbackClass();
                    m_feedBack.Display = pActiveView.ScreenDisplay;
                    m_feedBack.Start(m_point);
                }
                m_feedBack.MoveTo(pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y));
            }
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (m_hookHelper != null)
            {
                if (!m_isMouseDown)
                {
                    return;
                }
                IActiveView pActiveView = (IActiveView)m_hookHelper.FocusMap;
                IEnvelope pEnvelope = default(IEnvelope);
                if(m_feedBack == null)
                {
                    pEnvelope = pActiveView.Extent;
                    pEnvelope.Expand(0.5,0.5,true);
                    pEnvelope.CenterAt(m_point);
                }
                else
                {
                    pEnvelope = m_feedBack.Stop();
                    if(pEnvelope.Width == 0||pEnvelope.Height == 0)
                    {
                        m_feedBack = null;
                        m_isMouseDown = false;
                    }
                }
                pActiveView.Extent = pEnvelope;
                pActiveView.Refresh();
                m_feedBack = null;
                m_isMouseDown = false;
            }
        }
        #endregion
    }
}
