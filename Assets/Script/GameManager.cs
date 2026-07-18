using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("六种卡片预制体（每种正面不同）")]
    public Card[] cardPrefabs = new Card[6];

    [Header("牌阵布局：3 行 4 列，铺在水平面上")]
    public int rows = 3;
    public int cols = 4;
    public float spacingX = 3.8f;   // 列间距
    public float spacingZ = 4.4f;   // 行间距
    public float tableY = 0f;       // 牌阵所在高度

    [Header("卡片平放角度")]
    public Vector3 cardEuler = new Vector3(-90f, 0f, 0f);

    [Header("统一卡面尺寸（牌板水平方向最大边长）")]
    public float cardSize = 20f;   // 牌板网格含离群顶点，数值需远大于视觉尺寸

    [Header("玩法设置")]
    public float mismatchDelay = 0.8f;   // 配对失败后盖回前的展示时间
    public float matchDelay = 0.3f;      // 配对成功后消除前的停顿

    private Camera cam;
    private Card firstCard;
    private bool comparing;              // 比对期间锁定输入
    private int matchedPairs;
    private const int totalPairs = 6;
    private int moves;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        // 先展示开场介绍（嫘祖文化与玩法），点击"开始祭祀"后才生成牌阵
        OpeningUI.Show();
    }

    /// <summary>开场 UI 的"开始祭祀"按钮回调</summary>
    public void StartGame()
    {
        OpeningUI.Hide();
        GenerateCards();
    }

    /// <summary>重新开始（胜利 UI"返回游览"后调用）</summary>
    public void RestartGame()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        firstCard = null;
        comparing = false;
        matchedPairs = 0;
        moves = 0;
        GenerateCards();
    }

    /// <summary>生成 12 张牌（6 种各 2 张），洗牌后按 3×4 整齐排布</summary>
    private void GenerateCards()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Fisher-Yates 洗牌
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        float offsetX = (cols - 1) * spacingX * 0.5f;
        float offsetZ = (rows - 1) * spacingZ * 0.5f;
        Quaternion spawnRot = Quaternion.Euler(cardEuler);

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int id = ids[index++];
                Vector3 gridPos = new Vector3(
                    c * spacingX - offsetX,
                    tableY,
                    r * spacingZ - offsetZ);

                Card card = Instantiate(cardPrefabs[id], gridPos, spawnRot, transform);
                card.cardId = id;

                // 先统一大小，再对齐：缩放之后包围盒才准确
                card.NormalizeSize(cardSize);
                // 视觉中心对齐网格点（含高度 tableY），消除模型枢轴差异，整齐且共面
                card.SnapTo(gridPos);
            }
        }
    }

    private void Update()
    {
        if (comparing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 碰撞体在子物体上，Card 脚本在根节点，用 GetComponentInParent 查找
                Card card = hit.collider.GetComponentInParent<Card>();
                if (card != null)
                    OnCardClicked(card);
            }
        }
    }

    private void OnCardClicked(Card card)
    {
        if (card.IsFaceUp || card.IsMatched) return;

        card.Flip();   // 瞬时原地翻面

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            moves++;
            StartCoroutine(CompareRoutine(firstCard, card));
            firstCard = null;
        }
    }

    /// <summary>比对两张翻开的卡：相同消除，不同盖回</summary>
    private IEnumerator CompareRoutine(Card a, Card b)
    {
        comparing = true;

        if (a.cardId == b.cardId)
        {
            yield return new WaitForSeconds(matchDelay);
            a.Eliminate();
            b.Eliminate();
            matchedPairs++;

            if (matchedPairs >= totalPairs)
            {
                Debug.Log($"全部消除！共用了 {moves} 步");
                VictoryUI.Show(moves);   // 弹出可滑动的嫘祖祭祀文化介绍界面
            }
        }
        else
        {
            yield return new WaitForSeconds(mismatchDelay);
            a.Flip();   // 瞬时盖回
            b.Flip();
        }

        comparing = false;
    }
}
