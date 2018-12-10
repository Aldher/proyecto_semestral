using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    [Header("Grado de verdad")]
    //poner cada conjunto de manera individual
    public AnimationCurve Neutral;
    public AnimationCurve Bueno;
    public AnimationCurve Malo;

    public float visionRadius;

    public BoxCollider2D llave;
    private float n_Neutral;
    private float n_Bueno;
    private float n_Malo;

    //variable para evaluar graficas

    [SerializeField]
    public static float reputation = 0;

    Rigidbody2D rb2d;

    // Variable para guardar al jugador
    GameObject player;

    Vector3 initialPosition, target;

    // Use this for initialization
    void Start () {


        // Recuperamos al jugador gracias al Tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Guardamos nuestra posición inicial
        initialPosition = transform.position;

        rb2d = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {

        // Por defecto nuestro target siempre será nuestra posición inicial
        target = initialPosition;

        // Comprobamos un Raycast del enemigo hasta el jugador
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            player.transform.position - transform.position,
            10,
            1 << LayerMask.NameToLayer("Default"));

        // Aquí podemos debugear el Raycast
        Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
        Debug.DrawRay(transform.position, forward, Color.red);

        // Si el Raycast encuentra al jugador lo ponemos de target
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player")
            {
                target = player.transform.position;
            }
        }

        // Calculamos la distancia y dirección actual hasta el target
        float distance = Vector3.Distance(target, transform.position);
        Vector3 dir = (target - transform.position).normalized;

        // Si es el NPC podemos hablar con el dentro de su rango de vision
        if ( distance < visionRadius)
        {
           // Debug.Log("CHECK");

            if (Input.GetKeyDown(KeyCode.Q))
            {
                EvaluateReputation();
                //estado 1
                if(n_Malo <= 1 && n_Malo > 0.6f)
                {
                    Debug.Log("Gracias por derrotar a los enemigos");
                }

                //estado 2
                else if (n_Malo <= 0.52f && n_Neutral > 0.88f)
                {
                    Debug.Log("Que paso ahi amigito? Como que el miniboss te derroto. Para eso me gustabas");
                }

                //estado 3
                else if (n_Neutral <= 1 && n_Bueno >= 0 && n_Bueno < 0.04f)
                {
                    Debug.Log("Ahhhhh Prrruuuuuuuu!!!!! Ahora te falta solo derrotar al boss final");
                }

                //estado 4
                else if (n_Neutral <= 0.48f && n_Bueno >= 0.04f && n_Bueno < 0.64f )
                {
                    Debug.Log("Ya casi compa, derrota al boss y aqui te estaran esperando una helodias");
                }

                //estado 5
                else if (n_Bueno >= 0.64f)
                {
                    Debug.Log("AHHHHHHHHHHHHHHHHHHHH PPRRRRRRRRUUUUUUUUUUUU MATASTE AL BOSSSS");
                }
            }
        }

       

        // Y un debug optativo con una línea hasta el target
        Debug.DrawLine(transform.position, target, Color.green);

        Debug.Log("Reputacion" + reputation);

        if (reputation >= 6) llave.enabled = false;
    }

    // Podemos dibujar el radio de visión y ataque sobre la escena dibujando una esfera
    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
       // Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

    public void EvaluateReputation()
    {

        //valor que tenga el input fiel
        float inputValue = (reputation);


        //le vamos a guardar el resultado de la evaluacion dentro de la grafica
        n_Neutral = Neutral.Evaluate(inputValue);
        n_Bueno = Bueno.Evaluate(inputValue);
        n_Malo = Malo.Evaluate(inputValue);


    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        
        
        if (obj.gameObject.tag == ("Player"))
        {
          //  Debug.Log("CHECK");
            
            // //  if (Input.GetKeyDown("space"))
            //   {
             //      Debug.Log("Putos todos");
           //    }
            
           

            
        }
    }

    
}
