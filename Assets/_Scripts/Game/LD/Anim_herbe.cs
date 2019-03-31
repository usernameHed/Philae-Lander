using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_herbe : MonoBehaviour
{
    [SerializeField]
    private Animator anim = null;

    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_bush;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Tags.Player.ToString())
            || other.gameObject.CompareTag(GameData.Tags.Enemy.ToString()))
        {
            anim.SetBool ("isBouge", true);
            SoundManager.Instance.PlaySound(SFX_bush);

            Invoke("UnableIsBouge", 0.2f);
        }
    }

    private void UnableIsBouge()
    {
        anim.SetBool("isBouge", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
