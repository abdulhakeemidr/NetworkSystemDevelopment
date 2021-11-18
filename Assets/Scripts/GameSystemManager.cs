using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject submitButton, joinGameRoomButton, ticTacToeSquareButton;
    GameObject usernameInput, passwordInput;
    GameObject createToggle, loginToggle;
    GameObject waitingText, playerNameText;
    GameObject networkClient;
    [HideInInspector]
    public GameObject chatBoxSystem;
    InputField chatInputField;

    string currentAccountName;

    private void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "UsernameInputField")
            {
                usernameInput = go;
            }
            else if (go.name == "PasswordInputField")
            {
                passwordInput = go;
            }
            else if (go.name == "SubmitButton")
            {
                submitButton = go;
            }
            else if (go.name == "CreateToggle")
            {
                createToggle = go;
            }
            else if (go.name == "LoginToggle")
            {
                loginToggle = go;
            }
            else if (go.name == "NetworkClient")
            {
                networkClient = go;
            }
            else if (go.name == "JoinGameRoomButton")
            {
                joinGameRoomButton = go;
            }
            else if (go.name == "TicTacToeSquareButton")
            {
                ticTacToeSquareButton = go;
            }
            else if (go.name == "WaitingText")
            {
                waitingText = go;
            }
            else if (go.name == "PlayerNameText")
            {
                playerNameText = go;
            }
            else if (go.name == "ChatBox")
            {
                chatBoxSystem = go;
            }
        }
        chatInputField = chatBoxSystem.transform.GetChild(1).gameObject.GetComponent<InputField>();

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);
        ticTacToeSquareButton.GetComponent<Button>().onClick.AddListener(TicTacToeSquareButtonPressed);

        ChangeState(GameStates.LoginMenu);
    }

    private void Update()
    {
        ChatBoxSystem chatBox = chatBoxSystem.GetComponent<ChatBoxSystem>();
        if (chatInputField.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.SendMessageToLocalChatBox(currentAccountName + ": " + chatInputField.text);
                ChatBoxMessageSend();
                chatInputField.text = "";
            }
        }
        else
        {
            if (!chatInputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatInputField.ActivateInputField();
            }
        }
        if (!chatInputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                chatBox.SendMessageToLocalChatBox("You pressed space bar");
            }
        }
        //ChatBoxMessageSend();
    }

    public void SubmitButtonPressed()
    {
        string username = usernameInput.GetComponent<InputField>().text;
        string password = passwordInput.GetComponent<InputField>().text;

        string msg;

        if (createToggle.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.CreateAccount + "," + username + "," + password;
        else
            msg = ClientToServerSignifiers.LoginAccount + "," + username + "," + password;

        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        playerNameText.GetComponent<Text>().text = "Profile: " + username;
        currentAccountName = username;
        Debug.Log(msg);
    }

    public void JoinGameRoomButtonPressed()
    {
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "");
        ChangeState(GameStates.WaitingInQueueForOtherPlayer);
    }

    public void TicTacToeSquareButtonPressed()
    {
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToeShapeSelectPlay + "");
        ChangeState(GameStates.TicTacToe);
    }

    public void ChatBoxMessageSend()
    {
        string chatMsg = chatInputField.GetComponent<InputField>().text;
        string msg;

        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    Debug.Log(chatMsg);
        //    msg = ClientToServerSignifiers.ChatBoxMessageSend + "," + chatMsg;
        //    networkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        //}

        Debug.Log(chatMsg);
        msg = ClientToServerSignifiers.ChatBoxMessageSend + "," + chatMsg;
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
    }

    public void ChangeState(GameStates newState)
    {
        joinGameRoomButton.SetActive(false);
        submitButton.SetActive(false);
        ticTacToeSquareButton.SetActive(false);
        usernameInput.SetActive(false);
        passwordInput.SetActive(false);
        createToggle.SetActive(false);
        loginToggle.SetActive(false);
        waitingText.SetActive(false);
        playerNameText.SetActive(false);
        chatBoxSystem.SetActive(false);

        if (newState == GameStates.LoginMenu)
        {
            submitButton.SetActive(true);
            usernameInput.SetActive(true);
            passwordInput.SetActive(true);
            createToggle.SetActive(true);
            loginToggle.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            joinGameRoomButton.SetActive(true);
            playerNameText.SetActive(true);
        }
        else if (newState == GameStates.WaitingInQueueForOtherPlayer)
        {
            waitingText.SetActive(true);
            playerNameText.SetActive(true);
        }
        else if (newState == GameStates.TicTacToe)
        {
            ticTacToeSquareButton.SetActive(true);
            playerNameText.SetActive(true);
            chatBoxSystem.SetActive(true);
        }
    }
}

public enum GameStates
{
    LoginMenu,
    MainMenu,
    WaitingInQueueForOtherPlayer,
    TicTacToe
}