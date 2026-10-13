using UnityEngine;

public class LinternaController : MonoBehaviour
{
    [Header("Configuración de la Linterna")]
    public Light luzLinterna; // Arrastra aquí tu componente Spot Light
    public KeyCode teclaControl = KeyCode.F; // Tecla para encender/apagar

    [Header("Efecto de parpadeo opcional (Terror)")]
    public bool parpadeoHabilitado = false;
    private float tiempoSiguienteParpadeo;

    void Start()
    {
        // Si no se asignó por el inspector, la busca automáticamente en los hijos
        if (luzLinterna == null)
        {
            luzLinterna = GetComponentInChildren<Light>();
        }

        // Encendida por defecto al iniciar el juego (opcional)
        if (luzLinterna != null)
        {
            luzLinterna.enabled = true;
        }
    }

    void Update()
    {
        // Presionar la tecla para alternar el estado
        if (Input.GetKeyDown(teclaControl))
        {
            if (luzLinterna != null)
            {
                luzLinterna.enabled = !luzLinterna.enabled;
            }
        }

        // Efecto opcional de parpadeo si la batería falla (ideal para momentos de tensión)
        if (parpadeoHabilitado && luzLinterna != null && luzLinterna.enabled)
        {
            if (Time.time > tiempoSiguienteParpadeo)
            {
                luzLinterna.enabled = false;
                Invoke("RestaurarLuz", Random.Range(0.05f, 0.2f));
                tiempoSiguienteParpadeo = Time.time + Random.Range(2f, 6f);
            }
        }
    }

    void RestaurarLuz()
    {
        if (luzLinterna != null)
        {
            luzLinterna.enabled = true;
        }
    }
}