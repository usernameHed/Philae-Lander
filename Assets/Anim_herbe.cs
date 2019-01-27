using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_herbe : MonoBehaviour
{
   Animator anim;
    bool isBouge;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        isBouge = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Tags.Player.ToString()))
        {
            anim.SetBool ("isBouge", true);
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
