using UnityEngine;

public class VillanoIA : MonoBehaviour
{
    public Transform jugador;
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 5f;
    
    private enum Estado { Patrullando, Investigando, Persiguiendo }
    private Estado estadoActual = Estado.Patrullando;

    private Animator animator;

    void Start()
    {
        // Busca el Animator en el modelo 3D hijo del villano
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No se encontró ningún componente Animator en el villano o sus hijos.");
        }
    }

    void Update()
    {
        if (jugador == null) return;

        // Obtenemos el componente de ruido del jugador
        PlayerController datosJugador = jugador.GetComponent<PlayerController>();

        float velocidadActualMovimiento = 0f;

        if (datosJugador != null)
        {
            // Calculamos la distancia actual entre el villano y el jugador
            float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

            // Si el radio de ruido del jugador alcanza al villano, este reacciona
            if (distanciaAlJugador <= datosJugador.radioDeRuido)
            {
                if (datosJugador.nivelDeRuido >= 2f)
                {
                    // ¡Corrió cerca! El villano lo ve/escucha directamente y lo persigue
                    estadoActual = Estado.Persiguiendo;
                }
                else if (datosJugador.nivelDeRuido == 1f && estadoActual != Estado.Persiguiendo)
                {
                    // Escuchó pasos sospechosos, investiga la posición
                    estadoActual = Estado.Investigando;
                }
            }
        }

        // Comportamiento según el estado
        switch (estadoActual)
        {
            case Estado.Patrullando:
                // Lógica de patrullaje simple (por ahora quieto o velocidad 0)
                velocidadActualMovimiento = 0f;
                break;

            case Estado.Investigando:
                // Moverse hacia el último sonido o posición del jugador
                transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadPatrulla * Time.deltaTime);
                transform.LookAt(jugador);
                velocidadActualMovimiento = velocidadPatrulla; // Activa la animación de caminar
                break;

            case Estado.Persiguiendo:
                // Persecución directa a toda velocidad
                transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadPersecucion * Time.deltaTime);
                transform.LookAt(jugador);
                velocidadActualMovimiento = velocidadPersecucion; // Activa la animación de correr
                break;
        }

        // Actualizar el parámetro de velocidad en el Animator para que mueva las piernas
        if (animator != null)
        {
            animator.SetFloat("Velocidad", velocidadActualMovimiento);
        }
    }
}