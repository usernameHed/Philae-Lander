using Borodar.RainbowHierarchy;
using UnityEditor;
using UnityEngine;

public class SceneGUI : EditorWindow
{
    public static bool isEnabled = false;

    //here all script
    private static GameManager gameManager;
    private static PlayerController playerController;
    private static PhilaeManager philaeManager;

    private static RaycastHit saveRaycastHit;

    private static GameObject hoverSceneView;
    private static Vector3 pointHitSceneView;

    [MenuItem("PERSO/Enable GameManager GUI")]
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Enable()
    {
        if (isEnabled)
        {
            Disable();
            return;
        }

        SceneView.onSceneGUIDelegate += OnScene;
        Debug.Log("Scene GUI : Enabled");
        isEnabled = true;

        SetupAllScripts();
        //if (!EditorApplication.isPlaying)
        //    GravityCalculs();
    }

    //[MenuItem("PERSO/Scene GUI/Disable")]
    public static void Disable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
        Debug.Log("Scene GUI : Disabled");
        isEnabled = false;
    }

    private static void SetupAllScripts()
    {
        gameManager = UtilityEditor.GetScript<GameManager>();
        playerController = UtilityEditor.GetScript<PlayerController>();
        philaeManager = UtilityEditor.GetScript<PhilaeManager>();
    }

    private static void ResetupIfNull()
    {
        if (!gameManager || !playerController)
        {
            SetupAllScripts();
        }
    }
    

    private static void AllGUI(SceneView view)
    {
        Handles.BeginGUI();


        GUIGameManager(view);

        Handles.EndGUI();
    }

    private static void DisplayStringIn3D(Vector3 position, string toDisplay)
    {
        if (!Application.isPlaying)
            return;

        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 14;
        textStyle.normal.textColor = Color.black;
        textStyle.alignment = TextAnchor.MiddleCenter;
        Handles.Label(position, toDisplay, textStyle);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CalledWhenScriptOrGamePlay()
    {
        if (philaeManager && !Application.isPlaying)
            philaeManager.needRecalculate = true;
    }

    
    private static void GravityCalculs()
    {
        if (Application.isPlaying)
            return;

        if (philaeManager == null)
            return;
        for (int i = 0; i < philaeManager.ldManager.allGravityAttractorLd.Count; i++)
        {
            if (philaeManager.ldManager.allGravityAttractorLd[i] == null)
                continue;
            UtilityEditor.AddCustomEditorToObject(philaeManager.ldManager.allGravityAttractorLd[i].gameObject, false);
        }

        philaeManager.ldManager.FillList(true);
        for (int i = 0; i < philaeManager.ldManager.allGravityAttractorLd.Count; i++)
        {
            UtilityEditor.AddCustomEditorToObject(philaeManager.ldManager.allGravityAttractorLd[i].gameObject, true, HierarchyIcon.None, false, Borodar.RainbowCore.CoreBackground.ClrIndigo, false);
        }
    }

    private static void PhilaeManagerUI(SceneView view)
    {
        if (Application.isPlaying || philaeManager == null)
            return;

        GUILayout.BeginArea(new Rect(10, view.camera.pixelRect.size.y - 50, 100, 100));
        GUILayout.BeginVertical();
        GUI.backgroundColor = Color.gray;
        GUI.color = Color.white;

        if (GUILayout.Button("Gravity Calculs"))
        {
            GravityCalculs();
        }
        if (philaeManager.needRecalculate)
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("/!\\ need recalcul");
            GravityCalculs();
        }
            
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private static void GUIGameManager(SceneView view)
    {
        //
        //GUILayout.Label("hi");

        if (playerController)
            DisplayStringIn3D(playerController.rb.position + Vector3.up * 0.7f, playerController.rb.velocity.magnitude.ToString("F2"));


        PhilaeManagerUI(view);

        ////////// slider move 
        //GUILayout.BeginArea(new Rect(EditorGUIUtility.currentViewWidth - 200, view.camera.pixelRect.size.y - 30, 100, 100));
        
        
    }

    /// <summary>
    /// Display the preview point
    /// </summary>
    private static void SetCurrentOverObject()
    {
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //first a test only for point
        //if (Physics.Raycast(worldRay, out saveRaycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer(gravityAttractorEditor.layerPoint), QueryTriggerInteraction.Ignore))
        if (Physics.Raycast(worldRay, out saveRaycastHit, Mathf.Infinity))
        {
            if (saveRaycastHit.collider.gameObject != null)
            {
                hoverSceneView = saveRaycastHit.collider.gameObject;
                pointHitSceneView = saveRaycastHit.point;
            }
        }
    }

    private static void EventInput(SceneView view, Event e)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                //GUIUtility.hotControl = controlID;
                //Debug.Log("Left Click");
                break;

        }
        /*
        if (e.button == 1 && e.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseUp && gameManager.hoverSceneView && gameManager.hoverSceneView.HasComponent<GostCamera>())
        {
            gameManager.hoverSceneView.GetComponent<GostCamera>().ClickOnGost();
            //Unity.EditorCoroutines.Editor.EditorCoroutine
        }
        if (e.button == 1 && e.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown && gameManager.hoverSceneView && gameManager.hoverSceneView.HasComponent<Spline>())
        {
            gameManager.ClickOnSpline(gameManager.posPoint);
        }
        */
    }

    private static void OwnGUI()
    {
        SceneView view = SceneView.currentDrawingSceneView;
        Event e = Event.current;

        ResetupIfNull();

        AllGUI(view);
        SetCurrentOverObject();
        EventInput(view, e);
    }

    private static void OnScene(SceneView sceneview)
    {
        OwnGUI();
    }
}
