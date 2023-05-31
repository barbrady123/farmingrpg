using System.Collections;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds _pause;
    private bool _isAnimating = false;

    private void Awake()
    {
        _pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isAnimating)
            return;

        StartCoroutine(
            Nudge(
                gameObject.transform.position.x >= collision.gameObject.transform.position.x));

        if (collision.gameObject.tag == Global.Tags.Player)
        {
            AudioManager.Instance.PlaySound(SoundName.EffectRustle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isAnimating)
            return;

        StartCoroutine(
            Nudge(
                gameObject.transform.position.x <= collision.gameObject.transform.position.x));

        if (collision.gameObject.tag == Global.Tags.Player)
        {
            AudioManager.Instance.PlaySound(SoundName.EffectRustle);
        }
    }

    private IEnumerator Nudge(bool isClockwise)
    {
        _isAnimating = true;

        var transform = gameObject.transform.GetChild(0);
        var origRotation = transform.rotation;

        yield return NudgeIncrement(4, isClockwise);
        yield return NudgeIncrement(5, !isClockwise);
        transform.rotation = origRotation;

        yield return _pause;

        _isAnimating = false;
    }

    private IEnumerator NudgeIncrement(int count, bool isClockwise)
    {
        for (int x = 0; x < count; x++)
        {
            transform.Rotate(0f, 0f, isClockwise ? -2f : 2f);
            yield return _pause;
        }
    }
}
