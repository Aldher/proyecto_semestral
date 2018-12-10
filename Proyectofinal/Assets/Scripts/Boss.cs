using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    //Principio de variables logica difusa///////////////////////////////////////////////////////////////////////////////////////
     
    [Header("Grado de verdad")]
    //poner cada conjunto de manera individual
    public AnimationCurve graficaMoribundo;
    public AnimationCurve graficaDañado;
    public AnimationCurve graficaSaludable;

    //para guardar resultados al evaluar los estatus de la vida dentro de la grafica
    private float moribundoValor;
    private float dañadoValor;
    private float saludableValor;

    //public AnimationCurve Neutral;
    //public AnimationCurve Bueno;
    //public AnimationCurve Malo;

    //variable para evaluar graficas
    float healthInput = 100;
    

    float enemigoTiempo;




    //Final variables logica difusa//////////////////////////////////////////////////////////////////////////////////////////////


    ///----- Variables relacionadas con el ataque y defensa del boss
    [Tooltip("Prefab de la roca que se disparará")]
    public GameObject rockPrefab;
    [Tooltip("Prefab de los enemigos que se instansiaran")]
    public GameObject Enemy;
    public GameObject Enemy2;
    [Tooltip("Velocidad de ataque (segundos entre ataques)")]
    public float attackSpeed;
    bool attacking;
    ///----- Fin de Variables relacionadas con el ataque y defensa del boss


    // Variable para guardar al jugador
    GameObject player;

    // Variable para guardar la posición inicial
    Vector3 initialPosition, target;

    // Animador y cuerpo cinemático con la rotación en Z congelada
    Animator anim;
    Rigidbody2D rb2d;

    void Start()
    {

        // Recuperamos al jugador gracias al Tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Guardamos nuestra posición inicial
        initialPosition = transform.position;

        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        StartCoroutine(Attack(5));

    }

    void Update()
    {
       

        // Por defecto nuestro target siempre será nuestra posición inicial
        target = initialPosition;

        // Calculamos la distancia y dirección actual hasta el target
        float distance = Vector3.Distance(target, transform.position);
        Vector3 dir = (target - transform.position).normalized;

        target = initialPosition;

        // Comprobamos un Raycast del enemigo hasta el jugador
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            player.transform.position - transform.position,
            10,
            1 << LayerMask.NameToLayer("Default")
        // Poner el propio Enemy en una layer distinta a Default para evitar el raycast
        // También ponemos al objeto Attack y al Prefab Slash una Layer Attack 
        // Sino los detectará como entorno y se mueve atrás al hacer ataques
        );




    }


   IEnumerator Attack(float seconds)
    {


        int bandera = 0;
        
        while (true)
        {

            EvaluateHealth();
            if (saludableValor >= 0.5f)
            //if(healthInput > 80)
            {

                yield return new WaitForSeconds(5);
                
                enemigoTiempo = 4f;
                //ciclo para instanciar enemigos
                do
                {
                    StartCoroutine("SpawmEnemy");
                    bandera++;
                }
                while (bandera <= 5);
                Debug.Log("Estado 1");
              
            }
            else if (saludableValor < 0.5f && dañadoValor <= 0.5f && moribundoValor == 0)
            //else if(healthInput > 60 && healthInput < 80)
            {
                yield return new WaitForSeconds(5);
                enemigoTiempo = 2f;

                //ciclo para instanciar enemigos pero más rapido
                do
                {
                    StartCoroutine("SpawmEnemy");
                    bandera++;
                }
                while (bandera <= 13);
                Debug.Log("Estado 2");

            }
            else if (dañadoValor > 0.5f && moribundoValor == 0)
            //else if(healthInput > 40 && healthInput < 60)
            {
                
                yield return new WaitForSeconds(4f);

                //despierta al boss eniciando animacion
                anim.SetBool("walking", true);
                anim.Play("Enemy_Walk", -1, 0);

                //el enemigo empieza a atackar
                Instantiate(rockPrefab, transform.position, transform.rotation);
                anim.SetBool("walking", true);
                anim.Play("Enemy_Walk", -1, 0);
                Debug.Log("Estado 3");
            }

            else if (dañadoValor >= 0 && moribundoValor <= 0.5f)
            //else if(healthInput > 20 && healthInput < 40)
            {
                //lo mismo que el estado 3 pero más rapido
                yield return new WaitForSeconds(2f);
                Instantiate(rockPrefab, transform.position, transform.rotation);
                anim.SetBool("walking", true);
                anim.Play("Enemy_Walk", -1, 0);
                Debug.Log("Estado 4");
            }

            else if (moribundoValor > 0.5f)
            //else if(healthInput > 0 && healthInput < 20)
            {

                //estado para atacar e instanciar enemigos más rapido sin limite de oleadas.
                enemigoTiempo = 2f;
                StartCoroutine("SpawmEnemy");
                yield return new WaitForSeconds(0.5f);
                Instantiate(rockPrefab, transform.position, transform.rotation);
                anim.SetBool("walking", true);
                anim.Play("Enemy_Walk", -1, 0);
                Debug.Log("Estado 5");
            }

        }
    }

    IEnumerator SpawmEnemy()
    {

      
            yield return new WaitForSeconds(enemigoTiempo);
            Instantiate(Enemy);
            Instantiate(Enemy2);

    }

    public void Iniciar_C() {

        //Inicia cortina de ataque para el boss
        StartCoroutine(Attack(5));
    }


    public void EvaluateHealth()
    {
      
        //valor que tenga el input fiel
        float inputValue = (healthInput);


        //le vamos a guardar el resultado de la evaluacion dentro de la grafica
        moribundoValor = graficaMoribundo.Evaluate(inputValue);
        dañadoValor = graficaDañado.Evaluate(inputValue);
        saludableValor = graficaSaludable.Evaluate(inputValue);


    }

    public void Attacked()
    {
        //se resta vida al boss
        healthInput = healthInput - 10;

        //Si al boss no le queda más vida, se elimina el boss.
        if (healthInput <= 0)
        {
            NPC.reputation = NPC.reputation + 50;
            Destroy(gameObject);
        }
            
        Debug.Log("Atacado HP=" + healthInput);

        EvaluateHealth();
    }

    ///---  Dibujamos las vidas del enemigo en una barra 
    void OnGUI()
    {
        // Guardamos la posición del enemigo en el mundo respecto a la cámara
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);

        // Dibujamos el cuadrado debajo del enemigo con el texto
        GUI.Box(
            new Rect(
                pos.x - 20,                   // posición x de la barra
                Screen.height - pos.y + 60,   // posición y de la barra
                40,                           // anchura de la barra    
                24                            // altura de la barra  
            ), healthInput + "/" + 100               // texto de la barra
        );
    }


}
