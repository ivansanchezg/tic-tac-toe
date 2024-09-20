using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string value { get; private set; } = null;
    TextMeshProUGUI textMeshPro;

    void Awake() {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnClick() {
        if (TicTacToe.instance.isSinglePlayer && !TicTacToe.instance.isP1Turn) {
            return;
        }

        if (value == null) {
            TicTacToe.instance.SetValue(this);
        }
    }

    public void SetValue(string value) {
        this.value = value;
        textMeshPro.text = value;
    }

    public void Clear() {
        value = null;
        textMeshPro.text = "";
    }
}
