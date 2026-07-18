using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("翻转轴：(1,0,0) 前后翻；(0,0,1) 左右翻")]
    public Vector3 flipAxis = Vector3.right;

    [Header("卡片身份（由 GameManager 赋值）")]
    public int cardId;

    public bool IsFaceUp { get; private set; } = false;
    public bool IsMatched { get; private set; } = false;

    /// <summary>所有渲染器合起来的包围盒中心：对齐、翻面、缩放都以它为准，保证整齐</summary>
    private Vector3 VisualCenter
    {
        get
        {
            Renderer[] rs = GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) return transform.position;
            Bounds b = rs[0].bounds;
            foreach (Renderer r in rs) b.Encapsulate(r.bounds);
            return b.center;
        }
    }

    private void Awake()
    {
        // 生成瞬间原地翻面，盖住正面（玩家看不到这一过程）
        transform.RotateAround(VisualCenter, flipAxis, 180f);
    }

    /// <summary>把牌的视觉中心对齐到指定网格点（由 GameManager 调用）</summary>
    public void SnapTo(Vector3 targetCenter)
    {
        transform.position += targetCenter - VisualCenter;
    }

    /// <summary>把整张卡等比缩放到统一视觉尺寸（所有渲染器合起来的水平最大边长 = size）</summary>
    public void NormalizeSize(float size)
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return;

        Bounds b = rs[0].bounds;
        foreach (Renderer r in rs) b.Encapsulate(r.bounds);

        float maxXZ = Mathf.Max(b.size.x, b.size.z);   // 牌平放在 XOZ 面，取水平最大边
        if (maxXZ > 1e-6f)
            transform.localScale *= size / maxXZ;

        Debug.Log($"[Card] {name} 包围盒={b.size} 目标={size} 最终缩放={transform.localScale.x}");
    }

    /// <summary>瞬时原地翻面（绕视觉中心，位置不动）</summary>
    public void Flip()
    {
        if (IsMatched) return;
        transform.RotateAround(VisualCenter, flipAxis, 180f);
        IsFaceUp = !IsFaceUp;
    }

    /// <summary>配对成功：原地缩小消失</summary>
    public void Eliminate()
    {
        IsMatched = true;
        StartCoroutine(EliminateRoutine());
    }

    private IEnumerator EliminateRoutine()
    {
        Vector3 center = VisualCenter;
        Vector3 original = transform.localScale;

        float t = 0f, duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            ScaleAround(center, original * Mathf.Lerp(1f, 0f, t / duration));
            yield return null;
        }

        Destroy(gameObject);
    }

    private void ScaleAround(Vector3 center, Vector3 newScale)
    {
        Vector3 pivot = transform.position;
        float factor = newScale.x / transform.localScale.x;
        transform.position = center + (pivot - center) * factor;
        transform.localScale = newScale;
    }
}
