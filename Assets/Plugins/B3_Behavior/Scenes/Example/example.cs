using UnityEngine;
using System.Collections;

public class example : MonoBehaviour 
{
    private Rigidbody body;
    private bool mousePress;

	public void Start () 
    {
        body = GetComponent<Rigidbody>();
	}
	
	public void LateUpdate () 
    {
        if (!mousePress)
        {
            mousePress = Input.GetMouseButtonDown(0);
        }
	}

    public bool isCanJump
    {
        get
        {
            return Mathf.Abs(body.velocity.y) < 0.05f;
        }
    }

    public bool isJump
    {
        get
        {
            return mousePress;
        }
    }

    public bool isRotate
    {
        get
        {
            return Input.GetMouseButton(1);
        }
    }

    public void Jump()
    {
        body.AddForce(Vector3.up * 150f);
        mousePress = false;
    }

    public void Rotate()
    {
        transform.Rotate(0f, 250f * Time.deltaTime, 0f);
    }
}
