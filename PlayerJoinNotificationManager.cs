using UnityEngine;
using Photon.Pun;
using TMPro; // Use this if you're using TextMeshPro
using System.Collections;

public class PlayerJoinNotificationManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI PlayerJoinNotificationText;
    public float displayTime = 2.0f; // How long the notification will be displayed

    void Start()
    {
        // Optional: Display a welcome message or leave this out
        ShowNotification("");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        // Call ShowNotification when a new player joins the room
        ShowNotification(newPlayer.NickName + " has joined the game!");
    }

    public void ShowNotification(string message)
    {
        PlayerJoinNotificationText.text = message;
        StartCoroutine(DisappearAfterTime(displayTime));
    }

    IEnumerator DisappearAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerJoinNotificationText.text = "";
    }
}
