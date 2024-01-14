using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace code.toolbarcontrol
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout, ArcScene/SceneControl
    /// or ArcGlobe/GlobeControl
    /// </summary>
    [Guid("edf5dc91-df31-4ad5-8970-226af62591a2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("code.toolbarcontrol.RemoveLayers")]
    public sealed class RemoveLayers : BaseCommand
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
        private IMapControl3 m_mapControl;
        private IHookHelper m_hookHelper;

        public RemoveLayers()
        {
            base.m_category = ""; //localizable text
            base.m_caption = "É¾³ýÍ¼²ã";  //localizable text
            base.m_message = "";   //localizable text 
            base.m_toolTip = "";  //localizable text 
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")
        }

        #region Overridden Class Methods

        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        public override void OnClick()
        {
            //»ñÈ¡Ñ¡¶¨µÄÍ¼²ã
            ILayer layer = (ILayer)m_mapControl.CustomProperty;
            //ÒÆ³ýÍ¼²ã
            if (layer != null)
            {
                m_mapControl.Map.DeleteLayer(layer);
                m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        #endregion
    }
}
