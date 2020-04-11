using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    enum loginState
    { 
        none = 0,
        success = 1,
        fail = 2
    }

    [SerializeField] InputField inputAccount;
    [SerializeField] InputField inputPassword;

    loginState State;
    ChatClient client = null;
    bool connectSucceed = false;

    private void Start()
    {
        State = loginState.none;
    }

    private void FixedUpdate()
    {
        if (connectSucceed)
        {
            client.Run();
            switch (State)
            {
                case loginState.none:
                    break;
                case loginState.success:
                    Debug.Log("work");
                    break;
                case loginState.fail:
                    Debug.Log("fail");
                    break;
                default:
                    break;
            }
        }
    }

    public void Login()
    {
        client = new ChatClient();
        //connectSucceed = client.Connect("34.80.167.143", 4099);
        //local
        connectSucceed = client.Connect("127.0.0.1", 4099);
        if (connectSucceed)
        {
            Debug.Log("connect");
            client.SendAccount(inputAccount.text, inputPassword.text);
        }
    }

}
