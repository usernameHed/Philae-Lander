using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handle for moving objects in list
/// </summary>
public class PositionHandleChilds : MonoBehaviour
{
    [Tooltip(""), SerializeField]
    public List<Transform> _allChildToMove = new List<Transform>();
}
