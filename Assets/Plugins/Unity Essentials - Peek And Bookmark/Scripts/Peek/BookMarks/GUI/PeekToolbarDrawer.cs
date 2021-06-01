using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Peek.Extensions;
using UnityEssentials.Peek.Extensions.Editor;
using UnityEssentials.Peek.Extensions.Editor.EventEditor;
using UnityEssentials.Peek.Options;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Peek
{
    /// <summary>
    /// this class is only used to display the content of the PeekToolBar
    /// </summary>
    public class PeekToolbarDrawer
    {
        private const int _heightText = 8;
        public const string OPEN_EDITOR_WINDOW_ICON = "Favorite Icon";
        private const string PREVIOUS_ICON = "◀";
        private const string NEXT_ICON = "►";

        /// <summary>
        /// display the content of the peek toolBar
        /// </summary>
        public bool DisplayPeekToolBar()
        {
            if (PeekSaveDatas.Instance == null)
            {
                return (false);
            }

            if (PeekSerializeObject.CurrentIndex >= PeekSerializeObject.SelectedObjectsCount)
            {
                PeekSerializeObject.SetCurrentIndex(PeekSerializeObject.SelectedObjectsCount - 1);
                PeekSerializeObject.Save(30);
            }

            using (VerticalScope vertical = new VerticalScope())
            {
                GUILayout.Label("Browse selections", ExtGUIStyles.MiniTextCentered, GUILayout.Height(_heightText));
                using (HorizontalScope horizontal = new HorizontalScope())
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent(OPEN_EDITOR_WINDOW_ICON), ExtGUIStyles.MicroButton, GUILayout.Width(28), GUILayout.Height(EditorGUIUtility.singleLineHeight)) && Event.current.button == 0)
                    {
                        PeekLogic.ManageOpenPeekEditorWindow();
                    }
#if UNITY_2018_3_OR_NEWER
                    if (IsRightClickContext())
                    {
                        return (true);
                    }
#endif


                    EditorGUI.BeginDisabledGroup(PeekSerializeObject.SelectedObjectsCount == 0);
                    {
                        float delta = 0;
                        bool isScrollingDown = ExtMouse.IsScrollingDown(Event.current, ref delta);
                        StringBuilder previousTip = new StringBuilder();
                        previousTip.Append("Go to previous selection (");
                        previousTip.Append(ExtShortCutEditor.GetModifierKey(UnityEssentialsPreferences.SHORTCUT_MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_MODIFIER_KEY));
                        previousTip.Append("+");
                        previousTip.Append(ExtShortCutEditor.GetKey(UnityEssentialsPreferences.SHORTCUT_KEY_FOR_PREVIOUS_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_PREVIOUS_KEY));
                        previousTip.Append(")");

                        if ((GUILayout.Button(new GUIContent(PREVIOUS_ICON, previousTip.ToString()), ExtGUIStyles.MicroButton, GUILayout.Width(23), GUILayout.Height(EditorGUIUtility.singleLineHeight)) && Event.current.button == 0) || isScrollingDown)
                        {
                            if (Selection.objects.Length != 0)
                            {
                                PeekLogic.AddToIndex(-1);
                            }
                            PeekLogic.ForceSelection(PeekSerializeObject.SelectedObjectsIndex(PeekSerializeObject.CurrentIndex));
                        }
#if UNITY_2018_3_OR_NEWER
                        if (IsRightClickContext())
                        {
                            return (true);
                        }
#endif
                        bool isScrollingUp = ExtMouse.IsScrollingUp(Event.current, ref delta);
                        StringBuilder nextTip = new StringBuilder();
                        nextTip.Append("Go to next selection (");
                        nextTip.Append(ExtShortCutEditor.GetModifierKey(UnityEssentialsPreferences.SHORTCUT_MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_MODIFIER_KEY));
                        nextTip.Append("+");
                        nextTip.Append(ExtShortCutEditor.GetKey(UnityEssentialsPreferences.SHORTCUT_KEY_FOR_NEXT_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_NEXT_KEY));
                        nextTip.Append(")");

                        if ((GUILayout.Button(new GUIContent(NEXT_ICON, nextTip.ToString()), ExtGUIStyles.MicroButton, GUILayout.Width(23), GUILayout.Height(EditorGUIUtility.singleLineHeight)) && Event.current.button == 0) || isScrollingUp)
                        {
                            PeekLogic.AddToIndex(1);
                            PeekLogic.ForceSelection(PeekSerializeObject.SelectedObjectsIndex(PeekSerializeObject.CurrentIndex));
                        }
#if UNITY_2018_3_OR_NEWER
                        if (IsRightClickContext())
                        {
                            return (true);
                        }
#endif

                        if (PeekSerializeObject.SelectedObjectsCount == 0)
                        {
                            GUIContent gUIContent = new GUIContent("-/-", "there is no previously selected objects");
                            GUILayout.Label(gUIContent);
                        }
                        else
                        {
                            string showCount = (PeekSerializeObject.CurrentIndex + 1).ToString() + "/" + (PeekSerializeObject.SelectedObjectsCount);
                            GUIContent gUIContent = new GUIContent(showCount, "Scroll Up/Down to browse previous/next");
                            GUILayout.Label(gUIContent);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            GUILayout.FlexibleSpace();
            return (false);
        }

#if UNITY_2018_3_OR_NEWER
        private bool IsRightClickContext()
        {
            return Event.current.type == EventType.Repaint &&
                                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)
                                    && Event.current.button == 1;
        }
#endif
    }
}