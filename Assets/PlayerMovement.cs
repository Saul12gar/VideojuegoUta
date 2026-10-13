using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 4f;
    public float velocidadCorrer = 8f;
    public float velocidadActual;
    private CharacterController controller;

    [Header("Sistema de Salto y Gravedad")]
    public float fuerzaSalto = 5f;
    public float gravedad = -20f;
    private Vector3 velocidadVertical;
    private bool estaEnSuelo;
    private bool saltoSolicitado = false;
    private float temporizadorRuidoSalto = 0f;
    
    [Header("Buffer de Salto (Evita que falle el salto)")]
    public float bufferSaltoTiempo = 0.2f; // Tiempo que el juego "recuerda" que quisiste saltar
    private float contadorBufferSalto = 0f;

    [Header("Detección de Suelo")]
    public Transform comprobadorSuelo;
    public float radioSuelo = 0.3f;
    public LayerMask mascaraSuelo;

    [Header("Sistema de Estamina")]
    public float estaminaMaxima = 100f;
    public float estaminaActual;
    public float costoEstaminaPorSegundo = 25f;
    public float recuperacionEstamina = 20f;
    private bool estaCorriendo = false;

    [Header("Sistema de Ruido (Para el Villano)")]
    public float nivelDeRuido = 0f; 
    [Tooltip("Radio actual en metros al que el villano puede escuchar al jugador")]
    public float radioDeRuido = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        estaminaActual = estaminaMaxima;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ManejarGravedadYSaltos();
        ManejarMovimientoYEstamina();
        CalcularRuido();
    }

    void ManejarGravedadYSaltos()
    {
        // 1. Detección robusta del suelo
        bool enSueloController = controller.isGrounded;
        bool enSueloEsfera = false;
        if (comprobadorSuelo != null)
        {
            enSueloEsfera = Physics.CheckSphere(comprobadorSuelo.position, radioSuelo, mascaraSuelo);
        }
        estaEnSuelo = enSueloController || enSueloEsfera;

        if (estaEnSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f; 
        }

        // 2. Registrar la intención de salto (Buffer)
        if (Input.GetButtonDown("Jump"))
        {
            contadorBufferSalto = bufferSaltoTiempo;
        }
        else if (contadorBufferSalto > 0)
        {
            contadorBufferSalto -= Time.deltaTime;
        }

        // 3. Ejecutar el salto si hay memoria de buffer y estamos en el suelo (Sin saltos infinitos en el aire)
        if (contadorBufferSalto > 0 && estaEnSuelo)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
            saltoSolicitado = true;
            temporizadorRuidoSalto = 0.3f; 
            contadorBufferSalto = 0f; // Consumir el buffer para evitar dobles saltos
        }
        else
        {
            saltoSolicitado = false;
        }

        // 4. Aplicar gravedad constante
        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    void ManejarMovimientoYEstamina()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;

        bool quiereCorrer = Input.GetKey(KeyCode.LeftShift) && (moveX != 0 || moveZ != 0) && estaEnSuelo;

        if (quiereCorrer && estaminaActual > 0)
        {
            estaCorriendo = true;
            velocidadActual = velocidadCorrer;
            estaminaActual -= costoEstaminaPorSegundo * Time.deltaTime;

            if (estaminaActual <= 0)
            {
                estaminaActual = 0;
                estaCorriendo = false; 
            }
        }
        else
        {
            estaCorriendo = false;
            velocidadActual = velocidadCaminar;

            if (estaminaActual < estaminaMaxima)
            {
                estaminaActual += recuperacionEstamina * Time.deltaTime;
                if (estaminaActual > estaminaMaxima) estaminaActual = estaminaMaxima;
            }
        }

        controller.Move(moveDir * velocidadActual * Time.deltaTime);
    }

    void CalcularRuido()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool enMovimiento = (moveX != 0 || moveZ != 0);

        if (temporizadorRuidoSalto > 0)
        {
            temporizadorRuidoSalto -= Time.deltaTime;
        }

        if (temporizadorRuidoSalto > 0 || saltoSolicitado)
        {
            nivelDeRuido = 3f;
            radioDeRuido = 25f; 
        }
        else if (!enMovimiento)
        {
            nivelDeRuido = 0f;
            radioDeRuido = 0f; 
        }
        else if (estaCorriendo)
        {
            nivelDeRuido = 2f;
            radioDeRuido = 18f; 
        }
        else
        {
            nivelDeRuido = 1f;
            radioDeRuido = 5f; 
        }
    }
}