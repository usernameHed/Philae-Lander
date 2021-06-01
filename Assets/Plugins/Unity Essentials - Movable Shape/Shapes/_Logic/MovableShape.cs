using UnityEngine;
using UnityEngine.Events;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.MovableShape
{
    [ExecuteInEditMode]
    public abstract class MovableShape : MonoBehaviour
    {
        [SerializeField] private Color _color = new Color(1, 0.92f, 1f, 0.7f);
        [SerializeField] private bool _showZone = true;
        public bool ShowZone { get { return (_showZone); } set { _showZone = value; } }
        [SerializeField, ReadOnly] protected bool _isConstruct = false;

        private void Awake()
        {
            if (!_isConstruct)
            {
                Construct();
                _isConstruct = true;
            }
            else
            {
                Actualize();
            }
        }

        public abstract void Construct();
        public abstract void Actualize();

        private void Update()
        {
            if (transform.hasChanged)
            {
                Move(transform.position, transform.rotation, transform.lossyScale);
                transform.hasChanged = false;
            }
        }

#if UNITY_EDITOR
        public abstract void Draw();
#endif
        public abstract bool IsInsideShape(Vector3 position);
        public abstract bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint);


        public virtual void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale) { }

        protected Color GetColor()
        {
            return (_color);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_showZone || !gameObject.activeInHierarchy || !this.enabled)
            {
                return;
            }
            Draw();
        }

        private void OnValidate()
        {
            Actualize();
        }
#endif
    }
}