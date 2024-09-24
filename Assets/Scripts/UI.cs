using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour {
    public GameObject game;
    public GameObject mainMenu;
    public GameObject menuOptions;
    public GameObject difficulties;

    public TextMeshProUGUI message;
    public TextMeshProUGUI subMessage;

    List<GameObject> elements;

    void Start() {
        elements = new List<GameObject>{
            game,
            mainMenu,
            menuOptions,
            difficulties,
        };
    }

    public void DisplayGame() => SetActive(game);
    public void DisplayMenuOptions() => SetActive(mainMenu, menuOptions);
    public void DisplayDifficulties() => SetActive(mainMenu, difficulties);

    public void SetMessage(string text) => message.text = text;
    public void SetSubMessage(string text) => subMessage.text = text;
    
    void SetActive(params GameObject[] gameObjects) {
        foreach (GameObject gameObject in elements.Except(gameObjects)) {
            gameObject.SetActive(false);
        }
        
        foreach (GameObject gameObject in gameObjects) {
            gameObject.SetActive(true);
        }
    }
}
