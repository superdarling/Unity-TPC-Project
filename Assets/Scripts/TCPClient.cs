using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class TCPClient : MonoBehaviour
{
    public string serverIP = "10.69.52.103";
    public int port = 9000;

    public LogManager logManager;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private StringBuilder receiveBuffer = new StringBuilder();

    public bool IsConnected => client != null && client.Connected;

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);
            stream = client.GetStream();

            Debug.Log("Connected to server");

            if (logManager != null)
            {
                MainThreadExecutor.RunOnMainThread(() =>
                {
                    logManager.SetConnectionStatus(true);
                    logManager.AddLog("system", $"已连接到服务器 {serverIP}:{port}");
                });
            }

            // 开启接收线程
            receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
            receiveThread.Start();
        }
        catch (Exception ex)
        {
            Debug.Log("Connection failed: " + ex.Message);
            if (logManager != null)
            {
                MainThreadExecutor.RunOnMainThread(() =>
                {
                    logManager.SetConnectionStatus(false);
                    logManager.AddLog("error", $"连接服务器失败: {ex.Message}");
                });
            }
        }
    }

    private void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (IsConnected)
        {
            try
            {
                if (!stream.DataAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                int length = stream.Read(buffer, 0, buffer.Length);
                if (length == 0) break;

                string chunk = Encoding.UTF8.GetString(buffer, 0, length);
                receiveBuffer.Append(chunk);

                // 按换行拆分，每条完整消息
                string[] lines = receiveBuffer.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                receiveBuffer.Clear();
                // 如果 chunk 最后不以换行结尾，保留最后不完整的部分
                if (!chunk.EndsWith("\n") && lines.Length > 0)
                {
                    receiveBuffer.Append(lines[lines.Length - 1]);
                    Array.Resize(ref lines, lines.Length - 1);
                }

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                        continue;

                    string timestamp = DateTime.Now.ToString("HH:mm:ss");

                    string type = "recv"; // 默认类型
                    string message = trimmedLine;

                    // 尝试解析 JSON 中的 type 字段
                    try
                    {
                        var jsonObj = JsonUtility.FromJson<TempLogMessage>(trimmedLine);
                        if (jsonObj != null && !string.IsNullOrEmpty(jsonObj.type))
                            type = jsonObj.type;
                    }
                    catch
                    {
                        // 解析失败仍然显示原始内容，type = "recv"
                    }

                    string logText = $"[{timestamp}] {message}";

                    // 显示到 LogManager
                    MainThreadExecutor.RunOnMainThread(() =>
                    {
                        if (logManager != null)
                            logManager.AddLog(type, logText);
                    });
                }
            }
            catch
            {
                break;
            }
        }

        // 断开连接
        MainThreadExecutor.RunOnMainThread(() =>
        {
            if (logManager != null)
            {
                logManager.SetConnectionStatus(false);
                logManager.AddLog("system", "服务器断开或连接中断");
            }
        });
    }

    // 临时类用于解析 type 字段
    [Serializable]
    private class TempLogMessage
    {
        public string type;
    }

    public void SendJson(string json)
    {
        if (!IsConnected)
        {
            MainThreadExecutor.RunOnMainThread(() =>
            {
                if (logManager != null)
                    logManager.AddLog("error", "发送失败：未连接到服务器");
            });
            Debug.Log("Not connected");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            stream.Write(data, 0, data.Length);

            MainThreadExecutor.RunOnMainThread(() =>
            {
                if (logManager != null)
                    logManager.AddLog("send", json);
            });
        }
        catch (Exception ex)
        {
            MainThreadExecutor.RunOnMainThread(() =>
            {
                if (logManager != null)
                    logManager.AddLog("error", $"发送失败: {ex.Message}");
            });
        }
    }

    private void OnApplicationQuit()
    {
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();

        if (logManager != null)
        {
            MainThreadExecutor.RunOnMainThread(() =>
            {
                logManager.SetConnectionStatus(false);
                logManager.AddLog("system", "客户端已断开");
            });
        }
    }
}