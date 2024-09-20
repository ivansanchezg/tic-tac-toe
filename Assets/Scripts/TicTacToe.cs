using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour {

    public static TicTacToe instance;
    
    public GridLayoutGroup gridLayoutGroup;
    public TextMeshProUGUI message;
    public TextMeshProUGUI subMessage;

    public GameObject game;
    public GameObject mainMenu;

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

    public void SinglePlayer() {
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

    void RestartGame(InputAction.CallbackContext context) {
        if (isGameOver) {
            Restart();
        }
    }
    
    void LoadMainMenu(InputAction.CallbackContext context) {
        if (isGameOver) {
            isP1Turn = true;
            currentLetter = "X";
            game.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    void SetPlayerTurnMessage() {
        if (isSinglePlayer) {
            if (isP1Turn) {
                message.text = $"It is your turn ({currentLetter})";
            } else {
                message.text = $"It is the CPU's turn ({currentLetter})";
            }
        } else {
            if (isP1Turn) {
                message.text = $"It is Player 1 turn ({currentLetter})";
            } else {
                message.text = $"It is Player 2 turn ({currentLetter})";
            }
        }
    }

    void Restart() {
        game.SetActive(true);
        mainMenu.SetActive(false);

        SetPlayerTurnMessage();
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

        isGameOver = false;

        if (isSinglePlayer && !isP1Turn) {
            StartCoroutine(CpuMove());
        }
    }

    public void SetValue(Tile tile) {
        if (isGameOver) { return; }
        tile.SetValue(currentLetter);
        UpdatePlayer();
        CheckGameOver();

        if (!isGameOver && isSinglePlayer && !isP1Turn) {
            StartCoroutine(CpuMove());
        }
    }

    IEnumerator CpuMove() {
        // Will start with just selecting an empty random tile
        yield return new WaitForSeconds(1);
        var tilesList = tiles.Cast<Tile>().ToList();

        int randomIndex;
        do {
            randomIndex = Random.Range(0, tilesList.Count);
        } while (tilesList[randomIndex].value != null);
        
        SetValue(tilesList[randomIndex]);
    }

    void UpdatePlayer() {
        if (isGameOver) { return; }

        if (currentLetter == "X") {
            currentLetter = "O";
        } else {
            currentLetter = "X";
        }

        isP1Turn = !isP1Turn;

        SetPlayerTurnMessage();
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
            message.text = "It is a draw!";
            isGameOver = true;
        } else if (winner != null) {
            if (isSinglePlayer) {
                // We already changed the turn, so revert it to determine the winner
                if (isP1Turn) { // Previous turn was the CPU
                    message.text = "You lose";    
                } else {
                    message.text = "You win";
                }
            } else {
                if (isP1Turn) { // Previous turn was the Player 2
                    message.text = $"Player 2 ({winner}) wins";
                } else {
                    message.text = $"Player 1 ({winner}) wins";
                }
            }
            
            isGameOver = true;
        }

        if (isGameOver) {
            subMessage.text = "Press Enter to play again.\nPress Esc to return to main menu.";
        }
    }
}
