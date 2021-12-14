using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonChange : MonoBehaviour
{
    List<Button> buttons = new List<Button>();

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
            child.onClick.
                AddListener(delegate { PlayerTurn(child); });

            var text = child.transform.GetChild(0).GetComponent<Text>();
            text.text = "";
            buttons.Add(child);

        }
        //ResetButton = GameObject.Find("Reset").GetComponent<Button>();
        //ResetButton.onClick.AddListener(ResetGame);
    }

    public void PlayerTurn(Button buttonClicked)
    {
        //Debug.Log(buttonClicked.gameObject.name);
        var TextHold = buttonClicked.gameObject.transform.GetChild(0).GetComponent<Text>();

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

        buttonClicked.enabled = false;
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
}
