using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var fader in collision.gameObject.GetComponentsInChildren<ObscuringItemFader>())
        {
            fader.FadeOut();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var fader in collision.gameObject.GetComponentsInChildren<ObscuringItemFader>())
        {
            fader.FadeIn();
        }
    }
}
