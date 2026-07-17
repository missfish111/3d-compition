using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("六种卡片预制体（每种正面不同）")]
    public Card[] cardPrefabs = new Card[6];

    [Header("牌阵：3 行 4 列")]
    public int rows = 3;
    public int cols = 4;

    [Header("统一牌尺寸与间距")]
    public float cardSize = 2.0f;   
    public float gap = 0.5f;         
    public float tableY = 0f;          

    [Header("卡片平放角度")]
    public Vector3 cardEuler = new Vector3(-90f, 0f, 0f);

    [Header("玩法设置")]
    public float mismatchDelay = 0.8f;
    public float matchDelay = 0.3f;

    private Camera cam;
    private Card firstCard;
    private bool comparing;
    private int matchedPairs;
    private const int totalPairs = 6;
    private int moves;

    private void Start()
    {
        cam = Camera.main;
        GenerateCards();
    }

    private void GenerateCards()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        float spacingX = cardSize + gap;
        float spacingZ = cardSize + gap;
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
                card.NormalizeSize(cardSize);
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
                Card card = hit.collider.GetComponentInParent<Card>();
                if (card != null)
                    OnCardClicked(card);
            }
        }
    }

    private void OnCardClicked(Card card)
    {
        if (card.IsFaceUp || card.IsMatched) return;

        card.Flip();

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
                VictoryUI.Show(moves); 
            }
        }
        else
        {
            yield return new WaitForSeconds(mismatchDelay);
            a.Flip();
            b.Flip();
        }

        comparing = false;
    }
}