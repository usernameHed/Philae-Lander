using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.time
{
    [ExecuteInEditMode]
    public class ExtDrawEditorTimeScale : MonoBehaviour
    {
        private static List<CoolDownWrapperSphere> _coolDowns = new List<CoolDownWrapperSphere>();
        private static List<CoolDownWrapperLine> _coolDownsLine = new List<CoolDownWrapperLine>();


        /// <summary>
        /// wrapper of cooldowns sphere
        /// </summary>
        public class CoolDownWrapperSphere
        {
            public FrequencyCoolDown CoolDown = new FrequencyCoolDown();
            public Vector3 Position = Vector3.zero;
            public Color Color = Color.white;
            public float Radius = 1f;
            public float Time = 1f;

            public bool IsSameStats(Vector3 position, Color color, float radius, float time)
            {
                return (position == Position && Color == color && Radius == radius && Time == time);
            }

            public CoolDownWrapperSphere(Vector3 position, Color color, float radius, float time)
            {
                CoolDown = new FrequencyCoolDown();
                Position = position;
                Color = color;
                Radius = radius;
                Time = time;
                CoolDown.StartCoolDown(time, false, false, false);
            }
        }

        /// <summary>
        /// wrapper of cooldowns line
        /// </summary>
        public class CoolDownWrapperLine
        {
            public FrequencyCoolDown CoolDown = new FrequencyCoolDown();
            public Vector3 PositionA = Vector3.zero;
            public Vector3 PositionB = Vector3.zero;
            public Color Color = Color.white;
            public float Time = 1f;

            public bool IsSameStats(Vector3 positionA, Vector3 positionB, Color color, float time)
            {
                return (positionA == PositionA && positionB == PositionB && Color == color && Time == time);
            }

            public CoolDownWrapperLine(Vector3 positionA, Vector3 positionB, Color color, float time)
            {
                CoolDown = new FrequencyCoolDown();
                PositionA = positionA;
                PositionB = positionB;
                Color = color;
                Time = time;
                CoolDown.StartCoolDown(time, false, false, false);
            }
        }

        /// <summary>
        /// call this function to display a sphere in a given position, for a given time interval
        /// If you call twice thice function with the same arguments, reset tthe timer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <param name="time"></param>
        public static void DebugWireSphere(Vector3 position, Color color, float radius, float time)
        {
            CoolDownWrapperSphere newCoolDown = UpdateOrAddCoolDown(position, color, radius, time);
        }
        public static void DebugLine(Vector3 positionA, Vector3 positionB, Color color, float time)
        {
            CoolDownWrapperLine newCoolDown = UpdateOrAddCoolDownLine(positionA, positionB, color, time);
        }

        public static void DebugLine(Vector3 positionA, Vector3 direction, float size, Color color, float time)
        {
            Vector3 positionB = positionA + (direction * size);
            CoolDownWrapperLine newCoolDown = UpdateOrAddCoolDownLine(positionA, positionB, color, time);
        }

        /// <summary>
        /// call to instantanly remove all the current guizmo
        /// </summary>
        public static void ClearGuizmos()
        {
            _coolDowns.Clear();
            _coolDownsLine.Clear();
        }

        /// <summary>
        /// clear only point of a given color
        /// </summary>
        /// <param name="color"></param>
        public static void CLearColor(Color color)
        {
            for (int i = _coolDowns.Count - 1; i >= 0; i--)
            {
                if (_coolDowns[i].Color == color)
                {
                    _coolDowns.RemoveAt(i);
                }
            }
            for (int i = _coolDownsLine.Count - 1; i >= 0; i--)
            {
                if (_coolDownsLine[i].Color == color)
                {
                    _coolDownsLine.RemoveAt(i);
                }
            }
        }

        private void UpdateAllCoolDown()
        {
            for (int i = _coolDowns.Count - 1; i >= 0; i--)
            {
                if (_coolDowns[i].CoolDown.IsFinished())
                {
                    _coolDowns.RemoveAt(i);
                    continue;
                }

                DebugWireSphere(_coolDowns[i].Position, _coolDowns[i].Color, _coolDowns[i].Radius, 0f);
            }
            for (int i = _coolDownsLine.Count - 1; i >= 0; i--)
            {
                if (_coolDownsLine[i].CoolDown.IsFinished())
                {
                    _coolDownsLine.RemoveAt(i);
                    continue;
                }
                Debug.DrawLine(_coolDownsLine[i].PositionA, _coolDownsLine[i].PositionB, _coolDownsLine[i].Color, 0f);
            }
        }

        public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            float angle = 10.0f;

            Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
            Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
            Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

            Vector3 new_x;
            Vector3 new_y;
            Vector3 new_z;

            for (int i = 1; i < 37; i++)
            {

                new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
                new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
                new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

                Debug.DrawLine(x, new_x, color, duration, depthTest);
                Debug.DrawLine(y, new_y, color, duration, depthTest);
                Debug.DrawLine(z, new_z, color, duration, depthTest);

                x = new_x;
                y = new_y;
                z = new_z;
            }
        }

        private void Update()
        {
            UpdateAllCoolDown();
        }

        private static CoolDownWrapperSphere UpdateOrAddCoolDown(Vector3 position, Color color, float radius, float time)
        {
            for (int i = 0; i < _coolDowns.Count; i++)
            {
                if (_coolDowns[i].IsSameStats(position, color, radius, time))
                {
                    _coolDowns[i].CoolDown.StartCoolDown(time, false, false, false);
                    return (_coolDowns[i]);
                }
            }
            CoolDownWrapperSphere newWrapper = new CoolDownWrapperSphere(position, color, radius, time);
            _coolDowns.Add(newWrapper);
            return (newWrapper);
        }
        private static CoolDownWrapperLine UpdateOrAddCoolDownLine(Vector3 positionA, Vector3 positionB, Color color, float time)
        {
            for (int i = 0; i < _coolDownsLine.Count; i++)
            {
                if (_coolDownsLine[i].IsSameStats(positionA, positionB, color, time))
                {
                    _coolDownsLine[i].CoolDown.StartCoolDown(time, false, false, false);
                    return (_coolDownsLine[i]);
                }
            }
            CoolDownWrapperLine newWrapper = new CoolDownWrapperLine(positionA, positionB, color, time);
            _coolDownsLine.Add(newWrapper);
            return (newWrapper);
        }

    }
}