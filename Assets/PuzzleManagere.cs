using System.Collections;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Peças do puzzle")]
    [Tooltip("Se deixar vazio, o script procura todas as PuzzlePiece nos filhos automaticamente.")]
    public PuzzlePiece[] pieces;

    private int totalPieces = 0;
    private int placedPieces = 0;

    [Header("Roots da cena (opcional)")]
    public GameObject piecesStartRoot;    // Pieces_Start (peças soltas)
    public GameObject piecesTargetsRoot;  // Pieces_Targets (slots/guia)

    [Header("Imagem final (fade simples)")]
    public MeshRenderer fullImageRenderer;  // Quad da imagem final
    public float fadeDuration = 3f;         // ajuste no Inspector

    [Header("Efeitos visuais")]
    public GameObject bookSparkles;         // partículas nos livros
    public GameObject artistInfoPanel;      // quadro de informações do artista

    void Awake()
    {
        // Se você não preencher as peças no Inspector, ele encontra todas automaticamente
        if (pieces == null || pieces.Length == 0)
        {
            pieces = GetComponentsInChildren<PuzzlePiece>(includeInactive: true);
            Debug.Log($"[PuzzleManager] Awake: encontrei {pieces.Length} peças automaticamente nos filhos.");
        }

        totalPieces = (pieces != null) ? pieces.Length : 0;
        placedPieces = 0;

        Debug.Log($"[PuzzleManager] Awake: totalPieces = {totalPieces}");
    }

    void Start()
    {
        // Garante que a imagem final começa invisível/desligada
        if (fullImageRenderer != null)
        {
            fullImageRenderer.gameObject.SetActive(false);

            Material mat = fullImageRenderer.material;
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = 0f;
                mat.color = c;
                Debug.Log("[PuzzleManager] Start: fullImageRenderer inicializado com alpha = 0.");
            }
            else
            {
                Debug.LogWarning("[PuzzleManager] Start: material da imagem final NÃO tem _Color. O fade não irá funcionar.");
            }
        }
        else
        {
            Debug.LogWarning("[PuzzleManager] Start: nenhum fullImageRenderer atribuído.");
        }

        // Efeitos começam desligados
        if (bookSparkles != null)
            bookSparkles.SetActive(false);

        if (artistInfoPanel != null)
            artistInfoPanel.SetActive(false);
    }

    // Chamado por cada PuzzlePiece quando encaixa corretamente
    public void PiecePlacedCorrectly()
    {
        placedPieces++;

        if (totalPieces <= 0 && pieces != null)
        {
            totalPieces = pieces.Length;
            Debug.LogWarning($"[PuzzleManager] totalPieces estava 0. Recalculado para {totalPieces}.");
        }

        Debug.Log($"[PuzzleManager] Peça colocada: {placedPieces}/{totalPieces}");

        if (placedPieces >= totalPieces && totalPieces > 0)
        {
            Debug.Log("[PuzzleManager] Puzzle completo! Iniciando animação de fade.");
            StartCoroutine(OnPuzzleComplete());
        }
    }

    private IEnumerator OnPuzzleComplete()
    {
        // 1) Desativar as peças/slots se você quiser sumir com o puzzle
        if (piecesStartRoot != null)
        {
            piecesStartRoot.SetActive(false);
            Debug.Log("[PuzzleManager] piecesStartRoot desativado.");
        }

        if (piecesTargetsRoot != null)
        {
            piecesTargetsRoot.SetActive(false);
            Debug.Log("[PuzzleManager] piecesTargetsRoot desativado.");
        }

        // 2) Ativar a imagem final e aplicar fade simples no _Color.a
        if (fullImageRenderer != null)
        {
            fullImageRenderer.gameObject.SetActive(true);
            Debug.Log("[PuzzleManager] fullImageRenderer ativado para fade.");

            Material mat = fullImageRenderer.material;

            if (mat.HasProperty("_Color"))
            {
                yield return StartCoroutine(FadeInFullImage(mat));
            }
            else
            {
                Debug.LogWarning("[PuzzleManager] Material da imagem final não tem _Color. Não foi possível aplicar fade.");
            }
        }
        else
        {
            Debug.LogWarning("[PuzzleManager] fullImageRenderer é nulo. Não há imagem final para mostrar.");
        }

        // 3) Após o fade, ligar os efeitos visuais
        if (bookSparkles != null)
        {
            bookSparkles.SetActive(true);
            Debug.Log("[PuzzleManager] Partículas nos livros ativadas.");
        }

        if (artistInfoPanel != null)
        {
            artistInfoPanel.SetActive(true);
            Debug.Log("[PuzzleManager] Painel de informações do artista ativado.");

            // animação simples de "pop" no quadro
            StartCoroutine(ScalePop(artistInfoPanel.transform));
        }
    }

    private IEnumerator FadeInFullImage(Material mat)
    {
        Color c = mat.color;
        float t = 0f;

        float startAlpha = 0f;
        float endAlpha = 1f;

        Debug.Log("[PuzzleManager] Iniciando fade da imagem final.");

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fadeDuration);

            // easing suave
            float smooth = lerp * lerp * (3f - 2f * lerp);

            c.a = Mathf.Lerp(startAlpha, endAlpha, smooth);
            mat.color = c;

            yield return null;
        }

        c.a = 1f;
        mat.color = c;

        Debug.Log("[PuzzleManager] Fade da imagem final concluído.");
    }

    private IEnumerator ScalePop(Transform target)
    {
        Vector3 finalScale = target.localScale;
        Vector3 startScale = finalScale * 0.1f; // começa bem pequeno

        target.localScale = startScale;

        float t = 0f;
        float duration = 0.4f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            // easing simples
            float smooth = lerp * lerp * (3f - 2f * lerp);

            target.localScale = Vector3.Lerp(startScale, finalScale, smooth);
            yield return null;
        }

        target.localScale = finalScale;
    }

    public void ResetPuzzle()
    {
        Debug.Log("[PuzzleManager] Resetando puzzle...");

        placedPieces = 0;

        if (piecesStartRoot != null)
            piecesStartRoot.SetActive(true);

        if (piecesTargetsRoot != null)
            piecesTargetsRoot.SetActive(true);

        if (fullImageRenderer != null)
        {
            Material mat = fullImageRenderer.material;
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = 0f;
                mat.color = c;
            }

            fullImageRenderer.gameObject.SetActive(false);
        }

        // Desliga efeitos
        if (bookSparkles != null)
            bookSparkles.SetActive(false);

        if (artistInfoPanel != null)
            artistInfoPanel.SetActive(false);

        if (pieces != null)
        {
            foreach (var p in pieces)
            {
                if (p != null)
                    p.ResetPiece();
            }
        }

        Debug.Log("[PuzzleManager] Puzzle resetado.");
    }
}
