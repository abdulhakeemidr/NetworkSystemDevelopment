using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBoxSystem : MonoBehaviour
{
    public int maxMessageList = 25;
    
    public GameObject chatPanel, textObject;
    public InputField chatInputField;
    
    [SerializeField]
    List<Message> messages = new List<Message>();


    private void Start()
    {
        
    }

    private void Update()
    {
        if(chatInputField.text != "")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(chatInputField.text);
                chatInputField.text = "";
            }
        }
        else
        {
            if(!chatInputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatInputField.ActivateInputField();
            }
        }
        if (!chatInputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("You pressed space bar");
            }
        }
    }

    public void SendMessageToChat(string text)
    {
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

        messages.Add(newMessage);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}
