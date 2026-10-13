using UnityEngine;
using UnityEngine.UI; // Necesario si quieres conectar una barra UI más adelante

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 4f;
    public float velocidadCorrer = 8f;
    public float velocidadActual;
    private CharacterController controller;

    [Header("Sistema de Estamina")]
    public float estaminaMaxima = 100f;
    public float estaminaActual;
    public float costoEstaminaPorSegundo = 25f;
    public float recuperacionEstamina = 20f;
    private bool estaCorriendo = false;

    [Header("Sistema de Ruido (Para el Villano)")]
    // 0 = Silencioso, 1 = Caminando, 2 = Corriendo
    public float nivelDeRuido = 0f; 
    [Tooltip("Radio actual en metros al que el villano puede escuchar al jugador")]
    public float radioDeRuido = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            // Si no tiene Character Controller, lo añade automáticamente
            controller = gameObject.AddComponent<CharacterController>();
        }
        estaminaActual = estaminaMaxima;

        // --- NUEVO: BLOQUEAR EL CURSOR AL INICIAR EL JUEGO ---
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    }

    void Update()
    {
        ManejarMovimientoYEstamina();
        CalcularRuido();
    }

    void ManejarMovimientoYEstamina()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;

        // Detectar si intenta correr (Tecla Shift Izquierdo) y se está moviendo
        bool quiereCorrer = Input.GetKey(KeyCode.LeftShift) && (moveX != 0 || moveZ != 0);

        if (quiereCorrer && estaminaActual > 0)
        {
            estaCorriendo = true;
            velocidadActual = velocidadCorrer;
            estaminaActual -= costoEstaminaPorSegundo * Time.deltaTime;

            if (estaminaActual <= 0)
            {
                estaminaActual = 0;
                estaCorriendo = false; // Se agotó la estamina
            }
        }
        else
        {
            estaCorriendo = false;
            velocidadActual = velocidadCaminar;

            // Recuperar estamina si no está corriendo
            if (estaminaActual < estaminaMaxima)
            {
                estaminaActual += recuperacionEstamina * Time.deltaTime;
                if (estaminaActual > estaminaMaxima) estaminaActual = estaminaMaxima;
            }
        }

        // Aplicar movimiento básico en el plano (ignorando gravedad por simplicidad o añadiéndola)
        controller.Move(moveDir * velocidadActual * Time.deltaTime);
    }

    void CalcularRuido()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool enMovimiento = (moveX != 0 || moveZ != 0);

        if (!enMovimiento)
        {
            nivelDeRuido = 0f;
            radioDeRuido = 0f; // Quieto = Sin ruido
        }
        else if (estaCorriendo)
        {
            nivelDeRuido = 2f;
            radioDeRuido = 18f; // El villano te escucha desde lejos al correr
        }
        else
        {
            nivelDeRuido = 1f;
            radioDeRuido = 5f; // Caminando normal, radio corto de escucha
        }
    }
}