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


    private Bounds GetBounds()
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(transform.position, Vector3.one);
        Bounds b = rs[0].bounds;
        foreach (var r in rs) b.Encapsulate(r.bounds);
        return b;
    }

    private Vector3 VisualCenter => GetBounds().center;

    private void Awake()
    {
  
        transform.RotateAround(VisualCenter, flipAxis, 180f);
    }

    public void NormalizeSize(float targetSize)
    {
        Bounds b = GetBounds();
        float maxDim = Mathf.Max(b.size.x, b.size.y, b.size.z);
        if (maxDim <= 0f) return;

        float factor = targetSize / maxDim;
        if (Mathf.Approximately(factor, 1f)) return;
        ScaleAround(b.center, transform.localScale * factor);
    }

    public void SnapTo(Vector3 targetCenter)
    {
        transform.position += targetCenter - VisualCenter;
    }

    public void Flip()
    {
        if (IsMatched) return;
        transform.RotateAround(VisualCenter, flipAxis, 180f);
        IsFaceUp = !IsFaceUp;
    }

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