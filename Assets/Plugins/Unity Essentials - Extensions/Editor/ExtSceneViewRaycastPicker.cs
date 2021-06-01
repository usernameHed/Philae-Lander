using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UnityEssentials.Extensions.Editor
{
    /// <summary>
    /// scene view calculation
    /// </summary>
    public static class ExtSceneViewRaycastPicker
    {
        public class HitSceneView
        {
            public GameObject objHit;
            public Vector3 pointHit;
            public Vector3 normal;
            public Ray Ray;
            public RaycastHit Hit;
            public RaycastHit2D Hit2d;
        }

        /// <summary>
        /// Raycast from mouse Position in scene view and save object hit
        /// </summary>
        /// <param name="newHit">return the info of the hit</param>
        /// <param name="setNullIfNot">if true, set back the last hitObject to null</param>
        public static HitSceneView SetCurrentOverObject(HitSceneView newHit, bool setNullIfNot = true)
        {
            if (Event.current == null)
            {
                return (newHit);
            }
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            {
                return (newHit);
            }

            RaycastHit _saveRaycastHit;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);
            newHit.Ray = worldRay;
            //first a test only for point
            if (Physics.Raycast(worldRay, out _saveRaycastHit, Mathf.Infinity))
            {
                if (_saveRaycastHit.collider.gameObject != null)
                {
                    newHit.objHit = _saveRaycastHit.collider.gameObject;
                    newHit.pointHit = _saveRaycastHit.point;
                    newHit.normal = _saveRaycastHit.normal;
                    newHit.Hit = _saveRaycastHit;
                }
            }
            else
            {
                RaycastHit2D raycast2d = Physics2D.Raycast(worldRay.GetPoint(0), worldRay.direction);
                if (raycast2d.collider != null)
                {
                    newHit.objHit = raycast2d.collider.gameObject;
                    newHit.pointHit = raycast2d.point;
                    newHit.normal = raycast2d.normal;
                    newHit.Hit2d = raycast2d;
                }
                else
                {

                    if (setNullIfNot)
                    {
                        newHit.objHit = null;
                        newHit.pointHit = Vector3.zero;
                    }
                }
            }
            return (newHit);
        }
    }
}