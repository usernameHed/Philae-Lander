using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[CustomEditor(typeof(GravityAttractorEditor))]
public class GravityAttractorPointCreator : OdinEditor
{
    private GravityAttractorEditor gravityAttractorEditor;
    public enum MouseClicType
    {
        CLICK,
        HOLD,
        RELEASE,
        NONE,
    }

    private Vector3 oldPos;
    private RaycastHit saveRaycastHit;
    

    private new void OnEnable()
    {
        EditorApplication.update += OwnUpdate;
        gravityAttractorEditor = (GravityAttractorEditor)target;
    }

    public void SetupPointCreator()
    {
        if (!gravityAttractorEditor.previewPoint)
        {
            gravityAttractorEditor.GenerateParenting();
            if (gravityAttractorEditor.GetGravityAttractor())
            {
                gravityAttractorEditor.GetGravityAttractor().philaeManager = ExtUtilityEditor.GetScript<PhilaeManager>();
                gravityAttractorEditor.GetGravityAttractor().philaeManager.ldManager.FillList(false);
            }
        }
        ExtPhilaeEditor.AssignLabel(gravityAttractorEditor.previewPoint.gameObject, 0);
    }

    private Vector3 GetPointOfTriangle(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return (ExtUtilityFunction.GetNullVector());

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        if (triangles.Length == 0)
            return (Vector3.zero);

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);

        int indexFound = -1;
        return (ExtUtilityFunction.GetClosestPoint(hit.point, new Vector3[] { p0, p1, p2 }, ref indexFound));        
    }

    /// <summary>
    /// Display the preview point
    /// </summary>
    private void PreviewPointDisplay()
    {
        gravityAttractorEditor.previewPoint.gameObject.SetActive(gravityAttractorEditor.createMode == 1);


        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //first a test only for point
        if (Physics.Raycast(worldRay, out saveRaycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer(gravityAttractorEditor.layerPoint), QueryTriggerInteraction.Ignore))
        {
            if (saveRaycastHit.collider.gameObject != null)
            {
                gravityAttractorEditor.objectPreview = saveRaycastHit.collider.gameObject;
                gravityAttractorEditor.previewPoint.position = saveRaycastHit.point;
            }
        }
        //then a test for everythinng else
        else if (Physics.Raycast(worldRay, out saveRaycastHit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
        {
            if (saveRaycastHit.collider.gameObject != null)
            {
                gravityAttractorEditor.objectPreview = saveRaycastHit.collider.gameObject;
                gravityAttractorEditor.previewPoint.position = saveRaycastHit.point;
            }
            else
            {
                gravityAttractorEditor.objectPreview = null;
                gravityAttractorEditor.previewPoint.gameObject.SetActive(false);
            }
        }
        else
        {
            gravityAttractorEditor.objectPreview = null;
            gravityAttractorEditor.previewPoint.gameObject.SetActive(false);
        }

        if (gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview))
        {
            gravityAttractorEditor.previewPoint.gameObject.SetActive(false);
        }
    }

    private void PreviewPointToMerge()
    {
        for (int i = 0; i < gravityAttractorEditor.tmpPointCreated.Count; i++)
        {
            ExtPhilaeEditor.AssignLabel(gravityAttractorEditor.tmpPointCreated[i], 1);
        }

        if (gravityAttractorEditor.createMode != 2)
            return;

        gravityAttractorEditor.CleanUpTmpPointForm();

        for (int i = 0; i < gravityAttractorEditor.tmpForm.Count; i++)
        {
            gravityAttractorEditor.tmpForm[i].name = i.ToString();
            ExtPhilaeEditor.AssignLabel(gravityAttractorEditor.tmpForm[i], 2);
        }
    }

    private void CorrectPositionPreview(Event e)
    {
        if (e.shift && gravityAttractorEditor.objectPreview && gravityAttractorEditor.objectPreview.activeInHierarchy)
        {
            gravityAttractorEditor.previewPoint.position = Selection.activeGameObject.transform.position;
        }
        else if (e.control && gravityAttractorEditor.objectPreview && gravityAttractorEditor.objectPreview.activeInHierarchy)
        {
            gravityAttractorEditor.previewPoint.position = GetPointOfTriangle(saveRaycastHit);
        }
    }

    private GameObject CreatePoint(Vector3 pos)
    {
        GameObject newObjParent = new GameObject(gravityAttractorEditor.tmpPointCreated.Count.ToString());
        newObjParent.transform.SetParent(gravityAttractorEditor.parentTmpPoints);
        newObjParent.transform.position = pos;
        newObjParent.layer = LayerMask.NameToLayer(gravityAttractorEditor.layerPoint);
        SphereCollider sphere = newObjParent.AddComponent<SphereCollider>();
        sphere.radius = 0.25f;
        sphere.isTrigger = false;
        ExtPhilaeEditor.AssignLabel(newObjParent, 1);

        // Register root object for undo.
        //Undo.RegisterCreatedObjectUndo(newObjParent, "Create object");
        AddMergePoint(newObjParent);

        return (newObjParent);
    }

    private void AddMergePoint(GameObject obj)
    {
        gravityAttractorEditor.tmpForm.Add(obj);
        if (gravityAttractorEditor.tmpForm.Count > 4)
            gravityAttractorEditor.tmpForm.RemoveAt(0);
    }

    private void TryToCreateMergePoint(Event e)
    {
        if (gravityAttractorEditor.createMode != 2)
            return;
        //create point
        if (gravityAttractorEditor.objectPreview 
            && gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview)
            && !gravityAttractorEditor.tmpForm.Contains(gravityAttractorEditor.objectPreview)
            && e.button == 1)
        {
            AddMergePoint(gravityAttractorEditor.objectPreview);
            /*
            gravityAttractorEditor.tmpForm.Add(gravityAttractorEditor.objectPreview);
            if (gravityAttractorEditor.tmpForm.Count > 4)
                gravityAttractorEditor.tmpForm.RemoveAt(0);
                */
            //e.Use();
        }
        else if (gravityAttractorEditor.objectPreview
            && !gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview)
            && !gravityAttractorEditor.tmpForm.Contains(gravityAttractorEditor.objectPreview)
            && e.button == 1
            && e.control && e.shift
            && gravityAttractorEditor.objectPreview.layer == LayerMask.NameToLayer("Point"))
        {
            AddMergePoint(gravityAttractorEditor.objectPreview);
            //e.Use();
        }
        


        else if (gravityAttractorEditor.objectPreview
            && gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview)
            && gravityAttractorEditor.tmpForm.Contains(gravityAttractorEditor.objectPreview)
            && e.button == 1)
        {
            gravityAttractorEditor.tmpForm.Remove(gravityAttractorEditor.objectPreview);
            gravityAttractorEditor.CleanUpTmpPointForm();
            //e.Use();
        }
        else if (gravityAttractorEditor.objectPreview
            && !gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview)
            && gravityAttractorEditor.tmpForm.Contains(gravityAttractorEditor.objectPreview)
            && e.button == 1
            && gravityAttractorEditor.objectPreview.layer == LayerMask.NameToLayer("Point"))
        {
            gravityAttractorEditor.tmpForm.Remove(gravityAttractorEditor.objectPreview);

            ExtPhilaeEditor.AssignLabel(gravityAttractorEditor.objectPreview, 0);

            gravityAttractorEditor.CleanUpTmpPointForm();
            //e.Use();
        }
    }

    private void TryToValidateForm(Event e)
    {
        if (!(gravityAttractorEditor.createMode == 2  || gravityAttractorEditor.createMode == 1))
            return;

        if (e.keyCode == KeyCode.Return && e.type == EventType.KeyUp)
        {
            Debug.Log("ici yeah");
            gravityAttractorEditor.SetupAForm();

            for (int i = 0; i < gravityAttractorEditor.tmpForm.Count; i++)
            {
                ExtPhilaeEditor.AssignLabel(gravityAttractorEditor.tmpForm[i], 4);
            }
            gravityAttractorEditor.tmpForm.Clear();

            //gravityAttractorEditor.createMode = 0;
            //Vector2 pos2dObject = view.camera.WorldToViewportPoint(ldGravityAttractor.GetAllGravityPoint()[i].position);
            //objSelected = null;
        }
    }

    private void TryToCreatePoint(Event e)
    {
        if (gravityAttractorEditor.createMode != 1)
            return;

        //create point
        if (gravityAttractorEditor.objectPreview && !e.alt
            && e.button == 1 && !gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview))
        {
            Undo.RecordObject(gravityAttractorEditor, "list creation");

            gravityAttractorEditor.tmpPointCreated.Add(CreatePoint(gravityAttractorEditor.previewPoint.position));

            //e.Use();

            //SceneView.lastActiveSceneView.pivot = gravityAttractorEditor.transform.position;
            //SceneView.lastActiveSceneView.Repaint();
        }
        //delete point
        else if (gravityAttractorEditor.tmpPointCreated.Contains(gravityAttractorEditor.objectPreview) && e.control
            && e.alt && e.button == 1)
        {
            gravityAttractorEditor.tmpPointCreated.Remove(gravityAttractorEditor.objectPreview);
            Undo.RecordObject(gravityAttractorEditor.objectPreview, "destroy point");
            DestroyImmediate(gravityAttractorEditor.objectPreview);
            gravityAttractorEditor.CleanTheSick();
        }
    }

    private void ActiveOrNotToggle(Event e)
    {
        if (e.keyCode == KeyCode.C && e.type == EventType.KeyUp)
        {
            if (gravityAttractorEditor.createMode == 1)
                gravityAttractorEditor.createMode = 0;
            else
                gravityAttractorEditor.createMode = 1;
        }
        if (e.keyCode == KeyCode.V && e.type == EventType.KeyUp)
        {
            if (gravityAttractorEditor.createMode == 2)
                gravityAttractorEditor.createMode = 0;
            else
                gravityAttractorEditor.createMode = 2;
        }
    }

    private void EventInput(SceneView view, Event e)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        ActiveOrNotToggle(e);

        CorrectPositionPreview(e);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:

                break;
            case EventType.MouseUp:

                TryToCreatePoint(e);
                TryToCreateMergePoint(e);
                break;

            case EventType.MouseDrag:

                break;
        }

        TryToValidateForm(e);


        if (e.keyCode == KeyCode.Escape)
        {
            gravityAttractorEditor.createMode = 0;
            //Vector2 pos2dObject = view.camera.WorldToViewportPoint(ldGravityAttractor.GetAllGravityPoint()[i].position);
            //objSelected = null;
        }
    }

    private void OwnUpdate()
    {
        if (gravityAttractorEditor.isUpdatedFirstTime)
        {
            SetupPointCreator();
            gravityAttractorEditor.isUpdatedFirstTime = false;
        }
    }

    /// <summary>
    /// Displays a vertical list of toggles and returns the index of the selected item.
    /// </summary>
    public static int ToggleList(int selected, string[] items)
    {
        // Keep the selected index within the bounds of the items array
        selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

        GUILayout.BeginVertical();
        for (int i = 0; i < items.Length; i++)
        {
            // Display toggle. Get if toggle changed.
            bool change = GUILayout.Toggle(selected == i, items[i]);
            // If changed, set selected to current index.
            if (change)
                selected = i;
        }
        GUILayout.EndVertical();

        // Return the currently selected item's index
        return selected;
    }

    private void ButtonSceneViewDisplay()
    {
        Handles.BeginGUI();
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        GUILayout.BeginVertical();

        GUI.backgroundColor = Color.gray;
        GUI.color = Color.white;
        GUILayout.Label("--- GA ---");
        GUILayout.Label("Press G to focus parent GA");
        GUILayout.Label("CTRL + G: create GA");
        GUILayout.Label("CTRL + ALT + G: delete GA");


        GUI.backgroundColor = Color.white;

        gravityAttractorEditor.createMode = ToggleList(gravityAttractorEditor.createMode, new string[] { "desactive (C or V)", "Create mode (C)", "Validate mode (V)" });

        GUILayout.EndVertical();
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    private void ResetPoints()
    {
        gravityAttractorEditor.previewPoint.gameObject.SetActive(false);
    }

    void OnSceneGUI()
    {
        if (!Application.IsPlaying(gravityAttractorEditor))
        {
            if (Selection.activeGameObject && Selection.activeGameObject != gravityAttractorEditor.gameObject)
            {
                ResetPoints();
                return;
            }
                

            SceneView view = SceneView.currentDrawingSceneView;
            gravityAttractorEditor.CleanUpTmpPointForm();
            gravityAttractorEditor.CleanUpPointCreated();

            Event e = Event.current;

            PreviewPointDisplay();
            PreviewPointToMerge();
            EventInput(view, e);
            ButtonSceneViewDisplay();
        }
    }

    private new void OnDisable()
    {
        EditorApplication.update -= OwnUpdate;
    }

    public void OnDestroy()
    {
        if (Application.isEditor)
        {
            PhilaeManager phiTmp = ExtUtilityEditor.GetScript<PhilaeManager>();
            
            if (phiTmp)
                phiTmp.ldManager.FillList(false);
        }
    }
}