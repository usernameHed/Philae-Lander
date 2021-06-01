using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute;

namespace UnityEssentials.Attractor.editor
{
	///<summary>
	/// 
	///</summary>
	[CustomEditor(typeof(AttractorGroup))]
    public class AttractorGroupEditor : Editor
    {
        /// <summary>
        /// pass list of GameObject (that contain hopefully GravityFieldsActions,
        /// and set it to the list
        /// </summary>
        /// <param name="actions"></param>
		//public void FillGroupList(List<GameObject> actionsGameObject)
        //{
        //    serializedObject.Update();
        //
        //    List<Attractor> actions = new List<Attractor>(actionsGameObject.Count);
        //    for (int i = 0; i < actionsGameObject.Count; i++)
        //    {
        //        Attractor action = actionsGameObject[i].GetComponentInChildren<Attractor>();
        //        if (action != null)
        //        {
        //            actions.AddIfNotContain(action);
        //        }
        //    }
        //
        //    SerializedProperty gravityFields = serializedObject.FindProperty("_gravityFields");
        //    ExtSerializedProperties.SetListComponents<Attractor>(gravityFields, actions);
        //    serializedObject.ApplyModifiedProperties();
        //}
    }
}