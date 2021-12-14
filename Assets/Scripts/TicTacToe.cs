using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TicTacToe : MonoBehaviour
{
    List<Button> buttons = new List<Button>();

    [SerializeField]
    List<Button> usedButtons = new List<Button>();

    public UnityEvent gamePlayButton;

    Button ResetButton;

    string playerShape = "O";
    string player2Shape = "X";

    bool playerone = true;

    void Start()
    {
        var allChildren = GetComponentsInChildren<Button>();

        foreach(Button child in allChildren)
        {
            // Add Button listener function to each button
            child.onClick.
                AddListener(delegate { PlayerTurn(child); });

            var text = child.transform.GetChild(0).GetComponent<Text>();
            text.text = "";
            buttons.Add(child);

        }
        //ResetButton = GameObject.Find("Reset").GetComponent<Button>();
        //ResetButton.onClick.AddListener(ResetGame);
    }

    private void Update()
    {
        foreach(Button used in usedButtons)
        {
            if(used.enabled == true)
            {
                used.enabled = false;
            }
        }
    }

    public void PlayerTurn(Button buttonClicked)
    {
        var TextHold = buttonClicked.gameObject.transform.GetChild(0).GetComponent<Text>();

        //foreach(Button button in buttons)
        //{
        //}

        // Calls the TicTacToeSquareTurnPlayed() in gameSystemManager to send
        // a turn played message to the server
        gamePlayButton.Invoke();

        if (playerone == true)
        {
            TextHold.text = playerShape;
            playerone = false;
        }
        else if(playerone == false)
        {
            TextHold.text = player2Shape;
            playerone = true;
        }

        //buttonClicked.enabled = false;
        usedButtons.Add(buttonClicked);
    }

    void ResetGame()
    {
        foreach(Button button in buttons)
        {
            if(button.enabled == false)
            {
                button.enabled = true;
            }
            var TextHold = button.gameObject.transform.GetChild(0).GetComponent<Text>();
            TextHold.text = "";
        }
    }

    public void DisableAllButtons()
    {
        foreach(Button button in buttons)
        {
            button.enabled = false;
        }
    }

    public void EnableAllButtons()
    {
        foreach (Button button in buttons)
        {
            button.enabled = true;
        }
    }
}
