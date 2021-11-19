using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBoxSystem : MonoBehaviour
{
    public int maxMessageList = 25;
    
    public GameObject chatPanel, textObject;
    public InputField chatInputField;

    public Color yourMessageColor = Color.white;
    public Color otherPlayerMessageColor = Color.red;
    public Color observerMessageColor = Color.green;
    
    [SerializeField]
    List<Message> messages = new List<Message>();


    private void Start()
    {
        
    }

    public void SendMessageToLocalChatBox(string text, MessageType messageType = MessageType.playerMessage)
    {
        Debug.Log("Added to chat box");
        if (messages.Count >= maxMessageList)
        {
            Destroy(messages[0].textObject.gameObject);
            messages.Remove(messages[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColour(messageType);

        messages.Add(newMessage);
    }

    Color MessageTypeColour(MessageType type)
    {
        Color color = otherPlayerMessageColor;
        switch(type)
        {
            case MessageType.playerMessage:
                color = yourMessageColor;
                break;

            case MessageType.otherPlayerMessage:
                color = otherPlayerMessageColor;
                break;

            case MessageType.observerMessage:
                color = observerMessageColor;
                break;
        }
        return color;
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}

public enum MessageType
{
    playerMessage,
    otherPlayerMessage,
    observerMessage
}