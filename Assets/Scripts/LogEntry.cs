using System;
using UnityEngine;

[Serializable]
public class LogEntry
{
    public string type;       // move / animation / texture / highlight
    public string json;       // 原始 JSON
    public string timestamp;  // 时间字符串
}