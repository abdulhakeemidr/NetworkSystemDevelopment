using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5497;
    byte error;
    bool isConnected = false;
    int ourClientID;

    GameObject gameSystemManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
            {
                gameSystemManager = go;
            }
        }

        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + "Abdulhakeem" + "," + "abc123");
        //}
        //else if (Input.GetKeyDown(KeyCode.L))
        //{
        //    SendMessageToHost(ClientToServerSignifiers.LoginAccount + "," + "Abdulhakeem" + "," + "abc123");
        //}

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.11", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);
        //string message = csv[1];

        switch (signifier)
        {
            case ServertoClientSignifiers.CreateAccountFail:
                break;

            case ServertoClientSignifiers.CreateAccountSuccess:
                Debug.Log("CreateAccountSuccess");
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
                break;

            case ServertoClientSignifiers.LoginAccountFail:
                Debug.Log("LoginFail");
                break;

            case ServertoClientSignifiers.LoginAccountSuccess:
                Debug.Log("LoginSuccess");
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
                break;

            case ServertoClientSignifiers.OpponentPlay:
                Debug.Log("Opponent Play");
                break;

            case ServertoClientSignifiers.GameStart:
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                break;

            case ServertoClientSignifiers.chatBoxMessageReceive:
                GameObject chatBox = gameSystemManager.GetComponent<GameSystemManager>().chatBoxSystem;
                Debug.Log(csv[1]);
                int msgType = int.Parse(csv[2]);

                if(msgType == ServertoClientSignifiers.chatReceivedTypeObserver)
                {
                    chatBox.GetComponent<ChatBoxSystem>().SendMessageToLocalChatBox(csv[1], MessageType.observerMessage);
                }
                else if(msgType == ServertoClientSignifiers.chatReceivedTypePlayer)
                {
                    chatBox.GetComponent<ChatBoxSystem>().SendMessageToLocalChatBox(csv[1], MessageType.otherPlayerMessage);
                }
                else
                {
                    chatBox.GetComponent<ChatBoxSystem>().SendMessageToLocalChatBox(csv[1], MessageType.otherPlayerMessage);
                }

                break;

            case ServertoClientSignifiers.ObserverJoined:
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToeObserve);
                break;
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }

}

static public class ClientToServerSignifiers
{
    public const int CreateAccount = 1;
    public const int LoginAccount = 2;
    public const int JoinQueueForGameRoom = 3;
    public const int TicTacToeShapeSelectPlay = 4;
    public const int ChatBoxMessageSend = 5;
    public const int JoinAsObserver = 6;
}

static public class ServertoClientSignifiers
{
    public const int CreateAccountFail = 1;
    public const int LoginAccountFail = 2;

    public const int CreateAccountSuccess = 3;
    public const int LoginAccountSuccess = 4;

    public const int OpponentPlay = 5;
    public const int GameStart = 6;
    public const int chatBoxMessageReceive = 7;

    public const int ObserverJoined = 8;
    public const int chatReceivedTypePlayer = 9;
    public const int chatReceivedTypeObserver = 10;
    public const int chatReceivedTypeServer = 11;
}