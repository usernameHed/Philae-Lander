
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.readOnly;

public class EntitySaveLastPoint : MonoBehaviour
{
    [Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float sizeDistanceForSavePlayerPos = 0.5f;   //a-t-on un attract point de placé ?

    [SerializeField, Tooltip("ref rigidbody")]
    private EntityGravity playerGravity = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController = null;
    
    [SerializeField, Tooltip("ref script"), ReadOnly]
    private Vector3[] worldLastPosition = new Vector3[3];      //save la derniere position grounded...
    [Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float differenceAngleNormalForUpdatePosition = 5f;   //a-t-on un attract point de placé ?

    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        FillAllPos();
    }

    /// <summary>
    /// fill the array at start
    /// </summary>
    private void FillAllPos()
    {
        for (int i = 0; i < worldLastPosition.Length; i++)
        {
            worldLastPosition[i] = entityController.rb.transform.position;
        }
    }

    /// <summary>
    /// Set the last position
    /// decalle other positions down
    /// </summary>
    private void WorldLastPositionSet(Vector3 newValue)
    {
        Vector3 next = Vector3.zero;
        for (int i = 0; i < worldLastPosition.Length - 1; i++)
        {
            if (i == 0)
            {
                next = worldLastPosition[0];
                worldLastPosition[0] = newValue;
            }
            else
            {
                Vector3 tmpValue = worldLastPosition[i];
                worldLastPosition[i] = next;
                next = tmpValue;
            }
        }
    }
    private Vector3 WorldLastPositionGetIndex(int index)
    {
        index = (index < 0) ? 0 : index;
        index = (index >= worldLastPosition.Length) ? worldLastPosition.Length - 1 : index;
        return (worldLastPosition[index]);
    }

    /// <summary>
    /// save the last position on ground
    /// </summary>
    public void SaveLastPositionOnground()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            return;

        worldLastNormal = playerGravity.GetMainAndOnlyGravity();   //avoir toujours une normal à jour
        float distForSave = (WorldLastPositionGetIndex(0) - entityController.rb.transform.position).sqrMagnitude;

        //Debug.Log("dist save: " + distForSave);
        //si la distance entre les 2 point est trop grande, dans tout les cas, save la nouvelle position !
        if (distForSave > sizeDistanceForSavePlayerPos)
        {
            WorldLastPositionSet(entityController.rb.transform.position); //save la position onGround
            //ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.red, 0.5f, 1f);
        }
        //si la normal à changé, update la position + normal !
        else if (worldPreviousNormal != worldLastNormal)
        {
            //ici changement de position SEULEMENT si l'angle de la normal diffère de X
            float anglePreviousNormal = Vector3.Angle(worldPreviousNormal, entityController.rbRotateObject.up);
            float angleNormalPlayer = Vector3.Angle(worldLastNormal, entityController.rbRotateObject.up);
            //ici gérer les normal à zero ??
            float diff;
            if (ExtVector3.IsAngleCloseToOtherByAmount(anglePreviousNormal, angleNormalPlayer, differenceAngleNormalForUpdatePosition, out diff))
            {
                //Debug.Log("ici l'angle est trop proche, ducoup ne pas changer de position");

                //ni de normal ??
            }
            else
            {
                //ici change la normal, ET la position
                WorldLastPositionSet(entityController.rb.transform.position); //save la position onGround
                worldPreviousNormal = worldLastNormal;

                //ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.yellow, 0.5f, 1f);
                //Debug.DrawRay(entityController.rb.transform.position, worldPreviousNormal, Color.yellow, 1f);
            }
        }
    }

    public void OnGrounded()
    {

    }
    
    private void FixedUpdate()
    {
        SaveLastPositionOnground();
    }
}
