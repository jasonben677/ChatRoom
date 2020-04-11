using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ChatRoomManager : MonoBehaviour
{

    public int status = 3;

    public InputField IP;
    public InputField Name;
    public InputField Speak;

    public ScrollRect scroll;

    public Text messageSimple;

    private GameObject page_IP;
    private GameObject page_Name;
    private GameObject page_Speak;

    ChatClient client = null;

    bool connectSucceed = false;
    float timer = 0.0f;

    private void Awake()
    {
        page_IP = transform.GetChild(1).gameObject;
        page_Name = transform.GetChild(2).gameObject;
        page_Speak = transform.GetChild(3).gameObject;

    }

    private void Update()
    {
        if (connectSucceed == false)
        {
            return;
        }

        RunStatus();
    }

    /// <summary>
    /// 跑狀態
    /// </summary>
    private void RunStatus()
    {
        if (status == 0)
        {
            if (timer <= 0.01)
            {
                client.Run();
                //Debug.Log("run");
                timer = 1.5f;
            }
            else
            {
                timer -= Time.deltaTime;
            }

        }
        else if (status == 1)
        {
            client.SendBroadcast(Speak.text);
            Speak.text = "";
            status = 0;
        }
        else if (status == 3)
        {
            page_Name.SetActive(false);
            page_Speak.SetActive(true);
            client.SendName(Name.text);
            status = 0;
        }
    }

    /// <summary>
    /// 輸入ip
    /// </summary>
    public void SendIP()
    {
        page_IP.SetActive(false);
        page_Name.SetActive(true);
    }

    /// <summary>
    /// 輸入名稱，連線
    /// </summary>
    public void SendName()
    {
        client = new ChatClient();
        client.RoomManager = this;
        connectSucceed = client.Connect(IP.text, 4099);
    }

    /// <summary>
    /// 傳送訊息
    /// </summary>
    public void SendMessage()
    {
        client.messageProcess = MessageShow;
        status = 1;
    }

    /// <summary>
    /// 顯示訊息出來
    /// </summary>
    public void MessageShow(string _message)
    {
        RectTransform parentObj = scroll.content;
        Text copt = Instantiate(messageSimple, parentObj);
        copt.text = _message;
        copt.transform.localScale = Vector3.one;
        copt.gameObject.SetActive(true);
    }

    public void LoginCheck(string _bool)
    {
        if (_bool == "yes")
        {
            status = 3;
        }
        else
        { 
            
        }
    }
}
