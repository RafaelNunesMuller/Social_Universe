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
    public float typingSpeed = 0.05f; // Tempo entre a apari��o de cada caractere

    public CinemachineCamera cinemachineCamera; // Refer�ncia � sua c�mera FreeLook
    private Transform originalLookAtTarget;
    private Transform originalFollowTarget;
    public float cameraTransitionTime = 1f; // Tempo para a transi��o da c�mera
    public float jumpReactivationDelay = 0.2f; // Ajuste este valor conforme necess�rio

    private PlayerMove playerMoveScript; // Refer�ncia ao script de movimento do player
    private float originalPlayerSpeed; // Vari�vel para armazenar a velocidade original do player

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
            Debug.LogError("Dialogue Canvas n�o atribu�do ao DialogueManager em " + gameObject.name);
            enabled = false;
        }

        if (cinemachineCamera == null)
        {
            Debug.LogError("Cinemachine FreeLook Camera n�o foi atribu�da ao DialogueManager em " + gameObject.name);
            enabled = false;
        }

        // Encontra o script PlayerMove no Start
        playerMoveScript = Object.FindFirstObjectByType<PlayerMove>();
        if (playerMoveScript == null)
        {
            Debug.LogError("N�o foi encontrado um script PlayerMove na cena. Certifique-se de que o Player possui esse script.");
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

            // Se o di�logo estiver ativo e a rotina de digita��o n�o estiver rodando,
            // permite avan�ar para a pr�xima linha com a tecla Space.
            if (Input.GetKeyDown(KeyCode.Space) && typeSentenceCoroutine == null)
            {
                DisplayNextLine();
            }
            // Se a rotina de digita��o estiver rodando e o jogador pressionar Space,
            // pula a digita��o e exibe a linha completa imediatamente.
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

        // Salva os alvos originais da c�mera FreeLook
        originalLookAtTarget = cinemachineCamera.LookAt;
        originalFollowTarget = cinemachineCamera.Follow;

        // Define o NPC como o novo alvo da c�mera FreeLook
        cinemachineCamera.LookAt = targetPosition;
        // Opcional: Se quiser mudar o "follow" tamb�m, descomente a linha abaixo
        // cinemachineCamera.Follow = targetPosition;

        // Bloqueia o movimento, a rota��o e o pulo do jogador
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

        // Inicia a exibi��o da primeira linha com o efeito de digita��o
        StartCoroutine(TypeSentence());
    }

    void DisplayNextLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            // Inicia a exibi��o da pr�xima linha com o efeito de digita��o
            StartCoroutine(TypeSentence());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence()
    {
        // Garante que n�o haja outra rotina de digita��o rodando
        typeSentenceCoroutine = StartCoroutine(ActuallyTypeSentence());
        yield return typeSentenceCoroutine; // Espera a coroutine ActuallyTypeSentence terminar
                                            // Indica que a rotina de digita��o terminou (j� feito dentro de ActuallyTypeSentence)
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
        // Indica que a rotina de digita��o terminou
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

        // Restaura os alvos originais da c�mera FreeLook
        cinemachineCamera.LookAt = originalLookAtTarget;
        cinemachineCamera.Follow = originalFollowTarget;

        // Restaura o movimento, a rota��o e desativa o pulo temporariamente
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