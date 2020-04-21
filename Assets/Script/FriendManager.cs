using UnityEngine;

public class FriendManager : MonoBehaviour
{
    GameObject friend;
    private void Awake()
    {
        friend = transform.GetChild(0).gameObject;
    }

    public void UpdateFirend(Common.Message _player)
    {
        if (_player == null) return;
        Debug.Log("receive");
        friend.SetActive(true);
        //friend.transform.position = new Vector3 (_player.position.x, _player.position.y, _player.position.z);
    }

}
