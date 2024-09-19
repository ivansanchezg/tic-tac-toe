using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour {

    public static TicTacToe instance;
    
    public GridLayoutGroup gridLayoutGroup;
    public TextMeshProUGUI message;
    public TextMeshProUGUI subMessage;

    [Header("Winning Lines")]
    public Image leftColumn;
    public Image centerColumn;
    public Image rightColumn;
    public Image topRow;
    public Image centerRow;
    public Image bottomRow;
    public Image topLeftDiagonal;
    public Image topRightDiagonal;

    Tile[,] tiles = new Tile[3,3];
    bool gameOver = false;
    string currentPlayer = "X";
    GameInput gameInput;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        gameInput = new GameInput();
    }

    void Start() {
        for (int row = 0; row < 3; row++) {
            for (int col = 0; col < 3; col++) {
                tiles[row, col] = gridLayoutGroup.transform.GetChild((row * 3) + col).GetComponent<Tile>();
            }
        }
    }

    void OnEnable() {
        gameInput.Game.Enter.performed += RestartGame;
        gameInput.Enable();
    }

    void OnDisable() {
        gameInput.Game.Enter.performed -= RestartGame;
        gameInput.Disable();
    }

    void RestartGame(InputAction.CallbackContext context) {
        if (gameOver) {
            Restart();
        }
    }

    void Restart() {
        currentPlayer = "X";
        message.text = $"It is {currentPlayer}'s turn";
        subMessage.text = "";

        foreach (Tile tile in tiles) {
            tile.Clear();
        }

        leftColumn.enabled = false;
        centerColumn.enabled = false;
        rightColumn.enabled = false;
        topRow.enabled = false;
        centerRow.enabled = false;
        bottomRow.enabled = false;
        topLeftDiagonal.enabled = false;
        topRightDiagonal.enabled = false;

        gameOver = false;
    }

    public void SetValue(Tile tile) {
        if (gameOver) { return; }
        tile.SetValue(currentPlayer);
        CheckGameOver();
        UpdatePlayer();
    }

    void UpdatePlayer() {
        if (gameOver) { return; }

        if (currentPlayer == "X") {
            currentPlayer = "O";
        } else {
            currentPlayer = "X";
        }

        message.text = $"It is {currentPlayer}'s turn";
    }

    void CheckGameOver() {
        string winner = null;
        
        // Top row
        if (
            tiles[0, 0].value != null &&
            tiles[0, 0].value == tiles[0, 1].value &&
            tiles[0, 1].value == tiles[0, 2].value
        ) { 
            winner = tiles[0, 0].value;
            topRow.enabled = true;
        }

        // Center row
        if (
            tiles[1, 0].value != null &&
            tiles[1, 0].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[1, 2].value
        ) { 
            winner = tiles[1, 0].value;
            centerRow.enabled = true;
        }

        // Bottom row
        if (
            tiles[2, 0].value != null &&
            tiles[2, 0].value == tiles[2, 1].value &&
            tiles[2, 1].value == tiles[2, 2].value
        ) { 
            winner = tiles[2, 0].value;
            bottomRow.enabled = true;
        }

        // Left column
        if (
            tiles[0, 0].value != null &&
            tiles[0, 0].value == tiles[1, 0].value &&
            tiles[1, 0].value == tiles[2, 0].value
        ) { 
            winner = tiles[0, 0].value;
            leftColumn.enabled = true;
        }

        // Center column
        if (
            tiles[0, 1].value != null &&
            tiles[0, 1].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[2, 1].value
        ) { 
            winner = tiles[0, 1].value;
            centerColumn.enabled = true;
        }

        // Right column
        if (
            tiles[0, 2].value != null &&
            tiles[0, 2].value == tiles[1, 2].value &&
            tiles[1, 2].value == tiles[2, 2].value
        ) { 
            winner = tiles[0, 2].value;
            rightColumn.enabled = true;
        }

        // Check top left to bottom right diagonal
        if (
            tiles[0, 0].value != null &&
            tiles[0, 0].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[2, 2].value
        ) {
            winner = tiles[1, 1].value;
            topLeftDiagonal.enabled = true;
        }

        // Check top left to bottom right diagonal
        if (
            tiles[0, 2].value != null &&
            tiles[0, 2].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[2, 0].value
        ) {
            winner = tiles[1, 1].value;
            topRightDiagonal.enabled = true;
        }

        // Check if all tiles have a value
        bool allSet = tiles.Cast<Tile>().All((tile) => tile.value != null);

        if (allSet && winner == null) {
            message.text = "It is a draw!";
            gameOver = true;
        } else if (winner != null) {
            message.text = $"{winner} wins";
            gameOver = true;
        }

        if (gameOver) {
            subMessage.text = "Press Enter to play again";
        }
    }
}
