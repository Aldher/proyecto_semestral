using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Slash : MonoBehaviour {

    [Tooltip("Esperar X segundos antes de destruir el objeto")]
    public float waitBeforeDestroy;

    [HideInInspector]
    public Vector2 mov;

    public float speed;

    void Update()
    {
        //La rafaga se movera en la direccion de las variables almacenadas
        transform.position += new Vector3(mov.x, mov.y, 0) * speed * Time.deltaTime;
    }

    IEnumerator OnTriggerEnter2D(Collider2D col)
    {
        //Variable que indica que si choca contra un objeto, se destruya 
        if (col.tag == "Object")
        {
            yield return new WaitForSeconds(waitBeforeDestroy);

            Destroy(gameObject);
        }

        //variable que indica que si choca contra un enemigo, le reste vida
        else if (col.tag != "Player" && col.tag != "Attack")
        {
            if (col.tag == "Enemy") col.SendMessage("Attacked");
            Debug.Log("Atacado");
            Destroy(gameObject);
        }
    }
}
