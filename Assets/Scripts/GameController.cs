using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject cells;
    public Text gameOverText;

    private Text[] texts;
    private Button[] buttons;
    private string currentPlayer;
    private string previousWinner;

    // To do:
    // Restart button
    // Current Player display
    // Win counters

    void Awake()
    {
        previousWinner = null;
        SetCurrentPlayer();

        Cell[] cellList = cells.GetComponentsInChildren<Cell>();
        buttons = cells.GetComponentsInChildren<Button>();
        texts = new Text[cellList.Length];
        for (int index = 0; index < texts.Length; index++)
        {

            texts[index] = cellList[index].GetComponentInChildren<Text>();
        }
    }

    public void SetCurrentPlayer()
    {
        if (previousWinner == "X")
        {
            currentPlayer = "O";
        }
        else if (previousWinner == "O")
        {
            currentPlayer = "X";
        }
        else
        {
            int random = Random.Range(0, 2);
            currentPlayer = random == 0 ? "X" : "O";
        }
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void EndTurn()
    {
        if (CheckWin())
        {
            GameOver(false);
        }
        else if (CheckDraw())
        {
            GameOver(true);
        }
        else
        {
            ChangePlayer();
        }
    }

    private bool CheckWin()
    {
        return
            CheckLine(0, 1) || CheckLine(3, 1) || CheckLine(6, 1) ||
            CheckLine(0, 3) || CheckLine(1, 3) || CheckLine(2, 3) ||
            CheckLine(0, 4) || CheckLine(2, 2);
    }

    private bool CheckDraw()
    {
        foreach (Button button in buttons)
        {
            if (button.interactable)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckLine(int start, int offset)
    {
        string compare = texts[start].text;
        for (int index = start; index <= start + (offset * 2); index += offset)
        {
            if (texts[index].text.Length == 0 || compare != texts[index].text)
            {
                return false;
            }
        }
        return true;
    }

    private void ChangePlayer()
    {
        currentPlayer = currentPlayer == "X" ? "O" : "X";
    }

    private void GameOver(bool draw)
    {
        if (draw)
        {
            gameOverText.text = "Draw";
        }
        else
        {
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
            gameOverText.text = currentPlayer + " Wins";
        }
    }
}
