/* Copyright (C) GraphicDNA - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Iñaki Ayucar <iayucar@simax.es>, September 2016
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEssentials.Extensions;

[ExecuteInEditMode]
public class SplineController : MonoBehaviour
{ 
    public enum eSpeedUnits
    {
        /// <summary>
        /// Meters per second
        /// </summary>
        MetersPerSecond,
        /// <summary>
        /// Kilometers per hour
        /// </summary>
        KilometersPerHour,
        /// <summary>
        /// Miles per hour
        /// </summary>
        MilesPerHour,
        /// <summary>
        /// Curve Percentage (t) increment per second
        /// </summary>
        PercentagePerSecond,
    }
    public enum eLoopMode
    {
        None,
        AutoRewind,
        PingPong
    }
    public enum eMilestoneMode
    {
        ControlPoints,
        InterpolatedPoints,
    }
    public enum eOrientationMode
    {
        None,
        FollowSpline,
        FollowTarget,
    }

    public Spline Spline;
    /// <summary>
    /// Location on the position corresponding to a particular curve percent, where 0 is the origin of the spline, and 1 is the last point
    /// </summary>
    [Range(0, 1)]
    public float CurvePercent;
    [Range(0, 1)]
    public float DampingTime;

    [Space(10)]
    public bool AffectPosition;
    public Vector3 PositionOffset;

    [Space(10)]
    public Transform MeshToRotate;
    public Transform TargetToLook;
    public float speedRotation = 1f;
    public Vector3 AdditionalRotation;
    public Vector3 AdditionalRotationMesh;

    [Space(10)]
    public bool AutomaticWalking;
    public bool UpdateInUnityEditor;
    public eLoopMode LoopMode = eLoopMode.None;
    public eSpeedUnits SpeedUnits = eSpeedUnits.MetersPerSecond;
    public float Speed;
    private float TDelta = 0f;
    private Vector3 mCurrentAxisY;
    private Vector3 mCurrentAxisZ;
    private Vector3 mCurrentTarget;

    [Space(10)]
    public eMilestoneMode MilestonesBasedOn = eMilestoneMode.ControlPoints;
    public int CurrentMileStone;

    public UnityEngine.Events.UnityEvent MilestoneReached;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mps"></param>
    /// <returns></returns>
    private float Mps2Tps(float mps)
    {
        if (!Spline.IsNull() && Spline.TotalLength > 0)
            return mps / Spline.TotalLength;
        else return 0;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float GetTPerSeconds()
    {
        switch (SpeedUnits)
        {
            case eSpeedUnits.PercentagePerSecond:
                return Speed;
            case eSpeedUnits.MetersPerSecond:
                return Mps2Tps(Speed);                    
            case eSpeedUnits.KilometersPerHour:
                return Mps2Tps(Speed / 3.6f);
            case eSpeedUnits.MilesPerHour:
                return Mps2Tps(Speed * 0.44704f);
            default:
                return 0;
        }
    }

    public void ChangeSpeed(float _speed)
    {
        Speed = _speed;
        Init();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        Init();
        
    }

    public void Init()
    {
        TDelta = GetTPerSeconds();
    }
//    /// <summary>
//    /// 
//    /// </summary>
//    private void FixedUpdate()
//    {
//#if (!UNITY_EDITOR)
//        // If we are in play mode, use the normal damping time. If we are in the editor, set it to zero to reflect the changes immediately
//        float dampingTime = 0;
//        if (Application.isPlaying)
//            dampingTime = this.DampingTime;

//        InternalUpdate(Time.fixedDeltaTime, dampingTime);
//#endif
//    }
    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (!Application.isPlaying)
            CustomUpdate();
    }

    public void CustomUpdate()
    {
        // If we are in play mode, use the normal damping time. If we are in the editor, set it to zero to reflect the changes immediately
        float dampingTime = 0;
        if (Application.isPlaying)
            dampingTime = this.DampingTime;

        InternalUpdate(Time.deltaTime, dampingTime);
    }


    public Vector3 GetWorldPosByPercent(float percent, ref Vector3 axisX, ref Vector3 axisY, ref Vector3 axisZ)
    {
        Vector3 pos;
        int idx;
        Spline.GetWorldPositionFromT(percent, PositionOffset, out idx, out pos, out axisX, out axisY, out axisZ);
        return (pos);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deltaTime"></param>
    private void InternalUpdate(float deltaTime, float dampingTime)
    {
        if (Spline.IsNull())
            return;
        if (!AffectPosition)
            return;

        // Automatic update of t
        if (AutomaticWalking)
        {
            bool update = true;
#if (UNITY_EDITOR)
            update = Application.isPlaying || UpdateInUnityEditor;
#endif
            if(update)
                CurvePercent += TDelta * deltaTime;
        }

        // Loop
        switch(LoopMode)
        {
            case eLoopMode.AutoRewind:
                if (TDelta > 0 && CurvePercent >= 1)
                {
                    CurvePercent = 0;
                    TDelta = GetTPerSeconds();
                }
                else if (TDelta < 0 && CurvePercent <= 0)
                {
                    CurvePercent = 1;
                    TDelta = GetTPerSeconds();
                }
                break;
            case eLoopMode.PingPong:
                if (CurvePercent >= 1)
                    TDelta = -GetTPerSeconds();
                else if (CurvePercent <= 0)
                    TDelta = GetTPerSeconds();
                break;
            default:
#if (UNITY_EDITOR)
                //Spline.Refresh();
                TDelta = GetTPerSeconds();
#endif
                break;

        }
       
        // Update parameters
        Vector3 pos;
        Vector3 axisX, axisY, axisZ;
        int idx;
        Spline.GetWorldPositionFromT(CurvePercent, PositionOffset, out idx, out pos, out axisX, out axisY, out axisZ);

        // Choose milestone idx based on type of milestones
        int mileStoneIdx = idx;
        switch (MilestonesBasedOn)
        {
            case eMilestoneMode.InterpolatedPoints:
                mileStoneIdx = idx;
                break;
            case eMilestoneMode.ControlPoints:
                if (Spline.FinalPoints[idx].ControlPointIdx.HasValue)
                    mileStoneIdx = Spline.FinalPoints[idx].ControlPointIdx.Value;
                break;
        }
        if (mileStoneIdx != CurrentMileStone)
        {
            CurrentMileStone = mileStoneIdx;
            MilestoneReached.Invoke();
        }



        // Damp parameters
        Vector3 v = Vector3.zero;
        if (dampingTime <= 0)
        {
            mCurrentAxisY = axisY;
            mCurrentAxisZ = axisZ;
            if(!TargetToLook.IsNull())
                mCurrentTarget = TargetToLook.transform.position;
        }
        else
        {
            mCurrentAxisY = Vector3.SmoothDamp(mCurrentAxisY, axisY, ref v, dampingTime);
            mCurrentAxisZ = Vector3.SmoothDamp(mCurrentAxisZ, axisZ, ref v, dampingTime);
            if (!TargetToLook.IsNull())
                mCurrentTarget = Vector3.SmoothDamp(mCurrentTarget, TargetToLook.transform.position, ref v, dampingTime);
        }

        if (AffectPosition)
        {
            if (dampingTime <= 0)
                this.transform.position = pos;
            else
            {
                Vector3 vel = Vector3.zero;
                transform.position = Vector3.SmoothDamp(transform.position, pos, ref vel, dampingTime);
            }
        }
    
        Quaternion desiredRotation = Quaternion.LookRotation(mCurrentAxisZ, mCurrentAxisY) * Quaternion.Euler(AdditionalRotation);
        Quaternion desiredMeshRotation = this.transform.localRotation * Quaternion.Euler(AdditionalRotationMesh);
        if (TargetToLook)
            desiredMeshRotation = Quaternion.LookRotation((mCurrentTarget - this.transform.position).normalized, mCurrentAxisY) * Quaternion.Euler(AdditionalRotationMesh);

         
        if (dampingTime <= 0)
        {
            //Debug.Log("here");
            this.transform.rotation = desiredRotation;
            if (MeshToRotate)
                MeshToRotate.rotation = desiredMeshRotation;
        }            
        else
        {
            //Debug.Log("or");
            float rotationSpeedDegS = (Mathf.Clamp01(1f - dampingTime) * 99f) * speedRotation * (5 - ExtMathf.Remap(Speed, 0, 30, 0, 4));
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, deltaTime * rotationSpeedDegS);
            if (MeshToRotate)
            {
                MeshToRotate.rotation = ExtRotation.SmoothTurretLookRotation(desiredMeshRotation * Vector3.forward, Vector3.up, MeshToRotate.rotation, rotationSpeedDegS);
                //MeshToRotate.rotation = Quaternion.RotateTowards(MeshToRotate.rotation, desiredMeshRotation, deltaTime * rotationSpeedDegS);
            }
                
        }

    }
}