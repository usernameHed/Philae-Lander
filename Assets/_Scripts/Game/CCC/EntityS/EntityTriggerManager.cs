﻿using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class EntityTriggerManager : MonoBehaviour
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public GameObject objectToKill;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityNoGravity entityNoGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public UniqueGravityAttractorSwitch uniqueGravityAttractorSwitch;


    /*
    public void OwnTriggerEnter(Collider other)
    {

    }
    
    public void OwnTriggerStay(Collider other)
    {

    }

    public void OwnTriggerExit(Collider other)
    {

    }

    public void OwnCollisionEnter(Collision collision)
    {

    }

    public void OwnCollisionStay(Collision collision)
    {

    }

    public void OwnCollisionExit(Collision collision)
    {

    }
    */

    public void Kill()
    {
        if (objectToKill == null)
            return;
        IKillable kill = objectToKill.GetComponent<IKillable>();

        if (kill != null)
            kill.Kill();
    }
}
