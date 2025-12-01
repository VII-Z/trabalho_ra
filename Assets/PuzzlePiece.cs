using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzlePiece : MonoBehaviour
{
    [Header("Alvo correto da peça")]
    public Transform targetTransform;        // Slot_X
    public float magnetDistance = 0.4f;   // em vez de 0.15
    public float magnetSpeed = 10f;       // um pouco mais rápido
    public float finalSnapDistance = 0.02f;


    // Estado inicial da peça (EM RELAÇÃO AO PAI)
    [HideInInspector] public Vector3 startLocalPosition;
    [HideInInspector] public Quaternion startLocalRotation;
    [HideInInspector] public Vector3 startLocalScale;

    bool isPlaced = false;    // já encaixou e travou
    bool isSnapping = false;  // está sendo puxada pelo ímã

    Camera cam;
    PuzzleManager puzzleManager;

    float zDistanceToCamera;
    Vector3 dragOffset;

    void Start()
    {
        cam = Camera.main;
        puzzleManager = FindObjectOfType<PuzzleManager>();

        // guarda estado inicial EM LOCAL (relativo ao pai, ex.: Pieces_Start)
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
        startLocalScale = transform.localScale;
    }

    void Update()
    {
        // movimento "ímã"
        if (isSnapping && !isPlaced && targetTransform != null)
        {
            // aproxima posição e rotação do alvo (em espaço de MUNDO)
            transform.position = Vector3.Lerp(
                transform.position,
                targetTransform.position,
                Time.deltaTime * magnetSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetTransform.rotation,
                Time.deltaTime * magnetSpeed
            );

            // quando estiver bem perto, considera encaixado
            if (Vector3.Distance(transform.position, targetTransform.position) <= finalSnapDistance)
            {
                SnapToTarget();
            }
        }
    }

    void OnMouseDown()
    {
        if (isPlaced || isSnapping) return;

        if (cam == null) cam = Camera.main;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        zDistanceToCamera = screenPos.z;

        Vector3 mouseScreen = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            zDistanceToCamera
        );
        Vector3 worldPoint = cam.ScreenToWorldPoint(mouseScreen);

        dragOffset = transform.position - worldPoint;
    }

    void OnMouseDrag()
    {
        if (isPlaced || isSnapping) return;

        if (cam == null) cam = Camera.main;

        Vector3 mouseScreen = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            zDistanceToCamera
        );
        Vector3 worldPoint = cam.ScreenToWorldPoint(mouseScreen);

        transform.position = worldPoint + dragOffset;
    }

    void OnMouseUp()
    {
        if (isPlaced || targetTransform == null) return;

        float dist = Vector3.Distance(transform.position, targetTransform.position);

        // se soltar perto o suficiente, ativa o ímã
        if (dist <= magnetDistance)
        {
            isSnapping = true;
        }
    }

    void SnapToTarget()
    {
        isPlaced = true;
        isSnapping = false;

        if (targetTransform != null)
        {
            // COLA EXATAMENTE no alvo (sem mexer em Z extra, sem mudar pai)
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
            transform.localScale = startLocalScale; // mantém o tamanho original
        }

        // trava inputs
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        this.enabled = false;

        if (puzzleManager != null)
            puzzleManager.PiecePlacedCorrectly();
    }

    // chamado pelo PuzzleManager no botão de reset
    public void ResetPiece()
    {
        isPlaced = false;
        isSnapping = false;

        // VOLTA EXATAMENTE pra pose inicial, relativa ao pai (Pieces_Start)
        transform.localPosition = startLocalPosition;
        transform.localRotation = startLocalRotation;
        transform.localScale = startLocalScale;

        // reativa interação
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        this.enabled = true;
    }
}
