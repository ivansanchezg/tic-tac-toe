using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject cells;

    private Text[] texts;
    private string currentPlayer;
    private string previousWinner;

    // To do:
    // Game Over method. Block all buttons
    // Restart button
    // Current Player display
    // Win counters

	void Awake () {
        previousWinner = null;
        SetCurrentPlayer();

        Cell[] cellList = cells.GetComponentsInChildren<Cell>();
        texts = new Text[cellList.Length];
        for (int index = 0; index < texts.Length; index++) {
            texts[index] = cellList[index].GetComponentInChildren<Text>();
            Debug.Log(texts[index].name);
        }
    }

    public void SetCurrentPlayer() {
        if (previousWinner == "X") {
            currentPlayer = "O";
        } else if (previousWinner == "O") {
            currentPlayer = "X";
        } else {
            int random = Random.Range(0, 2);
            currentPlayer = random == 0 ? "X" : "O";
        }
    }

    public string GetCurrentPlayer() {
        return currentPlayer;
    }

    public void EndTurn() {
        if (CheckWin()) {
            Debug.Log("Player " + currentPlayer + " won");
        } else {
            ChangePlayer();
        }
    }

    private bool CheckWin() {
        return
            CheckLine(0, 1) || CheckLine(3, 1) || CheckLine(6, 1) ||
            CheckLine(0, 3) || CheckLine(1, 3) || CheckLine(2, 3) ||
            CheckLine(0, 4) || CheckLine(2, 2);
    }

    private bool CheckLine(int start, int offset) {
        string compare = texts[start].text;
        for (int index = start; index <= start + (offset * 2); index += offset) {
            if (texts[index].text.Length == 0 || compare != texts[index].text) {
                return false;
            }
        }
        return true;
    }

    private void ChangePlayer() {
        currentPlayer = currentPlayer == "X" ? "O" : "X";
    }
}
