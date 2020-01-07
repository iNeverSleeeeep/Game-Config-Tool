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
    internal sealed class RowWindow : MenuParent
    {
		// width and height are between [0,1] and represent a percentage of the total screen area
		public RowWindow( GCTWindow parentWindow) : base( parentWindow, 0, 0, 265, 0, string.Empty, MenuAnchor.TOP_RIGHT, MenuAutoSize.MATCH_VERTICAL )
		{
			SetMinimizedArea( -225, 0, 275, 0 );
        }

        public bool Draw(Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboardFocus, GCTRowTable row)
        {
            bool changeCheck = false;
            base.Draw(parentPosition, mousePosition, mouseButtonId, hasKeyboardFocus);

            if (row != null)
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

                GUILayout.BeginArea(m_transformedArea, m_content, m_style);
                {
                    m_currentScrollPos = EditorGUILayout.BeginScrollView(m_currentScrollPos, GUILayout.Width(0), GUILayout.Height(0));
                    float labelWidth = EditorGUIUtility.labelWidth;

                    EditorGUIUtility.labelWidth = 10;
                    var index = 0;
                    foreach (var field in row.Excel.Schema.Fields)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(field.Key, GUILayout.Width(100));
                        field.Value.Draw(row, index++);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndArea();
            }

            PostDraw();
            return changeCheck;
        }
    }
}
