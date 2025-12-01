using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BrushRevealController : MonoBehaviour
{
    [Header("Propriedade do shader")]
    public string progressProperty = "_Progress";

    [Header("Tempo da animação (segundos)")]
    public float revealDuration = 3f;

    [Header("Quanto menor a imagem começa (0.5 = 50% do tamanho)")]
    public float startScaleFactor = 0.5f;

    private Material _mat;
    private Coroutine _currentRoutine;
    private Vector3 _originalScale;

    void Awake()
    {
        var mr = GetComponent<MeshRenderer>();
        _mat = mr.material;
        _originalScale = transform.localScale;

        if (_mat.HasProperty(progressProperty))
        {
            // Começa "apagado": Progress = 1
            _mat.SetFloat(progressProperty, 1f);
            Debug.Log($"[BrushReveal] Awake em {name}. Shader = {_mat.shader.name}, progress=1");
        }
        else
        {
            Debug.LogWarning(
                $"[BrushReveal] Material em {gameObject.name} NÃO tem a propriedade {progressProperty}. Shader = {_mat.shader.name}"
            );
        }

        // NÃO desativa o objeto aqui, só deixa apagado pelo shader
        // gameObject.SetActive(true);
    }

    public void StartReveal()
    {
        if (_mat == null || !_mat.HasProperty(progressProperty))
        {
            Debug.LogWarning($"[BrushReveal] StartReveal chamado em {name}, mas material não tem {progressProperty}.");
            return;
        }

        Debug.Log($"[BrushReveal] StartReveal em {name}");

        gameObject.SetActive(true); // garante que o Quad está ativo

        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);

        _currentRoutine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        float t = 0f;

        // vamos de 1 → 0
        float startP = 1f;
        float endP   = 0f;

        // começa menor e vai "abrindo"
        Vector3 smallScale = _originalScale * startScaleFactor;
        transform.localScale = smallScale;

        _mat.SetFloat(progressProperty, startP);

        while (t < revealDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / revealDuration);

            // anima o _Progress de 1 até 0
            float p = Mathf.Lerp(startP, endP, lerp);
            _mat.SetFloat(progressProperty, p);

            // anima o tamanho: de pequeno até o original
            transform.localScale = Vector3.Lerp(smallScale, _originalScale, lerp);

            yield return null;
        }

        _mat.SetFloat(progressProperty, endP);   // garante que termina em 0 (imagem totalmente visível)
        transform.localScale = _originalScale;   // tamanho final normal

        Debug.Log($"[BrushReveal] Reveal concluído em {name}, progress=0");
    }
}
