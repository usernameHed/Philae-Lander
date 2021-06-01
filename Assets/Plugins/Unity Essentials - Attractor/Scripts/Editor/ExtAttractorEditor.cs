using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.Extensions.Editor;

namespace UnityEssentials.Attractor.editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtAttractorEditor
    {
        [MenuItem("GameObject/Unity Essentials/Attractor/Group", false, -100)]
        private static void GroupGravityFields()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                return;
            }

            List<GameObject> selectedWithAttractor = new List<GameObject>(selected.Length);
            for (int i = 0; i < selected.Length; i++)
            {
                Attractor action = selected[i].GetComponentInChildren<Attractor>();
                if (action != null)
                {
                    selectedWithAttractor.Add(selected[i]);
                }
            }

            //first, do not group anything if one of them is already grouped
            for (int i = 0; i < selectedWithAttractor.Count; i++)
            {
                AttractorGroup parent = selectedWithAttractor[i].GetComponentInParent<AttractorGroup>();
                if (parent != null)
                {
                    return;
                }
            }

            Selection.activeGameObject = null;
            Transform commonParent = ExtObjectEditor.FindCommonParent(selectedWithAttractor, selectedWithAttractor[0].transform.parent);
            EditorGUIUtility.PingObject(commonParent);


            GameObject newParent = new GameObject("AttractorGroup");
            newParent.transform.position = ExtVector3.GetMeanOfXPoints(selectedWithAttractor.ToArray(), out Vector3 sizeBoundingBox, true);
            AttractorGroup newGroup = newParent.AddComponent<AttractorGroup>();

            //UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(newGroup, typeof(GravityFieldsGroupEditor));
            //GravityFieldsGroupEditor groupEditor = (GravityFieldsGroupEditor)splineEditorGeneric;
            //groupEditor.FillGroupList(selectedWithGravityFieldsAction);

            SetupSiblingOfNewGroup(selectedWithAttractor, commonParent, newParent);


            ExtReflection.SetExpanded(newParent, true);
            ExtReflection.Rename(newParent);
        }

        private static void SetupSiblingOfNewGroup(List<GameObject> selectedWithAttractor, Transform commonParent, GameObject newParent)
        {
            if (commonParent != null)
            {
                newParent.transform.SetSiblingIndex(commonParent.GetSiblingIndex());
            }
            GameObjectUtility.EnsureUniqueNameForSibling(newParent);

            Undo.RegisterCreatedObjectUndo(newParent, "Create AttractorGroup");
            List<int> index = new List<int>(selectedWithAttractor.Count);
            for (int i = 0; i < selectedWithAttractor.Count; i++)
            {
                index.Add(selectedWithAttractor[i].transform.GetSiblingIndex());
            }
            for (int i = 0; i < selectedWithAttractor.Count; i++)
            {
                Undo.SetTransformParent(selectedWithAttractor[i].transform, newParent.transform, "Move parent");
            }
            ExtTransform.BubbleSortTransforms(index, selectedWithAttractor);
        }



        [MenuItem("GameObject/Unity Essentials/Attractor/UnGroup", false, -100)]
        private static void UnGroupGravityFields()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                return;
            }

            AttractorGroup groupSelected = null;

            List<GameObject> selectedWithGravityFieldsAction = new List<GameObject>(Attractor.DEFAULT_MAX_ATTRACTOR);
            for (int i = 0; i < selected.Length; i++)
            {
                groupSelected = selected[i].GetComponent<AttractorGroup>();
                if (groupSelected != null)
                {
                    break;
                }
            }

            if (groupSelected != null)
            {
                UngroupGroup(groupSelected);
            }
            else
            {
                UngroupAllSelected(selected, selectedWithGravityFieldsAction);
            }
        }

        private static void UngroupGroup(AttractorGroup groupSelected)
        {
            List<GameObject> roots = groupSelected.transform.GetChildsRoot();
            int index = groupSelected.transform.GetSiblingIndex();

            for (int i = roots.Count - 1; i >= 0; i--)
            {
                Undo.SetTransformParent(roots[i].transform, groupSelected.transform.parent, "Move to Parent");
                roots[i].transform.SetSiblingIndex(index);
            }
            Selection.objects = roots.ToArray();
            if (groupSelected.transform.childCount == 0)
            {
                Undo.DestroyObjectImmediate(groupSelected.gameObject);
            }
        }

        private static void UngroupAllSelected(GameObject[] selected, List<GameObject> selectedWithGravityFieldsAction)
        {
            for (int i = 0; i < selected.Length; i++)
            {
                Attractor action = selected[i].GetComponentInChildren<Attractor>();
                if (!selected[i].GetComponent<AttractorGroup>() && action != null && action.GetComponentInParent<AttractorGroup>() != null)
                {
                    selectedWithGravityFieldsAction.AddIfNotContain(selected[i]);
                }
            }
            if (selectedWithGravityFieldsAction.Count == 0)
            {
                return;
            }

            Transform firstParent = selectedWithGravityFieldsAction[0].transform.parent;
            Transform parent = firstParent;
            if (parent != null)
            {
                parent = parent.parent;
            }
            AttractorGroup mainGroup = selectedWithGravityFieldsAction[0].GetComponentInParent<AttractorGroup>();


            List<int> index = new List<int>(selectedWithGravityFieldsAction.Count);
            for (int i = 0; i < selectedWithGravityFieldsAction.Count; i++)
            {
                index.Add(selectedWithGravityFieldsAction[i].transform.GetSiblingIndex());
            }
            int additionnalIndex = 0;
            if (firstParent != null)
            {
                additionnalIndex = firstParent.GetSiblingIndex() + selectedWithGravityFieldsAction.Count;
            }


            for (int i = 0; i < selectedWithGravityFieldsAction.Count; i++)
            {
                Undo.SetTransformParent(selectedWithGravityFieldsAction[i].transform, parent, "Move to Parent");
                
            }
            ExtTransform.BubbleSortTransforms(index, selectedWithGravityFieldsAction);
            for (int i = 0; i < selectedWithGravityFieldsAction.Count; i++)
            {
                selectedWithGravityFieldsAction[i].transform.SetSiblingIndex(selectedWithGravityFieldsAction[i].transform.GetSiblingIndex() + additionnalIndex);
            }


            if (mainGroup.transform.childCount == 0)
            {
                Undo.DestroyObjectImmediate(mainGroup.gameObject);
            }
        }


        [MenuItem("GameObject/Unity Essentials/Attractor/Group", true, -100)]
        private static bool GroupGravityFieldsTest()
        {
            GameObject[] selected = Selection.gameObjects;
            //don't if nothing selected
            if (selected.Length == 0)
            {
                return (false);
            }
            if (!ExtObjectEditor.AreGameObjectInSameScene(selected))
            {
                return (false);
            }
            for (int i = 0; i < selected.Length; i++)
            {
                AttractorGroup parent = selected[i].GetComponentInParent<AttractorGroup>();
                if (parent != null)
                {
                    return (false);
                }
            }

            List<GameObject> selectedWithGravityFieldsAction = new List<GameObject>(selected.Length);
            for (int i = 0; i < selected.Length; i++)
            {
                Attractor action = selected[i].GetComponentInChildren<Attractor>();
                if (action != null)
                {
                    selectedWithGravityFieldsAction.Add(selected[i]);
                }
            }
            if (selectedWithGravityFieldsAction.Count == 0)
            {
                return (false);
            }

            return (true);
        }
        [MenuItem("GameObject/Unity Essentials/Attractor/UnGroup", true, -100)]
        private static bool UnGroupGravityFieldsTest()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                return (false);
            }
            if (!ExtObjectEditor.AreGameObjectInSameScene(selected))
            {
                return (false);
            }
            AttractorGroup parentCommon = null;
            for (int i = 0; i < selected.Length; i++)
            {
                AttractorGroup parent = selected[i].GetComponentInParent<AttractorGroup>();
                if (parent == null)
                {
                    return (false);
                }
                //can't ungroup 2 GravityFieldsAction of 2 different group...
                if (parentCommon != null && parent != parentCommon)
                {
                    return (false);
                }

                parentCommon = parent;
            }
            return (true);
        }
    }
}