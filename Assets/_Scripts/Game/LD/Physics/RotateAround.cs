using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// RotateAround Description
/// </summary>
public class RotateAround : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("directionSatelite"), SerializeField]
    private Vector3 direction = Vector3.zero;
    [FoldoutGroup("GamePlay"), Tooltip("planette de base"), SerializeField]
    private float speed = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("planette de base"), SerializeField]
    private Transform planet = null;


    private void Update()
    {
        transform.RotateAround(planet.position, direction, speed * Time.deltaTime);
    }

}
