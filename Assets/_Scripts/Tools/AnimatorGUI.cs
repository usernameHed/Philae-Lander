using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatorGUI : MonoBehaviour
{
    public Animator animator;

    private void OnEnable()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        animator.Play(0);
    }
}
