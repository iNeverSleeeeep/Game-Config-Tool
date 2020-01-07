using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GCT.Window
{
    internal class GCTWindow : SearchableEditorWindow, ISerializationCallbackReceiver
    {

        // Unity Menu item
        [MenuItem( "配置档工具/打开配置档编辑器", false, 1000 )]
		static void OpenMainShaderGraph()
		{
            GCTWindow currentWindow = OpenWindow("配置档编辑器");
            UIUtils.CurrentWindow = currentWindow;
            UIUtils.CurrentWindow.Show();
        }

        public static GCTWindow OpenWindow(string title, Texture icon = null)
        {
            GCTWindow currentWindow = (GCTWindow)GCTWindow.GetWindow(typeof(GCTWindow), false);
            currentWindow.minSize = new Vector2((Constants.MINIMIZE_WINDOW_LOCK_SIZE - 150), 270);
            currentWindow.wantsMouseMove = true;
            currentWindow.titleContent.text = title;
            if (icon != null)
                currentWindow.titleContent.image = icon;
            return currentWindow;
        }

        public GCTWindow()
        {
            m_drawInfo = new DrawInfo();
        }

        public override void OnEnable()
        {
            m_registeredMenus = new List<MenuParent>();

            m_ExcelsWindow = new ExcelsWindow(this, Contexts)
            {
                Resizable = true
            };
            ExcelsWindow.SelectedExcelName = SelectedExcelName;
            ExcelsWindow.SelectedIndex = SelectedIndex;
            m_registeredMenus.Add(m_ExcelsWindow);

            m_RowWindow = new RowWindow(this)
            {
                Resizable = true
            };
            m_registeredMenus.Add(m_RowWindow);
        }


        void OnGUI()
        {
            UIUtils.CurrentWindow = this;

            if ((object)UIUtils.MainSkin == null || !UIUtils.Initialized)
            {
                UIUtils.InitMainSkin();
            }

            m_currentEvent = Event.current;
            if (m_currentEvent.type == EventType.ExecuteCommand || m_currentEvent.type == EventType.ValidateCommand)
                m_currentCommandName = m_currentEvent.commandName;
            else
                m_currentCommandName = string.Empty;

            m_cameraInfo = position;
            
            if (m_currentEvent.type == EventType.Repaint)
                m_keyEvtMousePos2D = m_currentEvent.mousePosition;

            m_currentMousePos2D = m_currentEvent.mousePosition;
            m_currentMousePos.x = m_currentMousePos2D.x;
            m_currentMousePos.y = m_currentMousePos2D.y;

            m_repaintIsDirty |= m_ExcelsWindow.Draw(m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false);
            if (m_ExcelsWindow.IsResizing)
            {
                m_repaintIsDirty = true;
            }

            m_RowWindow.Draw(m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false, m_ExcelsWindow.CurrentRow);
            if (m_RowWindow.IsResizing)
            {
                m_repaintIsDirty = true;
            }


            if (m_repaintIsDirty)
            {
                m_repaintIsDirty = false;
                Repaint();
            }
        }

        void OnDestroy()
        {
            Destroy();
        }

        void Destroy()
        {
            m_registeredMenus.Clear();
            m_registeredMenus = null;

            m_ExcelsWindow.Destroy();
            m_ExcelsWindow = null;

            UIUtils.Initialized = false;
        }

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
            var contexts = new List<ExcelsWindow.ExcelDrawContext>();
            if (m_ExcelsWindow.ExcelsDrawContext != null)
                contexts.AddRange(m_ExcelsWindow.ExcelsDrawContext.Values);
            Contexts = contexts.ToArray();
            SelectedExcelName = ExcelsWindow.SelectedExcelName;
            SelectedIndex = ExcelsWindow.SelectedIndex;
        }

        private Event m_currentEvent;
        private Vector3 m_currentMousePos;
        private Vector2 m_keyEvtMousePos2D;
        private Vector2 m_currentMousePos2D;
        private string m_currentCommandName = string.Empty;
        private DrawInfo m_drawInfo;

        private Rect m_cameraInfo;

        [SerializeField]
        private bool m_repaintIsDirty;

        private List<MenuParent> m_registeredMenus;
        private ExcelsWindow m_ExcelsWindow;
        private RowWindow m_RowWindow;

        [SerializeField]
        private ExcelsWindow.ExcelDrawContext[] Contexts;
        [SerializeField]
        private string SelectedExcelName;
        [SerializeField]
        private int SelectedIndex;

        public Event CurrentEvent { get { return m_currentEvent; } }
        public GCTExcel Excel { get; private set; }
        public DrawInfo CameraDrawInfo { get { return m_drawInfo; } }
    }
}
