using UnityEngine;
using UnityEngine.UI;

public class HighlightUI : MonoBehaviour
{
    public TCPClient tcp;
    public Toggle highlightToggle;  // UI Toggle

    void Start()
    {
        if (highlightToggle != null)
            highlightToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        string action = isOn ? "on" : "off";
        string json = "{\"type\":\"highlight\",\"action\":\"" + action + "\"}\n";
        tcp.SendJson(json);

        Debug.Log("∑¢ÀÕ∏ﬂ¡¡: " + json);
    }
}