using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Quantized.Game.Behavior
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Behavior AI edit.
	/// </summary>
    [CustomEditor(typeof(BehaviorAI))]
    [CanEditMultipleObjects]
    public class BehaviorAIEdit : UnityEditor.Editor
    {
        private static readonly float ICO_LOGO_WIDTH = 128f;
        private static readonly float ICO_LOGO_HEIGHT = 64f;
        private static readonly string DEFAULT_XML = "<root>\n" +
                                                     "\t<selector>\n" +
                                                     "\t</selector>\n" +
                                                     "</root>";

        #region values
        private SerializedProperty xmlAI;
        private SerializedProperty mono;
		private SerializedProperty updateAllTree;
        private SerializedProperty waitTime;
		private Texture2D imgLogo;
        private BehaviorAI behTarget;
        #endregion

        #region msg
        public void OnEnable()
        {
            xmlAI = serializedObject.FindProperty("xmlAI");
			updateAllTree = serializedObject.FindProperty("updateAllTree");
            waitTime = serializedObject.FindProperty("waitTime");

			imgLogo = (Texture2D)Resources.Load("logo");
            behTarget = target as BehaviorAI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
			EditorGUILayout.BeginVertical();
			{
				UnityEngine.GUILayout.Space(10);

				Rect r = EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
                    GUILayout.Label(imgLogo, GUILayout.MaxHeight(ICO_LOGO_HEIGHT), GUILayout.MaxWidth(ICO_LOGO_WIDTH));
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
					UnityEngine.GUILayout.Space(5);
				}

				EditorGUI.DrawRect(new Rect(r.position.x, r.position.y + r.height + 5f, r.width, 1f), Color.black);

				UnityEngine.GUILayout.Space(10);
	            EditorGUILayout.PropertyField(xmlAI);
				EditorGUILayout.PropertyField(updateAllTree);
                EditorGUILayout.PropertyField(waitTime);
	            UnityEngine.GUILayout.Space(10);

				EditorGUILayout.BeginHorizontal();
				{
                    TextAsset ta = xmlAI.objectReferenceValue as TextAsset;
                    if (ta != null)
                    {
                        GUILayout.Label("Edit Behaviour", GUILayout.Width(EditorGUIUtility.labelWidth - EditorGUIUtility.standardVerticalSpacing * 5f));
                        if (UnityEngine.GUILayout.Button("Edit", GUILayout.ExpandWidth(false)))
                        {
                            BehaviorEditWindow.Init();
                        }
                    }
                    else
                    {
                        GUILayout.Label("Add Behaviour", GUILayout.Width(EditorGUIUtility.labelWidth - EditorGUIUtility.standardVerticalSpacing * 5f));
                        if (UnityEngine.GUILayout.Button("Add Default", GUILayout.ExpandWidth(false)))
                        {
                            SaveDefaultXml();
                        }
                    }

					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();

				UnityEngine.GUILayout.Space(10);
			}
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region methods
        private void SaveDefaultXml()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save default tree", 
                "default", 
                "xml",
                "Please enter a file name to save the Behaviour tree"
            );

            if (path.Length > 0)
            {
                System.IO.File.WriteAllText(path, DEFAULT_XML);
                AssetDatabase.Refresh();

                TextAsset newTxt = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                behTarget.xmlAI = newTxt;
            }
        }
        #endregion
    }
}