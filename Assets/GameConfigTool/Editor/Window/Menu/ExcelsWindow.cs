// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using NPOI.SS.UserModel;
using System;
using UnityEditor.IMGUI.Controls;

namespace GCT.Window
{
    internal sealed class ExcelsWindow : MenuParent
    {
        // width and height are between [0,1] and represent a percentage of the total screen area
        public ExcelsWindow(GCTWindow parentWindow, ExcelDrawContext[] contexts) : base(parentWindow, 0, 0, 265, 0, string.Empty, MenuAnchor.TOP_LEFT, MenuAutoSize.MATCH_VERTICAL)
        {
            SetMinimizedArea(-225, 0, 275, 0);

            GCTKeywords.Init();
            GCTCommonTypes.Init();
            var files = FileHelper.GetFiles(GCTSettings.Instance.ExcelPath, "*.xlsx");
            var excels = GCTExcelLoader.Load(files);
            GCTExcelLoader.GenerateData(excels.Values);
            ExcelsDrawContext = new SortedDictionary<string, ExcelDrawContext>();
            foreach (var excel in excels)
            {
                ExcelDrawContext context = null;
                if (contexts != null)
                {
                    foreach (var c in contexts)
                    {
                        if (c.name == excel.Key)
                        {
                            context = c;
                            context.excel = excel.Value;
                            context.ForceUpdate();
                        }
                    }
                }
                if (context == null)
                    context = new ExcelDrawContext(excel.Value);
                ExcelsDrawContext.Add(excel.Key, context);
            }
        }

        public new bool Draw(Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboardFocus)
        {
            bool changeCheck = false;
            base.Draw(parentPosition, mousePosition, mouseButtonId, hasKeyboardFocus);


            if (m_isMaximized)
            {
                KeyCode key = Event.current.keyCode;
                if (m_isMouseInside || hasKeyboardFocus)
                {
                    if (key == ShortcutsManager.ScrollUpKey)
                    {
                        m_currentScrollPos.y -= 10;
                        if (m_currentScrollPos.y < 0)
                        {
                            m_currentScrollPos.y = 0;
                        }
                        Event.current.Use();
                    }

                    if (key == ShortcutsManager.ScrollDownKey)
                    {
                        m_currentScrollPos.y += 10;
                        Event.current.Use();
                    }
                }

                if (m_forceUpdate)
                {

                }

                GUILayout.BeginArea(m_transformedArea, m_content, m_style);
                {
                    foreach (var excel in ExcelsDrawContext.Values)
                        DrawHelper(excel, excel.name, DrawExcelImpl);
                }
                GUILayout.EndArea();
            }

            PostDraw();
            return changeCheck;
        }


        public static bool DrawHelper(ExcelDrawContext context, string sectionName, DrawExcel DrawSection, DrawExcel HeaderSection)
        {
            bool repaint = false;
            Color cachedColor = GUI.color;
            GUI.color = new Color(cachedColor.r, cachedColor.g, cachedColor.b, 0.5f);
            EditorGUILayout.BeginHorizontal(UIUtils.MenuItemToolbarStyle);
            GUI.color = cachedColor;

            bool value = GUILayout.Toggle(context.foldout, sectionName, UIUtils.MenuItemToggleStyle);
            if (Event.current.button == Constants.FoldoutMouseId)
            {
                context.foldout = value;
            }
            HeaderSection(context);
            EditorGUILayout.EndHorizontal();

            if (context.foldout)
            {
                cachedColor = GUI.color;
                GUI.color = new Color(cachedColor.r, cachedColor.g, cachedColor.b, (EditorGUIUtility.isProSkin ? 0.5f : 0.25f));
                EditorGUILayout.BeginVertical(UIUtils.MenuItemBackgroundStyle);
                {
                    GUI.color = cachedColor;
                    EditorGUI.indentLevel++;
                    repaint = DrawSection(context);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndVertical();
            }
            return repaint;
        }

        private bool DrawHelper(ExcelDrawContext context, string sectionName, DrawExcel DrawSection)
        {
            Color cachedColor = GUI.color;
            GUI.color = new Color(cachedColor.r, cachedColor.g, cachedColor.b, 0.5f);
            EditorGUILayout.BeginHorizontal(UIUtils.MenuItemToolbarStyle);
            GUI.color = cachedColor;

            bool repaint = false;

            bool value = GUILayout.Toggle(context.foldout, sectionName, UIUtils.MenuItemToggleStyle);
            if (Event.current.button == Constants.FoldoutMouseId)
            {
                context.foldout = value;
            }
            EditorGUILayout.EndHorizontal();

            if (context.foldout)
            {
                cachedColor = GUI.color;
                GUI.color = new Color(cachedColor.r, cachedColor.g, cachedColor.b, (EditorGUIUtility.isProSkin ? 0.5f : 0.25f));
                EditorGUILayout.BeginVertical(UIUtils.MenuItemBackgroundStyle);
                {
                    GUI.color = cachedColor;
                    EditorGUI.indentLevel++;
                    repaint = DrawSection(context);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndVertical();
            }
            return repaint;
        }

        private bool DrawExcelImpl(ExcelDrawContext context)
        {
            context.Draw();
            return false;
        }

        public delegate bool DrawExcel(ExcelDrawContext context);

        private GUIStyle m_ExcelsStyle;
        private GUIStyle ExcelsStyle
        {
            get
            {
                m_ExcelsStyle = UIUtils.GetCustomStyle(CustomStyle.NodePropertiesTitle);
                m_ExcelsStyle.normal.textColor = m_ExcelsStyle.active.textColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
                return m_ExcelsStyle;
            }
        }
        private bool m_forceUpdate = false;
        private GUIContent m_dummyContent = new GUIContent();
        private static string m_SelectedExcelName;
        private static int m_SelectedIndex = -1;

        public static string SelectedExcelName { get { return m_SelectedExcelName; } set { m_SelectedExcelName = value; } }
        public static int SelectedIndex { get { return m_SelectedIndex; } set { m_SelectedIndex = value; } }

        public GCTRowTable CurrentRow
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedExcelName) == false)
                {
                    return ExcelsDrawContext[SelectedExcelName].rows[ExcelsDrawContext[SelectedExcelName].index];
                }
                return null;
            }
        }

        [Serializable]
        public class ExcelDrawContext
        {
            [SerializeField]
            public string name;
            [NonSerialized]
            public GCTExcel excel;
            [NonSerialized]
            public List<GCTRowTable> rows;
            [NonSerialized]
            public ReorderableList list;
            [SerializeField]
            public bool foldout = false;
            [SerializeField]
            public Vector2 scrollPos;
            [SerializeField]
            public int index;

            public ExcelDrawContext(GCTExcel excel)
            {
                this.excel = excel;
                this.name = excel.name;
                ForceUpdate();
            }

            public void Draw()
            {
                if (SelectedExcelName != name)
                    this.list.index = -1;
                this.list.DoLayoutList();
            }

            public void ForceUpdate()
            {
                rows = new List<GCTRowTable>();
                CollectRows(excel.Data.Data, rows);
                list = new ReorderableList(rows, typeof(GCTRowTable), false, false, false, false)
                {
                    headerHeight = 0,
                    footerHeight = 0,
                    showDefaultBackground = false,
                };
                list.drawElementCallback = DrawElementCallback;
                list.onSelectCallback = OnSelectCallback;
            }

            private void CollectRows(IDictionary<string, object> table, List<GCTRowTable> rows)
            {
                foreach (var value in table.Values)
                {
                    if (value is GCTRowTable)
                        rows.Add(value as GCTRowTable);
                    else if (value is IDictionary<string, object>)
                        CollectRows(value as IDictionary<string, object>, rows);
                }
            }
            
            private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
            {
                var row = rows[index];
                EditorGUI.LabelField(rect, row.ToString());
            }

            private void OnSelectCallback(ReorderableList list)
            {
                SelectedExcelName = name;
                SelectedIndex = list.index;
                this.index = list.index;
            }
        }
        public SortedDictionary<string, ExcelDrawContext> ExcelsDrawContext;
    }
}
