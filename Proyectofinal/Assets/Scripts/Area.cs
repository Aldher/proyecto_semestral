using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Area : MonoBehaviour {

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public IEnumerator ShowArea(string name)
    {
        //Efecto para que aparesca el nombre del escenario al cambiar de escena.
        anim.Play("Castillo_Show");
        transform.GetChild(0).GetComponent<Text>().text = name;
        transform.GetChild(1).GetComponent<Text>().text = name;
        yield return new WaitForSeconds(1f);
        anim.Play("Castillo_FadeOut");
    }
}
