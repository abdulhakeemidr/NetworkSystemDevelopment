using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject submitButton, joinGameRoomButton, joinGameRoomObserveButton,
        ticTacToeGame;
    GameObject usernameInput, passwordInput;
    GameObject createToggle, loginToggle;
    GameObject stateInfoText, playerNameText, observerListText;
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
            else if (go.name == "JoinGameRoomObserveButton")
            {
                joinGameRoomObserveButton = go;
            }
            else if (go.name == "TicTacToeGame")
            {
                ticTacToeGame = go;
            }
            else if (go.name == "StateInfoText")
            {
                stateInfoText = go;
            }
            else if (go.name == "PlayerNameText")
            {
                playerNameText = go;
            }
            else if (go.name == "ObserverListText")
            {
                observerListText = go;
            }
            else if (go.name == "ChatBox")
            {
                chatBoxSystem = go;
            }
        }
        chatInputField = chatBoxSystem.transform.GetChild(1).gameObject.GetComponent<InputField>();

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);
        joinGameRoomObserveButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomObserveButtonPressed);
        ticTacToeGame.GetComponent<ButtonChange>().gamePlayButton.AddListener(TicTacToeSquareTurnPlayed);

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
                Debug.Log("Updating chat to others");
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

    public void JoinGameRoomObserveButtonPressed()
    {
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinAsObserver + "");
        //ChangeState(GameStates.TicTacToe);
    }

    public void TicTacToeSquareTurnPlayed()
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
        joinGameRoomObserveButton.SetActive(false);
        submitButton.SetActive(false);
        ticTacToeGame.SetActive(false);
        usernameInput.SetActive(false);
        passwordInput.SetActive(false);
        createToggle.SetActive(false);
        loginToggle.SetActive(false);
        stateInfoText.SetActive(false);
        playerNameText.SetActive(false);
        observerListText.SetActive(false);
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
            joinGameRoomObserveButton.SetActive(true);
            playerNameText.SetActive(true);
        }
        else if (newState == GameStates.WaitingInQueueForOtherPlayer)
        {
            stateInfoText.SetActive(true);
            stateInfoText.GetComponent<Text>().text = "Waiting for other Players....";
            playerNameText.SetActive(true);
        }
        else if (newState == GameStates.TicTacToe)
        {
            ticTacToeGame.SetActive(true);
            playerNameText.SetActive(true);
            chatBoxSystem.SetActive(true);
            //observerListText.SetActive(true);
        }
        else if (newState == GameStates.TicTacToeObserve)
        {
            playerNameText.SetActive(true);
            chatBoxSystem.SetActive(true);
            //observerListText.SetActive(true);
            stateInfoText.SetActive(true);
            stateInfoText.GetComponent<Text>().text = "Joined As Observer";
        }
    }
}

public enum GameStates
{
    LoginMenu,
    MainMenu,
    WaitingInQueueForOtherPlayer,
    TicTacToe,
    TicTacToeObserve
}