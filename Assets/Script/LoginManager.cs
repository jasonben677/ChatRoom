using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    static public LoginManager instance;
    public ChatClient client = null;
    bool connectSucceed = false;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        Button button = GameObject.Find("LoginButton").GetComponent<Button>();
        button.onClick.AddListener(() => Login());
    }

    private void FixedUpdate()
    {
        if (connectSucceed)
        {
            client.Run();
        }
    }

    private void OnDisable()
    {
        if (connectSucceed)
        {
            client.tranmitter.Close();
            connectSucceed = false;
        }

    }
    public void SendPos(Vector3 pos)
    {
        float newX = (float)System.Math.Round(pos.x, 2);
        float newY = (float)System.Math.Round(pos.y, 2);
        float newZ = (float)System.Math.Round(pos.z, 2);

        client.SendPos(newX, newY, newZ);
    }
    public void Login()
    {
        client = new ChatClient();

        //虛擬機
        //connectSucceed = client.Connect("34.80.167.143", 4099);

        //local
        connectSucceed = client.Connect("127.0.0.1", 4099);
        if (connectSucceed)
        {
            Debug.Log("connect");
            string account = GameObject.Find("Account").GetComponent<InputField>().text;
            string password = GameObject.Find("Password").GetComponent<InputField>().text;
            client.SendAccount(account, password);
            client.tranmitter.Register(0,EnterGameScence);
        }
    }

    private void EnterGameScence(Common.Tranmitter _tran, TestDll.Message03 _message)
    {
        if(_message.success)
            SceneManager.LoadScene("GamePlay02");
        //SceneManager.LoadSceneAsync("GamePlay").isDone;
    }

}
