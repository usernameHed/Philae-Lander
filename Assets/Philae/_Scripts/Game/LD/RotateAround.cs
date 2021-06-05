using UnityEngine;

namespace Philae.LD
{
    /// <summary>
    /// RotateAround Description
    /// </summary>
    public class RotateAround : MonoBehaviour
    {
        [Tooltip("directionSatelite"), SerializeField]
        private Vector3 direction = Vector3.zero;
        [Tooltip("planette de base"), SerializeField]
        private float speed = 5f;
        [Tooltip("planette de base"), SerializeField]
        private Transform planet = null;


        private void Update()
        {
            transform.RotateAround(planet.position, direction, speed * Time.deltaTime);
        }

    }
}