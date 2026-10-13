using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public float openAngle = 90f; // Cuánto se abre la puerta
    public float speed = 2f; // Velocidad de apertura
    
    private bool isOpen = false;
    private bool playerInZone = false;
    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Guarda la rotación inicial y calcula la rotación abierta
        defaultRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);
    }

    void Update()
    {
        // Detecta el input si el jugador está en la zona
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Rota suavemente hacia el estado correspondiente
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * speed);
        }
    }

    // Detecta cuando el jugador entra al Box Collider (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInZone = true;
        }
    }

    // Detecta cuando el jugador sale del área
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInZone = false;
        }
    }
}