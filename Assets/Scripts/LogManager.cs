using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject logTextPrefab;      // Text 或 TMP_Text 预制件
    public Transform content;             // ScrollView Content
    public TMP_Text connectionStatusText;

    [Header("分类切换按钮")]
    public Button btnMove;
    public Button btnAnimation;
    public Button btnTexture;
    public Button btnHighlight;

    private string currentFilter = "all"; // 默认显示所有日志
    private List<LogEntry> logs = new List<LogEntry>();
    private const int maxLogs = 50;

    void Start()
    {
        if (btnMove != null) btnMove.onClick.AddListener(() => ChangeFilter("move"));
        if (btnAnimation != null) btnAnimation.onClick.AddListener(() => ChangeFilter("animation"));
        if (btnTexture != null) btnTexture.onClick.AddListener(() => ChangeFilter("texture"));
        if (btnHighlight != null) btnHighlight.onClick.AddListener(() => ChangeFilter("highlight"));

        UpdateLogDisplay();
        //测试日志 
        AddLog("move", "收到移动: (0.98, -0.21)"); 
        AddLog("animation", "播放动画 Walk"); 
        SetConnectionStatus(true);
    }

    public void AddLog(string type, string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        logs.Add(new LogEntry { type = type, json = message, timestamp = timestamp });

        if (logs.Count > maxLogs)
            logs.RemoveAt(0);

        UpdateLogDisplay(); // 始终刷新显示
    }

    void ChangeFilter(string type)
    {
        currentFilter = type;
        UpdateLogDisplay();
    }

    void UpdateLogDisplay()
    {
        foreach (Transform t in content)
            Destroy(t.gameObject);

        foreach (var log in logs)
        {
            if (currentFilter != "all" && log.type != currentFilter)
                continue;

            GameObject textObj = Instantiate(logTextPrefab, content);

            TMP_Text t = textObj.GetComponent<TMP_Text>();
            if (t != null)
                t.text = $"[{log.timestamp}] ({log.type}) {log.json}";
            else
            {
                Text txt = textObj.GetComponent<Text>();
                if (txt != null)
                    txt.text = $"[{log.timestamp}] ({log.type}) {log.json}";
            }
        }

        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = content.parent.GetComponent<ScrollRect>();
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    public void SetConnectionStatus(bool connected)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = connected ? "已连接" : "断开";
    }
}