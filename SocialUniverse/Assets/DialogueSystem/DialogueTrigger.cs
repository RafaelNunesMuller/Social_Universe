using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Transform dialoguePositionOffset;
    public DialogueManager dialogueManager; // Refer�ncia ao script DialogueManager
    public string[] dialogueLines;
    public string speakerName;
    public string interactionHint = "Pressione 'F' para conversar";
    public GameObject interactionHintUI;

    private Transform player;
    private bool isInInteractionRange = false;
    private bool isDialogueActive = false; // Nova vari�vel para controlar se o di�logo est� ativo

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager n�o atribu�do ao DialogueTrigger em " + gameObject.name);
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
        // S� tenta iniciar o di�logo se estiver na �rea de intera��o, a tecla 'F' for pressionada
        // E o di�logo N�O estiver ativo.
        if (isInInteractionRange && Input.GetKeyDown(KeyCode.F) && !isDialogueActive)
        {
            dialogueManager.StartDialogue(dialogueLines, speakerName, dialoguePositionOffset);
            isDialogueActive = true; // Marca o di�logo como ativo
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(false);
            }
        }

        // Precisamos tamb�m detectar quando o di�logo termina para reativar a intera��o.
        // Uma forma simples � verificar se o dialogueCanvas n�o est� mais ativo.
        if (isDialogueActive && (dialogueManager == null || !dialogueManager.dialogueCanvas.activeSelf))
        {
            isDialogueActive = false; // Marca o di�logo como inativo novamente
            // Se quiser reativar a dica de intera��o ao final do di�logo:
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
            if (interactionHintUI != null && !isDialogueActive) // S� mostra a dica se o di�logo n�o estiver ativo
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