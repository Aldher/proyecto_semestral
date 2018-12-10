using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FixDepth : MonoBehaviour {


    public bool fixEveryFrame;
    SpriteRenderer spr;

    void Start()
    {
        //Metodo para actualizar la profunndidad del jugador dependiendo de su posicion en el mapa.
        spr = GetComponent<SpriteRenderer>();
        spr.sortingLayerName = "Player";
        
        spr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void Update()
    {
        //Metodo para actualizar la profunndidad del jugador en tiempo real dependiendo de su posicion en el mapa.
        if (fixEveryFrame)
        {
            spr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}
