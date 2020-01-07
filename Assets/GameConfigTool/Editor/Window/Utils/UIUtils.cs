// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using System.Globalization;
using System.Text.RegularExpressions;

namespace GCT.Window
{
    public enum CustomStyle
    {
        NodeWindowOff = 0,
        NodeWindowOn,
        NodeTitle,
        NodeHeader,
        CommentaryHeader,
        ShaderLibraryTitle,
        ShaderLibraryAddToList,
        ShaderLibraryRemoveFromList,
        ShaderLibraryOpenListed,
        ShaderLibrarySelectionAsTemplate,
        ShaderLibraryItem,
        CommentaryTitle,
        PortEmptyIcon,
        PortFullIcon,
        InputPortlabel,
        OutputPortLabel,
        CommentaryResizeButton,
        CommentaryResizeButtonInv,
        CommentaryBackground,
        MinimizeButton,
        MaximizeButton,
        NodePropertiesTitle,
        ShaderModeTitle,
        MaterialModeTitle,
        ShaderNoMaterialModeTitle,
        PropertyValuesTitle,
        ShaderModeNoShader,
        MainCanvasTitle,
        ShaderBorder,
        MaterialBorder,
        SamplerTextureRef,
        SamplerTextureIcon,
        CustomExpressionAddItem,
        CustomExpressionRemoveItem,
        CustomExpressionSmallAddItem,
        CustomExpressionSmallRemoveItem,
        ResetToDefaultInspectorButton,
        SliderStyle,
        ObjectPicker,
        NodePropertyPicker,
        NodePreviewExpander,
        NodePreviewCollapser,
        SamplerButton,
        SamplerFrame,
        CommentarySuperTitle,
        MiniButtonTopLeft,
        MiniButtonTopMid,
        MiniButtonTopRight,
        ShaderFunctionBorder,
        ShaderFunctionMode,
        RightShaderMode,
        FlatBackground,
        DocumentationLink,
        GraphButtonIcon,
        GraphButton,
        NodeWindowOffSquare,
        NodeHeaderSquare,
        NodeWindowOnSquare,
        SelectedLabel
    }

    public enum NodeConnectionStatus
    {
        Not_Connected = 0,
        Connected,
        Error,
        Island
    }

    public enum InteractionMode
    {
        Target,
        Other,
        Both
    }


    internal class UIUtils
    {
        public static int SerializeHelperCounter = 0;

        public static bool Initialized = false;
        public static float HeaderMaxHeight;
        public static GUISkin MainSkin = null;
        public static GUIStyle PlusStyle;
        public static GUIStyle MinusStyle;
        public static GUIStyle RangedFloatSliderStyle;
        public static GUIStyle RangedFloatSliderThumbStyle;
        public static GUIStyle SwitchNodePopUp;
        public static GUIStyle PropertyPopUp;
        public static GUIStyle ObjectField;
        public static GUIStyle PreviewExpander;
        public static GUIStyle PreviewCollapser;
        public static GUIStyle ObjectFieldThumb;
        public static GUIStyle ObjectFieldThumbOverlay;
        public static GUIStyle InspectorPopdropdownStyle;
        public static GUIStyle InspectorPopdropdownFallback;
        public static GUIStyle BoldErrorStyle;
        public static GUIStyle BoldWarningStyle;
        public static GUIStyle BoldInfoStyle;
        public static GUIStyle Separator;
        public static GUIStyle ToolbarSearchTextfield;
        public static GUIStyle ToolbarSearchCancelButton;
        public static GUIStyle MiniButtonTopLeft;
        public static GUIStyle MiniButtonTopMid;
        public static GUIStyle MiniButtonTopRight;

        public static GUIStyle CommentaryTitle;
        public static GUIStyle InputPortLabel;
        public static GUIStyle OutputPortLabel;

        public static GUIStyle MiniObjectFieldThumbOverlay;
        public static GUIStyle MiniSamplerButton;

        public static GUIStyle NodeWindowOffSquare;
        public static GUIStyle NodeHeaderSquare;
        public static GUIStyle NodeWindowOnSquare;
        public static GUIStyle InternalDataOnPort;
        public static GUIStyle InternalDataBackground;

        public static GUIStyle GraphButtonIcon;
        public static GUIStyle GraphButton;
        public static GUIStyle GraphDropDown;

        public static GUIStyle EmptyStyle = new GUIStyle();

        public static GUIStyle TooltipBox;
        public static GUIStyle Box;
        public static GUIStyle Button;
        public static GUIStyle TextArea;
        public static GUIStyle Label;
        public static GUIStyle SelectedLabel;
        public static GUIStyle Toggle;
        public static GUIStyle Textfield;

        public static GUIStyle UnZoomedNodeTitleStyle;
        public static GUIStyle UnZoomedPropertyValuesTitleStyle;
        public static GUIStyle UnZoomedInputPortStyle;
        public static GUIStyle UnZoomedOutputPortPortStyle;

        // Node Property Menu items
        public static GUIStyle MenuItemToggleStyle;
        public static GUIStyle MenuItemEnableStyle;
        public static GUIStyle MenuItemBackgroundStyle;
        public static GUIStyle MenuItemToolbarStyle;
        public static GUIStyle MenuItemInspectorDropdownStyle;

        public static GUIStyle FloatIntPickerONOFF;

        public static Vector2 PortsSize;
        public static Vector3 PortsDelta;
        public static Vector3 ScaledPortsDelta;

        public static RectOffset RectOffsetZero;
        public static RectOffset RectOffsetOne;
        public static RectOffset RectOffsetTwo;
        public static RectOffset RectOffsetThree;
        public static RectOffset RectOffsetFour;
        public static RectOffset RectOffsetFive;
        public static RectOffset RectOffsetSix;

        public static Texture2D SliderButton = null;

        private static TextInfo m_textInfo;

        private static GCTWindow m_currentWindow = null;
        public static GCTWindow CurrentWindow
        {
            get
            {
                if (m_currentWindow == null)
                {
                    foreach (var window in IOUtils.AllOpenedWindows.Values)
                    {
                        if (window != null)
                            m_currentWindow = window;
                    }
                }
                return m_currentWindow;
            }
            set { m_currentWindow = value; }
        }

        public static GUIStyle GetCustomStyle(CustomStyle style)
        {
            return (Initialized) ? MainSkin.customStyles[(int)style] : null;
        }

        public static void InitMainSkin()
        {
            MainSkin = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(IOUtils.MainSkinGUID), typeof(GUISkin)) as GUISkin;
            Initialized = true;
            HeaderMaxHeight = MainSkin.customStyles[(int)CustomStyle.NodeHeader].normal.background.height;

            RectOffsetZero = new RectOffset(0, 0, 0, 0);
            RectOffsetOne = new RectOffset(1, 1, 1, 1);
            RectOffsetTwo = new RectOffset(2, 2, 2, 2);
            RectOffsetThree = new RectOffset(3, 3, 3, 3);
            RectOffsetFour = new RectOffset(4, 4, 4, 4);
            RectOffsetFive = new RectOffset(5, 5, 5, 5);
            RectOffsetSix = new RectOffset(6, 6, 6, 6);

            PropertyPopUp = GetCustomStyle(CustomStyle.NodePropertyPicker);
            ObjectField = new GUIStyle((GUIStyle)"ObjectField");
            PreviewExpander = GetCustomStyle(CustomStyle.NodePreviewExpander);
            PreviewCollapser = GetCustomStyle(CustomStyle.NodePreviewCollapser);


            CommentaryTitle = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.CommentaryTitle]);
            InputPortLabel = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.InputPortlabel]);
            OutputPortLabel = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.OutputPortLabel]);

            BoldErrorStyle = new GUIStyle((GUIStyle)"BoldLabel");
            BoldErrorStyle.normal.textColor = Color.red;
            BoldErrorStyle.alignment = TextAnchor.MiddleCenter;
            BoldWarningStyle = new GUIStyle((GUIStyle)"BoldLabel");
            BoldWarningStyle.normal.textColor = Color.yellow;
            BoldWarningStyle.alignment = TextAnchor.MiddleCenter;
            BoldInfoStyle = new GUIStyle((GUIStyle)"BoldLabel");
            BoldInfoStyle.normal.textColor = Color.white;
            BoldInfoStyle.alignment = TextAnchor.MiddleCenter;

            Separator = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.FlatBackground]);
            MiniButtonTopLeft = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.MiniButtonTopLeft]);
            MiniButtonTopMid = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.MiniButtonTopMid]);
            MiniButtonTopRight = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.MiniButtonTopRight]);

            InternalDataOnPort = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.NodeTitle]);
            InternalDataOnPort.fontSize = 8;
            InternalDataOnPort.fontStyle = FontStyle.BoldAndItalic;
            InternalDataBackground = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.NodeWindowOffSquare]);
            InternalDataBackground.normal.background = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("330fd0c8f074a3c4f8042114a61a73d9"), typeof(Texture2D)) as Texture2D;
            InternalDataBackground.overflow = RectOffsetOne;

            MiniObjectFieldThumbOverlay = new GUIStyle((GUIStyle)"ObjectFieldThumbOverlay");
            MiniSamplerButton = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.SamplerButton]);

            m_textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            RangedFloatSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            RangedFloatSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            RangedFloatSliderThumbStyle.normal.background = SliderButton;
            RangedFloatSliderThumbStyle.active.background = null;
            RangedFloatSliderThumbStyle.hover.background = null;
            RangedFloatSliderThumbStyle.focused.background = null;

            SwitchNodePopUp = new GUIStyle((GUIStyle)"Popup");

            GraphButtonIcon = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.GraphButtonIcon]);
            GraphButton = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.GraphButton]);
            GraphDropDown = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.GraphButton]);
            GraphDropDown.padding.right = 20;

            Box = new GUIStyle(GUI.skin.box);
            Button = new GUIStyle(GUI.skin.button);
            TextArea = new GUIStyle(GUI.skin.textArea);
            Label = new GUIStyle(GUI.skin.label);
            SelectedLabel = new GUIStyle(MainSkin.customStyles[(int)CustomStyle.SelectedLabel]);
            Toggle = new GUIStyle(GUI.skin.toggle);
            Textfield = new GUIStyle(GUI.skin.textField);
            //ShaderIcon = EditorGUIUtility.IconContent( "Shader Icon" ).image;
            //MaterialIcon = EditorGUIUtility.IconContent( "Material Icon" ).image;

            NodeWindowOffSquare = GetCustomStyle(CustomStyle.NodeWindowOffSquare);
            NodeHeaderSquare = GetCustomStyle(CustomStyle.NodeHeaderSquare);
            NodeWindowOnSquare = GetCustomStyle(CustomStyle.NodeWindowOnSquare);

            UnZoomedNodeTitleStyle = new GUIStyle(GetCustomStyle(CustomStyle.NodeTitle));
            UnZoomedNodeTitleStyle.fontSize = 13;

            UnZoomedPropertyValuesTitleStyle = new GUIStyle(GetCustomStyle(CustomStyle.PropertyValuesTitle));
            UnZoomedPropertyValuesTitleStyle.fontSize = 11;

            UnZoomedInputPortStyle = new GUIStyle(InputPortLabel);
            UnZoomedInputPortStyle.fontSize = (int)Constants.DefaultFontSize;

            UnZoomedOutputPortPortStyle = new GUIStyle(OutputPortLabel);
            UnZoomedOutputPortPortStyle.fontSize = (int)Constants.DefaultFontSize;

            ObjectFieldThumb = new GUIStyle((GUIStyle)"ObjectFieldThumb");
            ObjectFieldThumbOverlay = new GUIStyle((GUIStyle)"ObjectFieldThumbOverlay");

            FloatIntPickerONOFF = new GUIStyle("metimelabel");
            FloatIntPickerONOFF.padding.left = -2;
            FloatIntPickerONOFF.margin = new RectOffset(0, 2, 2, 2);

            TooltipBox = new GUIStyle((GUIStyle)"Tooltip");
            TooltipBox.richText = true;
            FetchMenuItemStyles();
        }
        private static void FetchMenuItemStyles()
        {
            ObjectFieldThumb = new GUIStyle((GUIStyle)"ObjectFieldThumb");
            ObjectFieldThumbOverlay = new GUIStyle((GUIStyle)"ObjectFieldThumbOverlay");
            MenuItemToggleStyle = new GUIStyle((GUIStyle)"foldout");
            MenuItemEnableStyle = new GUIStyle((GUIStyle)"OL ToggleWhite");
            MenuItemBackgroundStyle = new GUIStyle((GUIStyle)"TE NodeBackground");
            MenuItemToolbarStyle = new GUIStyle((GUIStyle)"toolbarbutton") { fixedHeight = 20 };
            MenuItemInspectorDropdownStyle = new GUIStyle((GUIStyle)"toolbardropdown") { fixedHeight = 20 };
            MenuItemInspectorDropdownStyle.margin.bottom = 2;


            InspectorPopdropdownStyle = new GUIStyle(GUI.skin.GetStyle("PopupCurveDropdown"));
            InspectorPopdropdownStyle.alignment = TextAnchor.MiddleRight;
            InspectorPopdropdownStyle.border.bottom = 16;

            InspectorPopdropdownFallback = new GUIStyle(InspectorPopdropdownStyle);
            InspectorPopdropdownFallback.overflow = new RectOffset(0, -5, 0, 0);

            PlusStyle = (EditorGUIUtility.isProSkin) ? new GUIStyle(GetCustomStyle(CustomStyle.CustomExpressionAddItem)) : new GUIStyle((GUIStyle)"OL Plus");
            PlusStyle.imagePosition = ImagePosition.ImageOnly;
            PlusStyle.overflow = new RectOffset(-2, 0, -4, 0);

            MinusStyle = (EditorGUIUtility.isProSkin) ? new GUIStyle(GetCustomStyle(CustomStyle.CustomExpressionRemoveItem)) : new GUIStyle((GUIStyle)"OL Minus");
            MinusStyle.contentOffset = Vector2.zero;
            MinusStyle.imagePosition = ImagePosition.ImageOnly;
            MinusStyle.overflow = new RectOffset(-2, 0, -4, 0);

            ToolbarSearchTextfield = new GUIStyle((GUIStyle)"ToolbarSeachTextField");
            ToolbarSearchCancelButton = new GUIStyle((GUIStyle)"ToolbarSeachCancelButton");
        }


        public static void MarkUndoAction()
        {
            SerializeHelperCounter = 2;
        }

        public static bool SerializeFromUndo()
        {
            if (SerializeHelperCounter > 0)
            {
                SerializeHelperCounter--;
                return true;
            }
            return false;
        }
    }
}
