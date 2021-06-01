using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// Each Chunk as one TriggerActionChunk reference, this script
    /// call Enter/Stay/Exit events
    /// </summary>
    public class TriggerActionChunk : MonoBehaviour
    {
        public delegate void TriggerActionChunks(ChunkLinker.Chunk chunk);
        public event TriggerActionChunks OnTriggerEnter;
        public event TriggerActionChunks OnTriggerStay;
        public event TriggerActionChunks OnTriggerExit;

        public void TriggerEnter(ChunkLinker.Chunk chunk)
        {
            Debug.Log("[ENTER] " + chunk.Description);
            OnTriggerEnter?.Invoke(chunk);
        }

        public void TriggerStay(ChunkLinker.Chunk chunk)
        {
            //Debug.Log("[STAY] " + chunk.Description);
            OnTriggerStay?.Invoke(chunk);
        }

        public void TriggerExit(ChunkLinker.Chunk chunk)
        {
            Debug.Log("[EXIT] " + chunk.Description);
            OnTriggerExit?.Invoke(chunk);
        }
    }
}