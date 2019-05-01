using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CapsuleCharacterCollisionDetection;

[ExecuteInEditMode]
public class TestKbTree : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Collider _collider;
    public GameObject player;

    private ExtNearestPoint.InfoKdTree infoKdTree;

    private void OnEnable()
    {
        infoKdTree = ExtNearestPoint.InitKdTree(meshFilter);
    }

    private void Update()
    {
        //Vector3 nearestPos = ExtNearestPoint.NearestPointToMeshWithInitKdTree(player.transform.position, meshFilter, infoKdTree);
        //Vector3 nearestPos = Physics.ClosestPoint(player.transform.position, _collider, meshFilter.transform.position, meshFilter.transform.rotation);
        //ContactInfo contact = ExtCollider.ClosestPointOnSurface(_collider, player.transform.position, player.transform.position, 0.1f);
        //Vector3 nearestPos = contact.point;
        //ExtDrawGuizmos.DebugWireSphere(nearestPos, Color.red, 0.4f);
    }
}
