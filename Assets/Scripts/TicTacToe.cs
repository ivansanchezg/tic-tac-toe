using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour {

    public static TicTacToe instance;
    
    public GridLayoutGroup gridLayoutGroup;

    public UI ui;

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
    bool isGameOver = false;
    string currentLetter = "X";
    string cpuLetter;
    Difficulty difficulty;
    GameInput gameInput;

    public bool isSinglePlayer { get; private set; } = false;
    public bool isP1Turn { get; private set; }

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

        isP1Turn = true;
        currentLetter = "X";
    }

    void OnEnable() {
        gameInput.Game.Enter.performed += RestartGame;
        gameInput.Game.Esc.performed += LoadMainMenu;
        gameInput.Enable();
    }

    void OnDisable() {
        gameInput.Game.Enter.performed -= RestartGame;
        gameInput.Game.Esc.performed -= LoadMainMenu;
        gameInput.Disable();
    }

    public void SetDifficulty(int difficulty) {
        this.difficulty = (Difficulty) difficulty;
        isSinglePlayer = true;
        Restart();
    }

    public void TwoPlayers() {
        isSinglePlayer = false;
        Restart();
    }

    public void QuitGame() {
        Application.Quit();
    }

    void RestartGame(InputAction.CallbackContext context) {
        if (isGameOver) {
            Restart();
        }
    }
    
    void LoadMainMenu(InputAction.CallbackContext context) {
        if (isGameOver) {
            isP1Turn = true;
            currentLetter = "X";
            ui.DisplayMenuOptions();
        }
    }

    void SetPlayerTurnMessage() {
        if (isGameOver) { return; }

        if (isSinglePlayer) {
            if (isP1Turn) {
                ui.SetMessage($"It is your turn ({currentLetter})");
            } else {
                ui.SetMessage($"It is the CPU's turn ({currentLetter})");
            }
        } else {
            if (isP1Turn) {
                ui.SetMessage($"It is Player 1 turn ({currentLetter})");
            } else {
                ui.SetMessage($"It is Player 2 turn ({currentLetter})");
            }
        }
    }

    void Restart() {
        isGameOver = false;

        ui.DisplayGame();

        SetPlayerTurnMessage();
        ui.SetSubMessage("");

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

        if (isSinglePlayer) {
            if (!isP1Turn) {
                cpuLetter = currentLetter;
            } else {
                if (currentLetter == "X") {
                    cpuLetter = "O";
                } else {
                    cpuLetter = "X";
                }
            }
        }

        if (isSinglePlayer && !isP1Turn) {
            StartCoroutine(CpuMove());
        }
    }

    public void SetValue(Tile tile) {
        if (isGameOver) { return; }
        tile.SetValue(currentLetter);
        
        CheckGameOver();
        UpdatePlayer();
        SetPlayerTurnMessage();

        if (!isGameOver && isSinglePlayer && !isP1Turn) {
            StartCoroutine(CpuMove());
        }
    }

    IEnumerator CpuMove() {
        var tilesList = tiles.Cast<Tile>().ToList();
        int bestMove = FindBestMove();

        yield return new WaitForSeconds(1);

        if (difficulty == Difficulty.Hard) {
            if (IsGridEmpty()) {
                // 50% of using the center tile
                var value = UnityEngine.Random.Range(0, 2);
                if (value == 0) {
                    SetValue(tiles[1,1]);
                } else {
                    SetValue(tilesList[bestMove]);
                }
            } else {
                SetValue(tilesList[bestMove]);
            }
        } else if (difficulty == Difficulty.Normal) {
            // 75% of using the best move
            var random = UnityEngine.Random.Range(0, 4);
            if (random == 3) {
                SetValue(tilesList[getRandomEmptyTile(tilesList)]);
            } else {
                SetValue(tilesList[bestMove]);
            }
        } else {
            // 25% of using the best move
            var random = UnityEngine.Random.Range(0, 4);
            if (random == 3) {
                SetValue(tilesList[bestMove]);
            } else {
                SetValue(tilesList[getRandomEmptyTile(tilesList)]);
            }
        }
    }

    int getRandomEmptyTile(List<Tile> tilesList) {
        int randomIndex;
        do {
            randomIndex = UnityEngine.Random.Range(0, tilesList.Count);
        } while (tilesList[randomIndex].value != null);

        return randomIndex;
    }

    void UpdatePlayer() {
        if (currentLetter == "X") {
            currentLetter = "O";
        } else {
            currentLetter = "X";
        }

        isP1Turn = !isP1Turn;
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
            winner == null &&
            tiles[1, 0].value != null &&
            tiles[1, 0].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[1, 2].value
        ) { 
            winner = tiles[1, 0].value;
            centerRow.enabled = true;
        }

        // Bottom row
        if (
            winner == null &&
            tiles[2, 0].value != null &&
            tiles[2, 0].value == tiles[2, 1].value &&
            tiles[2, 1].value == tiles[2, 2].value
        ) { 
            winner = tiles[2, 0].value;
            bottomRow.enabled = true;
        }

        // Left column
        if (
            winner == null &&
            tiles[0, 0].value != null &&
            tiles[0, 0].value == tiles[1, 0].value &&
            tiles[1, 0].value == tiles[2, 0].value
        ) { 
            winner = tiles[0, 0].value;
            leftColumn.enabled = true;
        }

        // Center column
        if (
            winner == null &&
            tiles[0, 1].value != null &&
            tiles[0, 1].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[2, 1].value
        ) { 
            winner = tiles[0, 1].value;
            centerColumn.enabled = true;
        }

        // Right column
        if (
            winner == null &&
            tiles[0, 2].value != null &&
            tiles[0, 2].value == tiles[1, 2].value &&
            tiles[1, 2].value == tiles[2, 2].value
        ) { 
            winner = tiles[0, 2].value;
            rightColumn.enabled = true;
        }

        // Check top left to bottom right diagonal
        if (
            winner == null &&
            tiles[0, 0].value != null &&
            tiles[0, 0].value == tiles[1, 1].value &&
            tiles[1, 1].value == tiles[2, 2].value
        ) {
            winner = tiles[1, 1].value;
            topLeftDiagonal.enabled = true;
        }

        // Check top left to bottom right diagonal
        if (
            winner == null &&
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
            ui.SetMessage("It is a draw!");
            isGameOver = true;
        } else if (winner != null) {
            if (isSinglePlayer) {
                if (isP1Turn) {
                    ui.SetMessage("You win");
                } else {
                    ui.SetMessage("You lose");
                }
            } else {
                if (isP1Turn) {
                    ui.SetMessage($"Player 1 ({winner}) wins");
                } else {
                    ui.SetMessage($"Player 2 ({winner}) wins");
                }
            }
            
            isGameOver = true;
        }

        if (isGameOver) {
            ui.SetSubMessage("Press Enter to play again.\nPress Esc to return to main menu.");
        }
    }

    int FindBestMove() {
        var tilesList = tiles.Cast<Tile>().ToList();
        var copiedList = tilesList.Select((tile) => tile.value).ToList();

        int bestValue = -1000;
        int bestMove = -1;

        for (int i = 0; i < tilesList.Count; i++) {
            if (tilesList[i].value == null) {
                copiedList[i] = cpuLetter;
                int moveValue = Minimax(copiedList, 0, false);
                copiedList[i] = null;

                if (moveValue > bestValue) {
                    bestMove = i;
                    bestValue = moveValue;
                }
            }
        }

        return bestMove;
    }

    int Minimax(List<string> tilesList, int depth, bool isMax) {
        int score = Evaluate(tilesList);

        if (score == 10) {
            return score - depth;
        }

        if (score == -10) {
            return score + depth;
        }

        bool allSet = tilesList.All((tile) => tile != null);
        if (allSet) {
            return 0;
        }

        if (isMax) {
            int best = -1000;
            for (int i = 0; i < tilesList.Count; i++) {
                if (tilesList[i] == null) {
                    tilesList[i] = cpuLetter;
                    best = Mathf.Max(best, Minimax(tilesList, depth + 1, !isMax));
                    tilesList[i] = null;
                }
            }
            return best;
        } else {
            int best = 1000;
            for (int i = 0; i < tilesList.Count; i++) {
                if (tilesList[i] == null) {
                    tilesList[i] = getOpponentLetter(cpuLetter);
                    best = Math.Min(best, Minimax(tilesList, depth + 1, !isMax));
                    tilesList[i] = null;
                }
            }
            return best;
        }
    }

    int Evaluate(List<string> tilesList) {
        for (int row = 0; row < 3; row++) {
            if (tilesList[row * 3] == tilesList[row * 3 + 1] && tilesList[row * 3 + 1] == tilesList[row * 3 + 2]) {
                if (tilesList[row * 3] == cpuLetter) {
                    return 10;
                } else if (tilesList[row * 3] == getOpponentLetter(cpuLetter)) {
                    return -10;
                }
            }
        }

        for (int col = 0; col < 3; col++) {
            if (tilesList[col] == tilesList[col + 3] && tilesList[col + 3] == tilesList[col + 6]) {
                if (tilesList[col] == cpuLetter) {
                    return 10;
                } else if (tilesList[col] == getOpponentLetter(cpuLetter)) {
                    return -10;
                }
            }
        }

        // Top left to bottom right diagonal
        if (tilesList[0] == tilesList[4] && tilesList[4] == tilesList[8]) {
            if (tilesList[0] == cpuLetter) {
                return 10;
            } else if (tilesList[0] == getOpponentLetter(cpuLetter)) {
                return -10;
            }
        }

        // Top right to bottom left diagonal
        if (tilesList[2] == tilesList[4] && tilesList[4] == tilesList[6]) {
            if (tilesList[2] == cpuLetter) {
                return 10;
            } else if (tilesList[2] == getOpponentLetter(cpuLetter)) {
                return -10;
            }
        }

        return 0;
    }

    string getOpponentLetter(string playerLetter) {
        if (playerLetter == "O") {
            return "X";
        }
        return "O";
    }

    public void DisplayDifficulties() {
        ui.DisplayDifficulties();
    }

    bool IsGridEmpty() {
        var tilesList = tiles.Cast<Tile>().ToList();
        return tilesList.All(tile => tile.value == null);
    }
}

public enum Difficulty {
    Easy = 0,
    Normal = 1,
    Hard = 2,
}
