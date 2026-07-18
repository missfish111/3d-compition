using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开场介绍界面：游戏启动时弹出，仿胜利 UI 的可滑动版式。
/// 介绍嫘祖文化背景与玩法，结尾设"开始祭祀"按钮，点击后开始游戏。
/// 界面全部由代码动态构建，无需在场景中预先摆放 UI。
/// </summary>
public static class OpeningUI
{
    private static GameObject root;
    private static Font uiFont;

    private const string intro =
        "嫘祖，黄帝之妃，始教民育蚕、缫丝织帛，被尊为“先蚕娘娘”。后世以祭祀大典缅怀其功德。本游戏以翻牌之礼重温六步仪程——六对卡面，与祭祀六礼一一对应。";

    private const string howToPlay =
        "<b><color=#8B2500>玩法</color></b>\n" +
        "点击卡牌将其翻开，连续翻开两张相同的卡面即可消除。在尽量少的步数内消除全部 12 张卡牌（6 对），即可完成祭祀大典。";

    private static readonly string[] stepNums   = { "第一步", "第二步", "第三步", "第四步", "第五步", "第六步" };
    private static readonly string[] stepNames  = { "鸣炮", "巡游", "焚香", "拜礼", "贡品", "乐舞" };
    private static readonly string[] stepTitles = { "启幕", "迎神", "通神", "致敬", "献祭", "颂德" };
    private static readonly string[] stepDescs  =
    {
        "礼炮鸣响，大典启幕。卡面之上，且听第一声庄严。",
        "圣像巡游，万民恭迎。翻牌之间，迎请先蚕娘娘降临。",
        "一炷心香，上达天宇。青烟所至，虔诚可通神明。",
        "肃立行礼，缅怀功德。俯仰之间，是千年的感念。",
        "丝绸五谷，敬献案前。以衣食丰足，告慰先祖。",
        "礼乐歌舞，颂扬功绩。待你集齐六对卡面，大典方成。"
    };

    /// <summary>游戏启动时由 GameManager 调用</summary>
    public static void Show()
    {
        if (root == null)
            root = Build();
        else
            root.SetActive(true);
    }

    public static void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private static GameObject Build()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null)
            uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 场景里没有 EventSystem 时补一个，否则按钮和滑动无效
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 画布（盖在一切之上）
        GameObject canvasGo = NewUI("OpeningCanvas", null);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();

        // 半透明背景
        GameObject backdrop = NewUI("Backdrop", canvasGo.transform);
        Image bg = backdrop.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.72f);
        SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // 主窗口（宣纸色）
        GameObject window = NewUI("Window", canvasGo.transform);
        Image winImg = window.AddComponent<Image>();
        winImg.color = new Color(0.965f, 0.93f, 0.86f);
        RectTransform winRect = (RectTransform)window.transform;
        winRect.anchorMin = winRect.anchorMax = new Vector2(0.5f, 0.5f);
        winRect.sizeDelta = new Vector2(820f, 980f);

        // 标题
        Text title = MakeText(window.transform, "嫘 祖 祭 祀", 54,
            new Color(0.545f, 0.145f, 0f), FontStyle.Bold, TextAnchor.MiddleCenter);
        SetRect(title.gameObject, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(30f, -105f), new Vector2(-30f, -30f));

        // 副标题
        Text sub = MakeText(window.transform, "先蚕娘娘 · 育蚕缫丝 · 衣被天下", 26,
            new Color(0.45f, 0.36f, 0.28f), FontStyle.Normal, TextAnchor.MiddleCenter);
        SetRect(sub.gameObject, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(30f, -165f), new Vector2(-30f, -115f));

        // 滚动视口（带遮罩，超出部分隐藏）
        GameObject viewport = NewUI("Viewport", window.transform);
        viewport.AddComponent<Image>().color = Color.white;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        SetRect(viewport, Vector2.zero, Vector2.one,
            new Vector2(35f, 40f), new Vector2(-60f, -185f));

        // 内容容器：垂直布局 + 自适应高度
        GameObject content = NewUI("Content", viewport.transform);
        RectTransform contentRect = (RectTransform)content.transform;
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(14, 14, 14, 14);
        vlg.spacing = 18f;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 滚动组件：支持鼠标拖拽与滚轮
        ScrollRect scroll = viewport.AddComponent<ScrollRect>();
        scroll.content = contentRect;
        scroll.viewport = (RectTransform)viewport.transform;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.scrollSensitivity = 30f;
        scroll.movementType = ScrollRect.MovementType.Elastic;

        // 条目：嫘祖简介
        MakeItem(content.transform, intro, 24, new Color(0.3f, 0.24f, 0.18f), TextAnchor.UpperLeft);

        // 条目：玩法说明
        MakeItem(content.transform, howToPlay, 24, new Color(0.3f, 0.24f, 0.18f), TextAnchor.UpperLeft);

        // 条目：六步仪程（与卡面一一对应）
        for (int i = 0; i < stepNames.Length; i++)
        {
            string txt = "<b><color=#8B2500>" + stepNums[i] + " · " + stepNames[i] +
                         "（" + stepTitles[i] + "）</color></b>\n" + stepDescs[i];
            MakeItem(content.transform, txt, 24, new Color(0.3f, 0.24f, 0.18f), TextAnchor.UpperLeft);
        }

        // 条目：结尾
        MakeItem(content.transform, "<b><color=#8B2500>—— 祭 祀 将 始 ——</color></b>", 26,
            new Color(0.545f, 0.145f, 0f), TextAnchor.MiddleCenter);

        // 条目：开始祭祀按钮（位于内容结尾，点击收起开场界面并生成牌阵）
        GameObject slot = NewUI("StartSlot", content.transform);
        slot.AddComponent<LayoutElement>().preferredHeight = 92f;
        GameObject btnGo = NewUI("StartButton", slot.transform);
        Image btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.545f, 0.145f, 0f);
        RectTransform btnRect = (RectTransform)btnGo.transform;
        btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(320f, 66f);
        btnRect.anchoredPosition = Vector2.zero;
        Button btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(() => {
            if (GameManager.Instance != null)
                GameManager.Instance.StartGame();
        });
        Text btnText = MakeText(btnGo.transform, "开始祭祀", 30,
            Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetRect(btnText.gameObject, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // 右侧滚动条
        GameObject sbGo = NewUI("Scrollbar", window.transform);
        sbGo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.08f);
        SetRect(sbGo, new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-50f, 40f), new Vector2(-32f, -185f));
        Scrollbar sb = sbGo.AddComponent<Scrollbar>();
        GameObject handleGo = NewUI("Handle", sbGo.transform);
        Image handleImg = handleGo.AddComponent<Image>();
        handleImg.color = new Color(0.545f, 0.145f, 0f, 0.7f);
        SetRect(handleGo, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        sb.handleRect = (RectTransform)handleGo.transform;
        sb.targetGraphic = handleImg;
        sb.direction = Scrollbar.Direction.BottomToTop;
        scroll.verticalScrollbar = sb;

        return canvasGo;
    }

    // ===== 工具方法 =====

    private static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        if (parent != null) go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    private static Text MakeText(Transform parent, string content, int size, Color color, FontStyle style, TextAnchor anchor)
    {
        GameObject go = NewUI("Text", parent);
        Text t = go.AddComponent<Text>();
        t.font = uiFont;
        t.text = content;
        t.fontSize = size;
        t.color = color;
        t.fontStyle = style;
        t.alignment = anchor;
        t.supportRichText = true;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    private static void MakeItem(Transform parent, string content, int size, Color color, TextAnchor anchor)
    {
        // 交给 VerticalLayoutGroup 控制：宽度随容器，高度按文字自动撑开
        MakeText(parent, content, size, color, FontStyle.Normal, anchor);
    }
}
