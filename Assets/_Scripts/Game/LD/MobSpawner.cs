﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public IsOnCamera isOnCamera;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public float timeToSpawn = 4f;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public float timeToAddRandom = 15f;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public float minDist = 100f;

    [FoldoutGroup("Object"), Tooltip("ref script")]
    public Transform posSpawn;

    private FrequencyCoolDown timeSpawn = new FrequencyCoolDown();
    private PlayerController playerController;

    private void Start()
    {
        playerController = PhilaeManager.Instance.playerControllerRef;
        timeSpawn.StartCoolDown(timeToSpawn + ExtRandom.GetRandomNumber(0.0f, timeToAddRandom));
    }

    private void Spawn()
    {
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.IA, posSpawn.position, posSpawn.rotation, transform);
        timeSpawn.StartCoolDown(timeToSpawn + ExtRandom.GetRandomNumber(0.0f, timeToAddRandom));
    }

    public bool IsCloseToPlayer()
    {
        float dist = Vector3.SqrMagnitude(transform.position - playerController.rb.transform.position);
        if (dist < minDist)
        {
            return (true);
        }
        return (false);
    }

    private void Update()
    {
        if (timeSpawn.IsReady() && !isOnCamera.isOnScreen && !IsCloseToPlayer())
            Spawn();
    }
}
