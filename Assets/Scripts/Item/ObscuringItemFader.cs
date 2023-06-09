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

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float diff = 1f - currentAlpha;

        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + diff / Settings.FadeInSeconds * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        };

        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float diff = currentAlpha - Settings.TargetAlpha;

        while (currentAlpha - Settings.TargetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - diff / Settings.FadeOutSeconds * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        _spriteRenderer.color = new Color(1f, 1f, 1f, Settings.TargetAlpha);
    }
}
