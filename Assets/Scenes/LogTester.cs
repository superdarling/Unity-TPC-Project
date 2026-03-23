using UnityEngine;

public class LogTester : MonoBehaviour
{
    public LogManager logManager;

    void Update()
    {
        // 按下 T 键发送一条测试日志
        if (Input.GetKeyDown(KeyCode.T))
        {
            SendTestLog();
        }
    }

    void SendTestLog()
    {
        if (logManager != null)
        {
            string testJson = "{\"testKey\":\"testValue\"}";
            logManager.AddLog("test", testJson);
            Debug.Log("Test log sent to LogManager: " + testJson);
        }
    }
}