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
        if (isInInteractionRange && Input.GetKeyDown(KeyCode.F))
        {
            // Chama a função StartDialogue NO SCRIPT DialogueManager
            dialogueManager.StartDialogue(dialogueLines, speakerName, dialoguePositionOffset);
            if (interactionHintUI != null)
            {
                interactionHintUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInInteractionRange = true;
            if (interactionHintUI != null)
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