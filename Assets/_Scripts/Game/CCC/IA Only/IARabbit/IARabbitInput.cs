using UnityEngine;


/// <summary>
/// InputPlayer Description
/// </summary>
public class IARabbitInput : EntityAction
{
    [Tooltip(""), SerializeField]
    protected IARabbitController iaRabbitController;
    public IARabbitController IaRabbitController() => iaRabbitController;
    /// <summary>
    /// tout les input du jeu, à chaque update
    /// </summary>
    public void SetRandomInput()
    {
        //all axis
        moveInput = new Vector2(ExtRandom.GetRandomNumber(-1f, 1f), ExtRandom.GetRandomNumber(-1f, 1f));
    }

    public void SetRandomJump()
    {
        //all button
        Jump = ExtRandom.GetRandomBool();
        JumpUp = false;
    }

    public void SetJump()
    {
        Jump = true;
        JumpUp = false;
    }

    public void SetDirectionPlayerOut()
    {
        Vector3 forwardLocalIA = GetMainReferenceForwardDirection();//iaFollowerController.rb.transform.forward;
        Vector3 iaDir = iaRabbitController.rb.transform.transform.position - iaRabbitController.playerController.rb.transform.position;
        Vector3 upLocalIA = iaRabbitController.rb.transform.up;

        Debug.DrawRay(iaRabbitController.rb.transform.position, iaDir, Color.white, 5f);
        Debug.DrawRay(iaRabbitController.rb.transform.position, forwardLocalIA, Color.blue, 5f);

        Vector3 left = ExtQuaternion.CrossProduct(forwardLocalIA, upLocalIA);
        Vector3 right = -left;

        Debug.DrawRay(iaRabbitController.rb.transform.position, left, Color.green, 5f);
        Debug.DrawRay(iaRabbitController.rb.transform.position, right, Color.green, 5f);

        float dotRight = ExtQuaternion.DotProduct(right, iaDir);
        float dotLeft = ExtQuaternion.DotProduct(left, iaDir);
        //Debug.Log("left: " + dotLeft + ", right: " + dotRight);
        //Quaternion turret = ExtQuaternion.TurretLookRotation(-iaDir, iaController.objRotateRef.up);
        //Vector3 dirUp = turret.eulerAngles;

        //moveInput = new Vector2(-iaDir.y, -iaDir.z);
        //Debug.DrawRay(iaController.rigidBodyRef.position, moveInput, Color.yellow, 5f);

        float xInput = 0;
        if (dotRight > 0)
        {
            xInput = 1;
            //Debug.Log("go right");
        }
        else if (dotLeft > 0)
        {
            xInput = -1;
            //Debug.Log("go left");
        }
        else
        {
            xInput = 0f;
            //Debug.Log("go pls");
        }

        moveInput = new Vector2(xInput, 0);

        //Debug.Break();
        Jump = false;
        JumpUp = false;
    }
}
