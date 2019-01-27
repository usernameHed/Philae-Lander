using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [FoldoutGroup("Object"), Tooltip("planette qui attire"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("planette qui attire"), SerializeField]
    private MeshCollider realCollider;
    [FoldoutGroup("Object"), Tooltip("planette qui attire"), SerializeField]
    private MeshCollider AtmosphereCollider;

    [FoldoutGroup("Debug"), Tooltip("planette qui attire"), SerializeField]
    private bool playerIsInside = false;
    [FoldoutGroup("Debug"), Tooltip("planette qui attire"), SerializeField]
    private bool enemyAreInside = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameData.Layers.Player.ToString()))
        {
            playerIsInside = true;
            PhilaeManager.Instance.playerControllerRef.ChangeMainPlanet(rb);
        }
        if (other.CompareTag(GameData.Layers.Enemy.ToString()))
        {
            enemyAreInside = true;
            IAController iaController = other.gameObject.GetComponentInParent<IAController>();
            if (iaController)
            {
                iaController.ChangeMainPlanet(rb);
            }
            else
            {
                Debug.LogError("no ia controller ?");
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameData.Layers.Player.ToString()))
        {
            playerIsInside = false;
        }
    }
}
