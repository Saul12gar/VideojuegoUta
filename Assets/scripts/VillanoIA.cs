using UnityEngine;

public class VillanoIA : MonoBehaviour
{
    public Transform jugador;
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 5f;
    
    private enum Estado { Patrullando, Investigando, Persiguiendo }
    private Estado estadoActual = Estado.Patrullando;

    void Update()
    {
        if (jugador == null) return;

        // Obtenemos el componente de ruido del jugador
        PlayerController datosJugador = jugador.GetComponent<PlayerController>();

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
                // Lógica de patrullaje simple (puedes personalizarla)
                break;

            case Estado.Investigando:
                // Moverse hacia el último sonido o posición del jugador
                transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadPatrulla * Time.deltaTime);
                transform.LookAt(jugador);
                break;

            case Estado.Persiguiendo:
                // Persecución directa a toda velocidad
                transform.position = Vector3.MoveTowards(transform.position, jugador.position, velocidadPersecucion * Time.deltaTime);
                transform.LookAt(jugador);
                break;
        }
    }
}