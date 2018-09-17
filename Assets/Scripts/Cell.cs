using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    public GameController gameController;

    public void Click() {
        Debug.Log("\nClick.\nname = " + this.name);
        Text text = this.GetComponentInChildren<Text>();
        text.text = gameController.GetCurrentPlayer();

        Button button = this.GetComponent<Button>();
        button.interactable = false;

        gameController.EndTurn();
    }
}
