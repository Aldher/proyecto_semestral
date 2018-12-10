using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adasdasdfas : MonoBehaviour {
    MeshRenderer puto;
	// Use this for initialization
	void Start () {
      
    }
	
	// Update is called once per frame
	void Update () {
        if (NPC.reputation == 12)
        {
           puto.enabled  = false;
        }
    }
}
