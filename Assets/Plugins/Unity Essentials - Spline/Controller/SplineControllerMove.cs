using System;
using UnityEngine;
using UnityEssentials.Spline.Extensions;

namespace UnityEssentials.Spline.Controller
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Controller/Move")]
    public class SplineControllerMove : ControllerStick
    {
        [Header("Movements Settings"), Space(30)]
        [SerializeField] protected float _speedMove = 1f; public float SpeedMove { get { return (_speedMove); } }
        [SerializeField] protected bool _canMove = true;
#if UNITY_EDITOR
        [SerializeField] protected bool _moveInEditor = false; public bool MoveInEditor { get { return (_moveInEditor); } }
#endif

        protected override void FixedUpdate()
        {
            AttemptToMove();
        }

        public virtual void AttemptToMove()
        {
            if (_toMove == null || _spline == null)
            {
                this.enabled = false;
                return;
            }
            if (_canMove)
            {
                base.Move(_speedMove * Time.fixedDeltaTime);
            }
            base.Stick();
        }
    }
}
