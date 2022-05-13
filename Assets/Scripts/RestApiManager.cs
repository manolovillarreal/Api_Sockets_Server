using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestApiManager : MonoBehaviour
{

    string Token;
    string Username;

    [SerializeField]
    private string URL;
    // Start is called before the first frame update
    void Start()
    {
        InitToken();
    }

    private void InitToken()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            //UI Event
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);
            StartCoroutine(GetPerfil());
            
        }
    }

    public void Login()
    {
        AuthData data = new AuthData();

        data.username = GameObject.Find("InputFieldUsername").GetComponent<InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<InputField>().text;

        string postdata = JsonUtility.ToJson(data);

        StartCoroutine(LoginPost(postdata));

    }

    IEnumerator LoginPost(string postData)
    {
        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url,postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: "+www.error);
        }
        else
        {
           if(www.responseCode == 200)
            {
                AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

                PlayerPrefs.SetString("token", resData.token);
                PlayerPrefs.SetString("username", resData.usuario.username);

                Debug.Log("Autenticacion exitosa!");
                SceneManager.LoadScene("Lobbie");

            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                mensaje+= "\nmsg:" +www.downloadHandler.text;
                Debug.Log(mensaje);

            }
        }
    }

    IEnumerator GetPerfil()
    {
        string url = URL + "/api/usuarios/"+ Username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                Debug.Log("Autenticacion exitosa!");
                SceneManager.LoadScene("Lobbie");
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                mensaje += "\nmsg:" + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public string msg;
    public string token;
    public UserData usuario;
}

[System.Serializable]
public class UserData
{
    public int _id;
    public string username;
    public bool estado;
}
