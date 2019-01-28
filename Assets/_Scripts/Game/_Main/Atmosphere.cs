using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("Ratio scale Atmosphere"), SerializeField]
    private GameObject prefabMetric;

    [FoldoutGroup("Object"), Tooltip("planette qui attire"), SerializeField]
    private Rigidbody rbPlanetParent;
    [FoldoutGroup("Object"), Tooltip("planette qui attire"), SerializeField]
    private Transform atmosphere;

    [FoldoutGroup("Debug"), Tooltip("planette qui attire"), SerializeField]
    private bool playerIsInside = false;
    [FoldoutGroup("Debug"), Tooltip("planette qui attire"), SerializeField]
    private bool enemyAreInside = false;

    [Button]
    private void SetMetric()
    {
        float radius = prefabMetric.GetComponent<SphereCollider>().radius;
        //get real scale ??
        float newLocalScale = ((radius / rbPlanetParent.transform.localScale.x) * 2) + 1;
        atmosphere.transform.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameData.Layers.Player.ToString()))
        {
            playerIsInside = true;
            PhilaeManager.Instance.playerControllerRef.ChangeMainPlanet(rbPlanetParent);
        }
        if (other.CompareTag(GameData.Layers.Enemy.ToString()))
        {
            enemyAreInside = true;
            IAController iaController = other.gameObject.GetComponentInParent<IAController>();
            if (iaController)
            {
                iaController.ChangeMainPlanet(rbPlanetParent);
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
