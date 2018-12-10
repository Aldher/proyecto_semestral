using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float speed = 4f;
    public GameObject initialMap;
    public GameObject slashPrefab;
    public int vidaJugador = 100;
    public static float bandera = 1;

    [Header("Etiquetas Resultado")]
    public Text txtVida;

    bool movePrevent;

    Aura aura; //metodo 

    public GameObject panel;
    Animator anim;
    Rigidbody2D rb2d;
    Vector2 mov;  // Ahora es visible entre los métodos

    CircleCollider2D attackCollider;

    void Awake()
    {
        // Comprobamos que haya un mapa inicial establecido 
        Assert.IsNotNull(initialMap);
        Assert.IsNotNull(slashPrefab);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        SetLabelText();

        // Recuperamos el collider de ataque del primer hijo
        attackCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();

        // Lo desactivamos desde el principio, luego
        attackCollider.enabled = false;

        // Establecemos los limites iniciales al primer mapa (o el que toque)
        Camera.main.GetComponent<MainCamera>().SetBound(initialMap);

        aura = transform.GetChild(1).GetComponent<Aura>();
    }

    void Update()
    {

        Debug.Log("Bandera" + bandera);

        // Detectamos el movimiento
        Movements();

        // Procesamos las animaciones
        Animations();

        // Ataque con espada
        SwordAttack();

        // Ataque con rayo maestro
        SlashAttack();

        // Prevenir movimiento
        PreventMovement();


    }

    private const string labelText = "{0} "; //Para concatenar

    void SetLabelText()
    {
        //Convertir de flotante a string
        txtVida.text = string.Format(labelText, vidaJugador);
       

    }

    void Movements() {

        // Detectamos el movimiento en un vector 2D
        mov = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

    }

    void Animations() {

        // Establecemos las animaciones
        if (mov != Vector2.zero)
        {
            anim.SetFloat("movX", mov.x);
            anim.SetFloat("movY", mov.y);
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }

    }

    void SwordAttack() {

        // Buscamos el estado actual mirando la información del animador
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool attacking = stateInfo.IsName("Player_Attack");

        // Detectamos el ataque, tiene prioridad por lo que va abajo del todo
        if (Input.GetKeyDown("space") && !attacking)
        {
            anim.SetTrigger("attacking");
        }

        // Vamos actualizando la posición de la colisión de ataque
        if (mov != Vector2.zero) attackCollider.offset = new Vector2(mov.x / 2, mov.y / 2);

        // Activamos el collider a la mitad de la animación de ataque
        if (attacking)
        {
            float playbackTime = stateInfo.normalizedTime;
            //Debug.Log(playbackTime);
            if (playbackTime > 0.55 && playbackTime < 0.85)
            {

                attackCollider.enabled = true;

                //Debug.Log("ataque habilitado");
            }
            else
            {

                attackCollider.enabled = false;
                //Debug.Log("ataque deshabilitado");
            }

        }

    }

    void SlashAttack()
    {
        // Buscamos el estado actual mirando la información del animador
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool loading = stateInfo.IsName("Player_Slash");

        // Ataque a distancia
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            anim.SetTrigger("loading");
            aura.AuraStart();
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            anim.SetTrigger("attacking");
            if (aura.IsLoaded())
            {
                // Para que se mueva desde el principio tenemos que asignar un
                // valor inicial al movX o movY en el edtitor distinto a cero
                float angle = Mathf.Atan2(
                    anim.GetFloat("movY"),
                    anim.GetFloat("movX")
                ) * Mathf.Rad2Deg;
                //Creamos la instancia del Slash
                GameObject slashObj = Instantiate(
                    slashPrefab, transform.position,
                    Quaternion.AngleAxis(angle, Vector3.forward)
                );
                //Otorgamos movimiento inicial
                Slash slash = slashObj.GetComponent<Slash>();
                slash.mov.x = anim.GetFloat("movX");
                slash.mov.y = anim.GetFloat("movY");
            }
            aura.AuraStop();

            StartCoroutine(EnableMovementAfter(0.4f));
        }

        // Prevenimos el movimiento mientras cargamos
        if (loading)
        {
            movePrevent = true;
        }

    }

        void PreventMovement()
    {
        if (movePrevent)
        {
            mov = Vector2.zero;
        }
    }

    IEnumerator EnableMovementAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        movePrevent = false;
    }

    void FixedUpdate()
    {
        // Nos movemos en el fixed por las físicas
        rb2d.MovePosition(rb2d.position + mov * speed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bala")
        {
            Debug.Log("putazo");
            vidaJugador -= Rock.dano;
            SetLabelText();
            if (vidaJugador <= 0)
            {

                Destroy(gameObject);
                panel.SetActive(true);
            }

        }
    }
}
