using UnityEngine;

[System.Serializable]
public class MoveData
{
    public string type = "move";
    public Vector2 move;
    public string time;
}

public class LeftStickController : MonoBehaviour
{
    public OnScreenStick leftStick;
    public TCPClient tcpClient;        
    public float sendRate = 0.05f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= sendRate)
        {
            timer = 0f;
            SendMove();
        }
    }

    void SendMove()
    {
        if (leftStick == null || tcpClient == null) return;

        Vector2 move = new Vector2(leftStick.Horizontal, leftStick.Vertical);

        // 构建JSON数据
        MoveData data = new MoveData
        {
            move = move,
            time = System.DateTime.Now.ToString("HH:mm:ss")
        };

        string json = JsonUtility.ToJson(data);

        // 在每条消息后加换行符
        json += "\n";

        // 发送到PC
        tcpClient.SendJson(json);

        // 调试
        Debug.Log("Send: " + json);
    }
}