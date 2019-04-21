using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class GravityAttractorTrigger : MonoBehaviour
{
    [Serializable]
    public struct TriggerCountExit
    {
        public GameObject objectTriggered;
        public int count;

        public TriggerCountExit(GameObject obj, int _count)
        {
            objectTriggered = obj;
            count = _count;
        }
    }

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("enum to interact")]
    private List<TagAccess.TagAccessEnum> tagList = new List<TagAccess.TagAccessEnum>()
    {
        TagAccess.TagAccessEnum.Player,
        TagAccess.TagAccessEnum.Enemy,
        TagAccess.TagAccessEnum.Object
    };

    [FoldoutGroup("Debug"), SerializeField, Tooltip("enum to interact")]
    private List<TriggerCountExit> countCancelExit = new List<TriggerCountExit>();

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref of the main GravityAttractor")]
    public GravityAttractorLD refGravityAttractor = null;

    private int ContainObject(GameObject obj)
    {
        for (int i = 0; i < countCancelExit.Count; i++)
        {
            if (obj.GetInstanceID() == countCancelExit[i].objectTriggered.GetInstanceID())
                return (i);
        }
        return (-1);
    }
    
    private bool ManagerEnterCancel(GameObject other)
    {
        int contain = ContainObject(other);
        if (contain != -1)
        {
            TriggerCountExit newTrigger = countCancelExit[contain];
            newTrigger.count++;
            countCancelExit[contain] = newTrigger;
            return (false);
        }
        else
        {
            countCancelExit.Add(new TriggerCountExit(other, 1));
            return (true);
        }
    }

    private bool ManageExitCancel(GameObject other)
    {
        int contain = ContainObject(other);
        if (contain != -1)
        {
            TriggerCountExit newTrigger = countCancelExit[contain];
            newTrigger.count--;
            if (newTrigger.count < 0)
                newTrigger.count = 0;
            countCancelExit[contain] = newTrigger;

            return (countCancelExit[contain].count == 0);
        }
        else
        {
            //error, can't be
            return (false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
		if (ExtList.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                if (entityNoGravity.entityTriggerManager.uniqueGravityAttractorSwitch)
                {
                    ManagerEnterCancel(other.gameObject);
                    entityNoGravity.entityTriggerManager.uniqueGravityAttractorSwitch.EnterInZone(refGravityAttractor);
                }
            }
		}
    }

	private void OnTriggerExit(Collider other)
	{
		if (ExtList.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                if (entityNoGravity.entityTriggerManager.uniqueGravityAttractorSwitch)
                {
                    if (ManageExitCancel(other.gameObject))
                    {
                        entityNoGravity.entityTriggerManager.uniqueGravityAttractorSwitch.LeanInZone(refGravityAttractor);
                    }
                } 
            }
        }
	}
}
