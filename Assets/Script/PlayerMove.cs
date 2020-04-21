using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public bool useServer = false;
    public FriendManager Friend;
    Rigidbody myRigi;
    float times = 0;
    private void Awake()
    {
        myRigi = transform.GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        times += Time.fixedDeltaTime;
        float hmove = Input.GetAxis("Horizontal");
        float vmove = Input.GetAxis("Vertical");
        Vector3 vec = transform.forward * vmove + transform.right * hmove;
        myRigi.position += vec * 10f* Time.fixedDeltaTime;

        // 1s傳4次
        if (times >= 0.25f && useServer)
        {
            LoginManager.instance.SendPos(transform.position);
            //add delegate
            times = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
    }

}
