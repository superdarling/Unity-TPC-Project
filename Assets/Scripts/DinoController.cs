using UnityEngine;

public class DinoController : MonoBehaviour
{
    [Header("移动")]
    public float movespeed = 5f;
    private Vector2 moveInput;

    [Header("动画")]
    public Animator animator;
    private readonly string animParamSpeed = "Speed";

    [Header("材质/贴图")]
    public Renderer dinoRenderer;
    public Material mat1;
    public Material mat2;

    [Header("高亮")]
    public Color highlightColor = Color.yellow;
    private Color normalColor = Color.white;

    void Start()
    {
        // 自动获取 Animator
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // 自动获取 Renderer
        if (dinoRenderer == null)
            dinoRenderer = GetComponentInChildren<Renderer>();
    }

    void FixedUpdate()
    {
        MoveDino();
    }

    void MoveDino()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // 设置方向和位置
        if (move.magnitude > 0.01f)
        {
            transform.forward = move.normalized;
            transform.position += move.normalized * movespeed * Time.fixedDeltaTime;
        }

        // 更新 Animator 的 speed 参数（float）
        if (animator != null)
        {
            // magnitude 范围 0-1，用作动画速度参数
            animator.SetFloat(animParamSpeed, move.magnitude);
        }
    }

    // 设置移动输入
    public void SetMove(Vector2 move)
    {
        moveInput = move;
        //Debug.Log("收到移动: " + move);
    }

    // 播放动作（使用 Trigger）
    public void PlayAnimation(string animTrigger)
    {
        if (animator == null) return;

        // 重置所有动作 Trigger
        animator.ResetTrigger("action1");
        animator.ResetTrigger("action2");
        animator.ResetTrigger("action3");

        // 设置 Trigger 播放动作
        animator.SetTrigger(animTrigger);

        Debug.Log("触发动画: " + animTrigger);
    }

    // 切换贴图
    public void SetTexture(string type)
    {
        if (dinoRenderer == null) return;

        switch (type)
        {
            case "skin1":
                dinoRenderer.material = mat1;
                break;
            case "skin2":
                dinoRenderer.material = mat2;
                break;
        }

        Debug.Log("切换贴图: " + type);
    }

    // 高亮开关
    public void SetHighlight(bool on)
    {
        Color highlight = highlightColor;
        Color normal = normalColor;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_BaseColor"))
                r.material.SetColor("_BaseColor", on ? highlight : normal);
            else
                r.material.color = on ? highlight : normal;
        }
    }
}