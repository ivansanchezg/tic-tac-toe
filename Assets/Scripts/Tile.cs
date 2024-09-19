using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string value { get; private set; } = null;
    TextMeshProUGUI textMeshPro;

    void Start() {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnClick() {
        if (value == null) {
            TicTacToe.instance.SetValue(this);
        }
    }

    public void SetValue(string value) {
        this.value = value;
        textMeshPro.text = value;
    }

    public void Clear() {
        this.value = null;
        textMeshPro.text = "";
    }
}
