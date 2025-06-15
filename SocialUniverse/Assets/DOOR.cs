using UnityEngine;

public class DOOR : MonoBehaviour
{
    public Transform portaTransform; // arraste aqui a parte móvel da porta
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool playerNearby = false;
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpening = false;

    void Start()
    {
        if (portaTransform == null)
        {
            portaTransform = this.transform; // fallback se não for atribuído no Inspector
        }

        closedRotation = portaTransform.rotation;
        openRotation = Quaternion.Euler(
            portaTransform.eulerAngles.x,
            portaTransform.eulerAngles.y + openAngle,
            portaTransform.eulerAngles.z
        );
    }

    void Update()
    {
        if (playerNearby && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Player_Move playerMove = playerObj.GetComponent<Player_Move>();

                if (playerMove != null && playerMove.HasKey())
                {
                    isOpening = true;
                    playerMove.UseKey();
                    Debug.Log("Porta aberta com chave!");
                }
                else
                {
                    Debug.Log("Você precisa de uma chave para abrir esta porta!");
                }
            }
        }

        if (isOpening && !isOpen)
        {
            portaTransform.rotation = Quaternion.Lerp(portaTransform.rotation, openRotation, Time.deltaTime * openSpeed);

            if (Quaternion.Angle(portaTransform.rotation, openRotation) < 1f)
            {
                portaTransform.rotation = openRotation;
                isOpening = false;
                isOpen = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
