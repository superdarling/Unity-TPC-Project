using UnityEngine;

public class TextureUI : MonoBehaviour
{
    public TCPClient tcp;

    public void SetSkin1()
    {
        Send("skin1");
    }

    public void SetSkin2()
    {
        Send("skin2");
    }

    void Send(string skin)
    {
        string json = "{\"type\":\"texture\",\"action\":\"" + skin + "\"}\n";
        tcp.SendJson(json);

        Debug.Log("·¢ËÍÌùÍ¼: " + json);
    }
}