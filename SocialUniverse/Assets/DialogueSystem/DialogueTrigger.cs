using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Transform dialoguePositionOffset;
    public DialogueManager dialogueManager; // Referência ao script DialogueManager
    public string[] dialogueLines;
    public string speakerName;
    public string interactionHint = "Pressione 'F' para conversar";
    public GameObject interactionHintUI;

    private Transform player;
    private bool isInInteractionRange = false;
    private bool isDialogueActive = false; // Nova variável para controlar se o diálogo está ativo

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager não atribuído ao DialogueTrigger em " + gameObject.name);
            enabled = false;
        }
        if (dialoguePositionOffset == null)
        {
            dialoguePositionOffset = transform;
        }

        if (interactionHintUI != null)
        {
            interactionHintUI.SetActive(false);
        }
    }

    void Update()
    {
        // Só tenta iniciar o diálogo se estiver na área de interação, a tecla 'F' for pressionada
        // E o diálogo NÃO estiver ativo.
        if (isInInteractionRange && Input.GetKeyDown(KeyCode.F) && !isDialogueActive)
        {
            dialogueManager.StartDialogue(dialogueLines, speakerName, dialoguePositionOffset);
            isDialogueActive = true; // Marca o diálogo como ativo
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(false);
            }
        }

        // Precisamos também detectar quando o diálogo termina para reativar a interação.
        // Uma forma simples é verificar se o dialogueCanvas não está mais ativo.
        if (isDialogueActive && (dialogueManager == null || !dialogueManager.dialogueCanvas.activeSelf))
        {
            isDialogueActive = false; // Marca o diálogo como inativo novamente
            // Se quiser reativar a dica de interação ao final do diálogo:
            if (isInInteractionRange && interactionHintUI != null)
            {
                interactionHintUI.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInInteractionRange = true;
            if (interactionHintUI != null && !isDialogueActive) // Só mostra a dica se o diálogo não estiver ativo
            {
                interactionHintUI.SetActive(true);
                TextMeshProUGUI hintText_TMP = interactionHintUI.GetComponent<TextMeshProUGUI>();
                if (hintText_TMP != null)
                {
                    hintText_TMP.text = interactionHint;
                }
                Text hintText_Legacy = interactionHintUI.GetComponent<Text>();
                if (hintText_Legacy != null)
                {
                    hintText_Legacy.text = interactionHint;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInInteractionRange = false;
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && triggerCollider.isTrigger)
        {
            if (triggerCollider is SphereCollider)
            {
                Gizmos.DrawWireSphere(transform.position + ((SphereCollider)triggerCollider).center, ((SphereCollider)triggerCollider).radius);
            }
            else if (triggerCollider is BoxCollider)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(((BoxCollider)triggerCollider).center, ((BoxCollider)triggerCollider).size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}