using UnityEngine;
using UnityEngine.UI;

public class UIPanelGlow : MonoBehaviour
{
    public Image glowImage;
    public float minAlpha = 0.1f;
    public float maxAlpha = 0.4f;
    public float speed = 2f;

    void Update()
    {
        if (glowImage == null) return;

        // t vai de 0 a 1 oscilando suavemente
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = glowImage.color;
        c.a = a;
        glowImage.color = c;
    }
}
