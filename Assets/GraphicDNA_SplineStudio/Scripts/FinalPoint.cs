﻿/* Copyright (C) GraphicDNA - All Rights Reserved
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
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class FinalPoint 
{
    public Vector3 Position;
    public float Camber;
    public Vector3 SegmentAxisX;
    public Vector3 SegmentAxisY;
    public Vector3 SegmentAxisZ;    
    public float NextSegmentLen;
    public float DistanceFromOrigin;
    public int? ControlPointIdx;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="localOffset"></param>
    /// <returns></returns>
    public static Vector3 GetLocalOffset(Vector3 axisX, Vector3 axisY, Vector3 axisZ, Vector3 localOffset)
    {
        return axisX * localOffset.x +
               axisY * localOffset.y +
               axisZ * localOffset.z;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pOffset"></param>
    /// <returns></returns>
    public Vector3 GetOffsetPoint(Vector3 pOffset)
    {
        return this.Position + GetLocalOffset(this.SegmentAxisX, this.SegmentAxisY, this.SegmentAxisZ, pOffset);
    }
}