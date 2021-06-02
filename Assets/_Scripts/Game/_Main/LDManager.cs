
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

[ExecuteInEditMode]
public class LDManager : MonoBehaviour
{
    [Tooltip("text debug to display"), SerializeField]
    private PhilaeManager philaeManager = default;
    
    [Tooltip("text debug to display"), ReadOnly]
    public List<GravityAttractorLD> allGravityAttractorLd = new List<GravityAttractorLD>();
    [Tooltip("text debug to display"), ReadOnly]
    public List<SetGravityRotation> allSetGravityOrientation = new List<SetGravityRotation>();


    
    public void FillList(bool calculateAfter)
    {
        philaeManager.needRecalculate = false;
        //Debug.Log("<color=cyan>fill all gravity LD</color>");

        object[] allGA = Resources.FindObjectsOfTypeAll(typeof(GravityAttractorLD));
        allGravityAttractorLd.Clear();
        for (int i = 0; i < allGA.Length; i++)
        {
            GravityAttractorLD gld = allGA[i] as GravityAttractorLD;
            if (gld.gameObject.activeInHierarchy)
                allGravityAttractorLd.Add(gld);
        }

        object[] allSetGR = Resources.FindObjectsOfTypeAll(typeof(SetGravityRotation));
        allSetGravityOrientation.Clear();
        for (int i = 0; i < allSetGR.Length; i++)
        {
            SetGravityRotation setGR = allSetGR[i] as SetGravityRotation;
            setGR.philaeManager = philaeManager;

            if (setGR.gameObject.activeInHierarchy)
                allSetGravityOrientation.Add(setGR);
        }

        if (calculateAfter)
            CalculateFromList();
    }

    private void CalculateFromList()
    {
        //Debug.Log("<color=red>Setup all individual GA (precalculate faces)</color>"); //SetupArrayPoints
        //Debug.Log("Setup all SetGravityOrientation"); //SetupArrayPoints
        
        for (int i = 0; i < allGravityAttractorLd.Count; i++)
        {
            allGravityAttractorLd[i].SetupArrayPoints();
        }
        
        for (int i = 0; i < allSetGravityOrientation.Count; i++)
        {
            allSetGravityOrientation[i].SetGravityRotate();
        }
    }
}
