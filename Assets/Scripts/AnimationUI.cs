using UnityEngine;

public class AnimationUI : MonoBehaviour
{
    public TCPClient tcp;

    public void PlayAction1()
    {
        Send("action1");
    }

    public void PlayAction2()
    {
        Send("action2");
    }

    public void PlayAction3()
    {
        Send("action3");
    }

    void Send(string action)
    {
        string json = "{\"type\":\"animation\",\"action\":\"" + action + "\"}\n";
        tcp.SendJson(json);

        Debug.Log("·¢ËÍ¶¯»­: " + json);
    }
}