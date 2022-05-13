using Firesplash.UnityAssets.SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WebSocketManager : MonoBehaviour
{
    private SocketIOCommunicator Socket;

    // Start is called before the first frame update

    void Awake()
    {
        //Socket = GameObject.Find("SocketIO").GetComponent<SocketIOCommunicator>(); 
        Socket = gameObject.GetComponent<SocketIOCommunicator>();
    }

    void Start()
    {

        Socket.Instance.On("connect", (string data) =>
         {
             Debug.Log("LOCAL: Conectado al servidor");

         });
        Socket.Instance.On("disconnect", (string payload) => {
            if (payload.Equals("io server disconnect"))
            {
                Debug.Log("Disconnected from server.");
            }
            else
            {
                Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }
        });
        Socket.Instance.On("friendRequest", (string payload) =>
        {
            JSONData data = JsonUtility.FromJson<JSONData>(payload);

            Debug.Log("Solicitud de amistad de " + data.username);

            SendFriendResponse(data.username, true);
        });

        Socket.Instance.On("friendResponse", (string payload) =>
        {
            JSONData data = JsonUtility.FromJson<JSONData>(payload);

            if (data.ok)
            {
                Debug.Log(data.msg);
            }
            else
            {
                Debug.Log(data.msg);
            }
            
        });

        Socket.Instance.On("friendNotOnline", (string payload) =>
        {
            JSONData data = JsonUtility.FromJson<JSONData>(payload);

            Debug.Log( data.username+ "no esta en linea, intente mas tarde");
        });
        Socket.Instance.On("friendOnline", (string payload) =>
        {
            JSONData data = JsonUtility.FromJson<JSONData>(payload);

            Debug.Log("Tu amigo "+data.username + "se ha contecado");
        });



        string token = PlayerPrefs.GetString("token");
        string username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("No hay token");
            SceneManager.LoadScene("Main");
            return;
        }

        SIOAuthPayload auth = new SIOAuthPayload();
        auth.AddElement("username", username); //The server will access this using socket.handshake.auth.id
        auth.AddElement("token", token); //The server will access this using socket.handshake.auth.token
        Socket.Instance.Connect(auth);

    }


    public void FindMatch()
    {
        Debug.Log("LOCAL: FIND MATCH");
        Socket.Instance.Emit("findMatch");
    }
    public void SendFriendRequest( )
    {

        JSONData data = new JSONData();
        data.username = GameObject.Find("InputFriendUsername").GetComponent<InputField>().text;

        Socket.Instance.Emit("friendRequest", JsonUtility.ToJson(data),false);
    }

    public void SendFriendResponse(string username, bool response)
    {

        JSONData data = new JSONData();
        data.username = username;
        data.response = response;
        Socket.Instance.Emit("friendResponse", JsonUtility.ToJson(data), false);
    }
    public void ReceiveFriendRequest()
    {

    }

    public void FriendNotOnline()
    {

    }
   
}

public class JSONData
{
   public string username; 
   public bool ok; 
   public string msg;
   public bool response;
}
