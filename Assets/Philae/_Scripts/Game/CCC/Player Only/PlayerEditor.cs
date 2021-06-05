
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerEditor : MonoBehaviour
{
    [Tooltip(""), SerializeField]
    public PlayerInput playerInput;
    [Tooltip(""), SerializeField]
    public Rigidbody rb;
}
