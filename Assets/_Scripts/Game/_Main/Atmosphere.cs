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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameData.Layers.Player.ToString()))
        {
            playerIsInside = true;
            PhilaeManager.Instance.playerControllerRef.ChangeMainPlanet(rb);
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
