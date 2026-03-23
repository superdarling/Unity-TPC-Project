using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[Serializable]
public class ControlCommand
{
    public string type;
    public string action;
    public Vector2 move;
}

[Serializable]
public class LogMessage
{
    public string type;
    public string message;
}

public class TCPServer : MonoBehaviour
{
    public int port = 9000;
    public DinoController dino;

    private TcpListener server;
    private Thread thread;
    private bool running = true;

    private TcpClient client;
    private NetworkStream stream;
    private StringBuilder receiveBuffer = new StringBuilder();

    void Start()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();

        thread = new Thread(Listen);
        thread.IsBackground = true;
        thread.Start();

        Debug.Log("Server Started");
    }

    void Listen()
    {
        while (running)
        {
            if (!server.Pending())
            {
                Thread.Sleep(10);
                continue;
            }

            client = server.AcceptTcpClient();
            stream = client.GetStream();

            SendLog("system", "客户端已连接: " + client.Client.RemoteEndPoint);

            byte[] buffer = new byte[1024];
            receiveBuffer.Clear();

            while (client.Connected)
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

                string[] lines = receiveBuffer.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                receiveBuffer.Clear();
                if (!chunk.EndsWith("\n") && lines.Length > 0)
                {
                    receiveBuffer.Append(lines[lines.Length - 1]);
                    Array.Resize(ref lines, lines.Length - 1);
                }

                foreach (var line in lines)
                {
                    try
                    {
                        ControlCommand cmd = JsonUtility.FromJson<ControlCommand>(line);
                        if (cmd == null) continue;

                        // 传入原始 JSON，执行命令后返回给客户端显示
                        MainThreadExecutor.RunOnMainThread(() =>
                        {
                            ProcessCommand(cmd, line);
                        });
                    }
                    catch (Exception ex)
                    {
                        SendLog("error", $"JSON解析失败: {ex.Message} → {line}");
                    }
                }
            }

            client.Close();
            SendLog("system", "客户端已断开");
        }
    }

    // 修改 ProcessCommand 接收原始 JSON
    void ProcessCommand(ControlCommand cmd, string originalJson)
    {
        if (dino != null)
        {
            switch (cmd.type)
            {
                case "move":
                    dino.SetMove(cmd.move);
                    break;
                case "animation":
                    dino.PlayAnimation(cmd.action);
                    break;
                case "texture":
                    dino.SetTexture(cmd.action);
                    break;
                case "highlight":
                    dino.SetHighlight(cmd.action == "on");
                    break;
            }
        }

        // 执行完动作后，把原始 JSON 发回客户端，用于手机端日志显示
        SendLog(cmd.type, originalJson);
    }

    void SendLog(string type, string message)
    {
        if (client == null || !client.Connected) return;

        LogMessage log = new LogMessage { type = type, message = message };
        string json = JsonUtility.ToJson(log) + "\n";
        byte[] data = Encoding.UTF8.GetBytes(json);

        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            // 客户端可能断开
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        server?.Stop();
        if (thread != null && thread.IsAlive) thread.Abort();
    }
}