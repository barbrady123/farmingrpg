using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float diff;

        do
        {
            diff = currentAlpha - Settings.targetAlpha;
            currentAlpha = Mathf.Max(currentAlpha - (diff / Settings.fadeOutSeconds * Time.deltaTime), Settings.targetAlpha);
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        } while (diff > 0.01f);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float diff;

        // why is this not visually fading out...just showing the alpha at 1f at the end?
        print("start");
        do
        {
            diff = 1f - currentAlpha;
            print($"diff:{diff}");
            currentAlpha = Mathf.Min(currentAlpha + (diff / Settings.fadeInSeconds * Time.deltaTime), 1f);
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            print($"alpha:{currentAlpha}");
            yield return null;
        } while (diff > 0.01f);
        print("done");
    }
}
