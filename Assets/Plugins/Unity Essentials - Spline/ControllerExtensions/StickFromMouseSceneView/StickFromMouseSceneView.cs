using System.Collections;
using UnityEngine;
using UnityEssentials.Spline.Controller;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.PropertyAttribute.NoNull;

namespace UnityEssentials.Spline.ControllerExtensions
{
    [ExecuteInEditMode]
    public class StickFromMouseSceneView : MonoBehaviour
    {
        [SerializeField, NoNull] private ControllerStick _controllerTarget;
        public SplineBase.PositionUnits PositionUnits { get { return (_controllerTarget.PositionUnits); } }
        public ControllerStick ControllerStick { get { return (_controllerTarget); } }
        public SplineBase Spline { get { return (_controllerTarget.SplineBase); } }


        [SerializeField] private ExtShortCut.ModifierKey _modifier1 = ExtShortCut.ModifierKey.CTRL;
        [SerializeField] private ExtShortCut.ModifierKey _modifier2 = ExtShortCut.ModifierKey.SHIFT;
        [SerializeField] private KeyCode _key = KeyCode.None;

        public bool IsTriggeringAction()
        {
            return (ExtShortCut.IsTriggeringShortCut2Modifier1Keys(Event.current, _modifier1, _modifier2, _key));
        }
    }
}