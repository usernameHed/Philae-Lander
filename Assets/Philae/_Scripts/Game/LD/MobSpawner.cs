
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.time;

public class MobSpawner : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [Tooltip("ref script")]
    public IsOnCamera isOnCamera;
    [Tooltip("ref script")]
    public float timeToSpawn = 4f;
    [Tooltip("ref script")]
    public float timeToAddRandom = 15f;
    [Tooltip("ref script")]
    public float minDist = 100f;

    [Tooltip("ref script")]
    public Transform posSpawn;

    private FrequencyCoolDown timeSpawn = new FrequencyCoolDown();

    private void Start()
    {
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
        if (timeSpawn.IsNotRunning() && !isOnCamera.isOnScreen && !IsCloseToPlayer())
        {
            Spawn();
        }
    }
}
