using TMPro;
using UnityEngine;

public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshTop1 = null;
    [SerializeField] private TextMeshProUGUI _textMeshTop2 = null;
    [SerializeField] private TextMeshProUGUI _textMeshTop3 = null;
    [SerializeField] private TextMeshProUGUI _textMeshBottom1 = null;
    [SerializeField] private TextMeshProUGUI _textMeshBottom2 = null;
    [SerializeField] private TextMeshProUGUI _textMeshBottom3 = null;

    public void SetTextboxText(
        string textTop1,
        string textTop2,
        string textTop3,
        string textBottom1,
        string textBottom2,
        string textBottom3)
    {
        _textMeshTop1.text = textTop1;
        _textMeshTop2.text = textTop2;
        _textMeshTop3.text = textTop3;
        _textMeshBottom1.text = textBottom1;
        _textMeshBottom2.text = textBottom2;
        _textMeshBottom3.text = textBottom3;
    }
}
