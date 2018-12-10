using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MainCamera : MonoBehaviour {

    public float smoothTime = 3f;


    Transform target;
    float tLX, tLY, bRX, bRY;

    
    Vector2 velocity; // necesario para el suavizado de cámara

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	
	void Update () {

        //v1. Sin limites 01-04-18.
        /*transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z);*/

        //v2 Con limites 30-04-18.
        /*transform.position = new Vector3(
            Mathf.Clamp(target.position.x, tLX, bRX),
            Mathf.Clamp(target.position.y, bRY, tLY),
            transform.position.z);*/
        /* v3 con límites y suavizado 01-06-18
        NOTA: REQUIERE LLAMAR FAST MOVE AL CAMBIAR DE MAPA */

        float posX = Mathf.Round(
            Mathf.SmoothDamp(transform.position.x,
                target.position.x, ref velocity.x, smoothTime) * 100) / 100;
        float posY = Mathf.Round(
         Mathf.SmoothDamp(transform.position.y,
             target.position.y, ref velocity.y, smoothTime) * 100) / 100;

        transform.position = new Vector3(
            Mathf.Clamp(posX, tLX, bRX),
            Mathf.Clamp(posY, bRY, tLY),
            transform.position.z
        );






    }

    //Metodo para delimitar el movimiento de la camara de acuerdo al escenario
    public void SetBound(GameObject map)
    {
        Tiled2Unity.TiledMap config = map.GetComponent<Tiled2Unity.TiledMap>();
        string nombre = config.name;
        float cameraSize = Camera.main.orthographicSize;

        if(nombre == "Castillo")
        {
            tLX = map.transform.position.x + cameraSize + 2;
            tLY = map.transform.position.y - cameraSize;
            bRX = map.transform.position.x + config.NumTilesWide + 1;
            bRY = map.transform.position.y - config.NumTilesHigh - 3;

        }

        else if (nombre == "Pasadiso a la Muerte")
        {
            tLX = 4 ;
            tLY = 86;
            bRX = 4;
            bRY = 26;

        }

        else if (nombre == "Calabozo")
        {
            tLX = 4.5f;
            tLY = 120.51f;
            bRX = 4.5f;
            bRY = 109;

          

        }

        FastMove();

    }

    //metodo para que exista un retraso al moverse la camara
    public void FastMove()
    {
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }
}
