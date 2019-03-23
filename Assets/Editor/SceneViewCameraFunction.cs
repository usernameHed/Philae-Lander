using UnityEngine;
using UnityEditor;

public class SceneViewCameraFunction : ScriptableObject
{
    /*
    [MenuItem("PERSO/Sceneview To Camera _n")]
    //[MenuItem("Edit/Camera _n")]

    public static void MoveSceneViewCamera()

    {		
		GameObject MCamera = GameObject.Find("Main Camera");
        //Vector3 position = SceneView.lastActiveSceneView.pivot;
		Vector3 position = MCamera.transform.position;
		Quaternion rotation = MCamera.transform.rotation;
		bool fov = MCamera.GetComponent<Camera>().orthographic;
        //position.z -= 10.0f;

        SceneView.lastActiveSceneView.pivot = position;
		SceneView.lastActiveSceneView.rotation = rotation;
		//SceneView.lastActiveSceneView.camera.fov = fov;
		//SceneView.lastActiveSceneView.orthographic = true;
		SceneView.lastActiveSceneView.orthographic = fov;			
		SceneView.lastActiveSceneView.LookAt(position);
		SceneView.lastActiveSceneView.size = 0.0f;
		if (MCamera.GetComponent<Camera>().orthographic)
			SceneView.lastActiveSceneView.size = MCamera.GetComponent<Camera>().orthographicSize;
		
        SceneView.lastActiveSceneView.Repaint();
    }
    */

    

    [MenuItem("PERSO/Vieport/Vieporta Zoom In")]
    public static void ViewportPanZoomIn(float zoom = 5f)
    {
        //Debug.Log(SceneView.lastActiveSceneView.size);
        if (SceneView.lastActiveSceneView.size > zoom)
            SceneView.lastActiveSceneView.size = zoom;
        SceneView.lastActiveSceneView.Repaint();
    }

    /*
	[MenuItem("PERSO/Vieport/Rotate Vieport up &i")]
    public static void ViewportHotKeysUp()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		//float r = rotation.eulerAngles.x - 1.0f;
		rotation = rotation*Quaternion.Euler(new Vector3(1.0f,0.0f,0.0f));
		SceneView.lastActiveSceneView.rotation = rotation;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Rotate Vieport down &k")]
    public static void ViewportHotKeysDown()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		//float r = rotation.eulerAngles.x - 1.0f;
		rotation = rotation*Quaternion.Euler(new Vector3(-1.0f,0.0f,0.0f));
		SceneView.lastActiveSceneView.rotation = rotation;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Rotate Vieporta right &l")]
    public static void ViewportHotKeysRight()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		//float r = rotation.eulerAngles.x - 1.0f;
		rotation = Quaternion.Euler(new Vector3(0.0f,-1.0f,0.0f))*rotation ;
		SceneView.lastActiveSceneView.rotation = rotation;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Rotate Vieporta left &j")]
    public static void ViewportHotKeysLeft()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		//float r = rotation.eulerAngles.x - 1.0f;
		rotation = Quaternion.Euler(new Vector3(0.0f,1.0f,0.0f))*rotation;
		SceneView.lastActiveSceneView.rotation = rotation;
		SceneView.lastActiveSceneView.Repaint();	
	}
	
	[MenuItem("PERSO/Vieport/Pan Vieporta up %&i")]
    public static void ViewportPanUp()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		Vector3 pos = rotation * (Vector3.up * (SceneView.lastActiveSceneView.size/100.0f) );
		SceneView.lastActiveSceneView.pivot += pos;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Pan Vieporta down %&k")]
    public static void ViewportPanDown()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		Vector3 pos = rotation * (Vector3.down * (SceneView.lastActiveSceneView.size/100.0f) );
		SceneView.lastActiveSceneView.pivot += pos;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Pan Vieporta right %&l")]
    public static void ViewportPanRight()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		Vector3 pos = rotation * (Vector3.right * (SceneView.lastActiveSceneView.size/100.0f) );
		SceneView.lastActiveSceneView.pivot += pos;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	[MenuItem("PERSO/Vieport/Pan Vieporta left %&j")]
    public static void ViewportPanLeft()
	{
		Quaternion rotation = SceneView.lastActiveSceneView.rotation;
		Vector3 pos = rotation * (Vector3.left * (SceneView.lastActiveSceneView.size/100.0f) );
		SceneView.lastActiveSceneView.pivot += pos;
		SceneView.lastActiveSceneView.Repaint();
	}
	
	
	
	[MenuItem("PERSO/Vieport/Vieporta Zoom Out &p")]
    public static void ViewportPanZoomOut()
	{
		if (SceneView.lastActiveSceneView.size > 0.0f)
			SceneView.lastActiveSceneView.size += (SceneView.lastActiveSceneView.size/50);
		SceneView.lastActiveSceneView.Repaint();
	}
    */
}