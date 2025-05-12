using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine; // Importe o namespace Cinemachine

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public TextMeshProUGUI dialogueText_TMP;
    public Text dialogueText_Legacy;
    public TextMeshProUGUI speakerNameText_TMP;
    public Text speakerNameText_Legacy;
    public float cameraZoomDistance = 2f;
    public float cameraZoomSpeed = 5f;
    public float typingSpeed = 0.05f; // Tempo entre a aparição de cada caractere

    public CinemachineCamera cinemachineCamera; // Referência à sua câmera FreeLook
    private Transform originalLookAtTarget;
    private Transform originalFollowTarget;
    public float cameraTransitionTime = 1f; // Tempo para a transição da câmera
    public float jumpReactivationDelay = 0.2f; // Ajuste este valor conforme necessário

    private PlayerMove playerMoveScript; // Referência ao script de movimento do player
    private float originalPlayerSpeed; // Variável para armazenar a velocidade original do player

    private Transform mainCameraTransform;
    private Transform targetTransform;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Transform targetLookAt;
    private string[] currentDialogueLines;
    private string currentSpeakerName;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private Coroutine typeSentenceCoroutine;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Dialogue Canvas não atribuído ao DialogueManager em " + gameObject.name);
            enabled = false;
        }

        if (cinemachineCamera == null)
        {
            Debug.LogError("Cinemachine FreeLook Camera não foi atribuída ao DialogueManager em " + gameObject.name);
            enabled = false;
        }

        // Encontra o script PlayerMove no Start
        playerMoveScript = Object.FindFirstObjectByType<PlayerMove>();
        if (playerMoveScript == null)
        {
            Debug.LogError("Não foi encontrado um script PlayerMove na cena. Certifique-se de que o Player possui esse script.");
        }
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (targetTransform != null && dialogueCanvas != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
                dialogueCanvas.GetComponent<RectTransform>().position = screenPos;
            }

            if (targetLookAt != null)
            {
                float distance = Vector3.Distance(mainCameraTransform.position, targetLookAt.position);
                if (distance > cameraZoomDistance)
                {
                    mainCameraTransform.position = Vector3.Lerp(mainCameraTransform.position, targetLookAt.position + mainCameraTransform.forward * -cameraZoomDistance, Time.deltaTime * cameraZoomSpeed);
                }
                mainCameraTransform.LookAt(targetLookAt);
            }

            // Se o diálogo estiver ativo e a rotina de digitação não estiver rodando,
            // permite avançar para a próxima linha com a tecla Space.
            if (Input.GetKeyDown(KeyCode.Space) && typeSentenceCoroutine == null)
            {
                DisplayNextLine();
            }
            // Se a rotina de digitação estiver rodando e o jogador pressionar Space,
            // pula a digitação e exibe a linha completa imediatamente.
            else if (Input.GetKeyDown(KeyCode.Space) && typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
                if (dialogueText_TMP != null) dialogueText_TMP.text = currentDialogueLines[currentLineIndex - 1];
                if (dialogueText_Legacy != null) dialogueText_Legacy.text = currentDialogueLines[currentLineIndex - 1];
                typeSentenceCoroutine = null;
            }
        }
    }

    public void StartDialogue(string[] lines, string speaker, Transform targetPosition)
    {
        currentDialogueLines = lines;
        currentSpeakerName = speaker;
        currentLineIndex = 0;
        targetTransform = targetPosition;
        targetLookAt = targetPosition;

        originalCameraPosition = mainCameraTransform.position;
        originalCameraRotation = mainCameraTransform.rotation;

        // Salva os alvos originais da câmera FreeLook
        originalLookAtTarget = cinemachineCamera.LookAt;
        originalFollowTarget = cinemachineCamera.Follow;

        // Define o NPC como o novo alvo da câmera FreeLook
        cinemachineCamera.LookAt = targetPosition;
        // Opcional: Se quiser mudar o "follow" também, descomente a linha abaixo
        // cinemachineCamera.Follow = targetPosition;

        // Bloqueia o movimento, a rotação e o pulo do jogador
        if (playerMoveScript != null)
        {
            originalPlayerSpeed = playerMoveScript.Speed;
            playerMoveScript.Speed = 0f;
            playerMoveScript.canRotate = false;
            playerMoveScript.canJump = false; // Desabilita o pulo
        }

        isDialogueActive = true;
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(true);
        }

        if (speakerNameText_TMP != null) speakerNameText_TMP.text = currentSpeakerName;
        if (speakerNameText_Legacy != null) speakerNameText_Legacy.text = currentSpeakerName;

        // Inicia a exibição da primeira linha com o efeito de digitação
        StartCoroutine(TypeSentence());
    }

    void DisplayNextLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            // Inicia a exibição da próxima linha com o efeito de digitação
            StartCoroutine(TypeSentence());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence()
    {
        // Garante que não haja outra rotina de digitação rodando
        typeSentenceCoroutine = StartCoroutine(ActuallyTypeSentence());
        yield return typeSentenceCoroutine; // Espera a coroutine ActuallyTypeSentence terminar
                                            // Indica que a rotina de digitação terminou (já feito dentro de ActuallyTypeSentence)
                                            // currentLineIndex++; // Movido para ActuallyTypeSentence
    }

    IEnumerator ActuallyTypeSentence()
    {
        if (dialogueText_TMP != null) dialogueText_TMP.text = "";
        if (dialogueText_Legacy != null) dialogueText_Legacy.text = "";
        char[] sentence = currentDialogueLines[currentLineIndex].ToCharArray();
        foreach (char letter in sentence)
        {
            if (dialogueText_TMP != null) dialogueText_TMP.text += letter;
            if (dialogueText_Legacy != null) dialogueText_Legacy.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        // Indica que a rotina de digitação terminou
        typeSentenceCoroutine = null;
        currentLineIndex++;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }
        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = originalCameraPosition;
            mainCameraTransform.rotation = originalCameraRotation;
        }

        // Restaura os alvos originais da câmera FreeLook
        cinemachineCamera.LookAt = originalLookAtTarget;
        cinemachineCamera.Follow = originalFollowTarget;

        // Restaura o movimento, a rotação e desativa o pulo temporariamente
        if (playerMoveScript != null)
        {
            playerMoveScript.Speed = originalPlayerSpeed;
            playerMoveScript.canRotate = true;
            playerMoveScript.canJump = false;
            StartCoroutine(ReactivateJump());
        }

        targetTransform = null;
        targetLookAt = null;
    }

    IEnumerator ReactivateJump()
    {
        yield return new WaitForSeconds(jumpReactivationDelay);
        if (playerMoveScript != null)
        {
            playerMoveScript.canJump = true;
        }
    }

    void OnDisable()
    {
        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = originalCameraPosition;
            mainCameraTransform.rotation = originalCameraRotation;
        }
    }
}