using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline.Controller;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.PointsOnSplineExtensions;
using UnityEssentials.Spline.PropertyAttribute.NoNull;

namespace UnityEssentials.Spline.ControllerExtensions.EventsTrigger
{
    /// <summary>
    /// this script will run though all chunks, and manage the OnTrigger Enter / Stay / Exit
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Events/Chunks")]
    public class ControllerEventsChunks : ControllerEvents
    {
        [SerializeField] private List<ChunkLinker.Chunk> _chunksInside = new List<ChunkLinker.Chunk>(10);
        
        private List<ChunkLinker> _allChunkLister = new List<ChunkLinker>(3);

        private void OnEnable()
        {
            _chunksInside.Clear();
            _allChunkLister.Clear();
            for (int i = 0; i < ListToCheck.Length; i++)
            {
                if (ListToCheck[i] == null)
                {
                    continue;
                }
                ChunkLinker[] chunks = ListToCheck[i].GetComponents<ChunkLinker>();
                _allChunkLister.Append(chunks);
            }
            CalculateInsideOutChunks();
        }

        private void Update()
        {
            CalculateInsideOutChunks();
        }

        private void CalculateInsideOutChunks()
        {
            for (int i = 0; i < _allChunkLister.Count; i++)
            {
                for (int k = 0; k < _allChunkLister[i].ChunkCount; k++)
                {
                    ChunkLinker.Chunk chunk = _allChunkLister[i].GetChunk(k);
                    bool isInside = chunk.IsControllerInsideChunk(_controller, _allChunkLister[i].Points);
                    bool isAlreadyInside = IsAlreadyInsideChunk(chunk);

                    if (!isAlreadyInside && isInside)
                    {
                        chunk.ActionChunk.TriggerEnter(chunk);
                        _chunksInside.Add(chunk);
                    }
                    else if (isAlreadyInside && isInside)
                    {
                        chunk.ActionChunk.TriggerStay(chunk);
                    }
                    else if (isAlreadyInside && !isInside)
                    {
                        chunk.ActionChunk.TriggerExit(chunk);
                        _chunksInside.Remove(chunk);
                    }
                }
            }
        }

        public bool IsAlreadyInsideChunk(ChunkLinker.Chunk chunk)
        {
            return (_chunksInside.Contains(chunk));
        }
    }
}
