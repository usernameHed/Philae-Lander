using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerEditor : MonoBehaviour
{
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    public PlayerInput playerInput;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    public Rigidbody rb;
}
