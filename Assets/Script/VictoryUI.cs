using UnityEngine;
using UnityEngine.UI;


public static class VictoryUI
{
    private static GameObject root;
    private static Font uiFont;

    private static readonly string[] stepNums   = { "第一步", "第二步", "第三步", "第四步", "第五步", "第六步" };
    private static readonly string[] stepNames  = { "鸣炮", "巡游", "焚香", "拜礼", "贡品", "乐舞" };
    private static readonly string[] stepTitles = { "启幕", "迎神", "通神", "致敬", "献祭", "颂德" };
    private static readonly string[] stepDescs  =
    {
        "鸣放礼炮，宣告大典正式开始。炮声庄严肃穆，既营造威仪氛围，也表达后人对先蚕娘娘的崇高敬意。",
        "恭抬嫘祖圣像绕场巡游，信众夹道恭迎。寓意恭请先蚕娘娘降临享祭，也让桑蚕文化随队伍播撒四方。",
        "主祭与来宾依次敬香。青烟袅袅上达天宇，古人认为香能沟通人神，借一炷心香表达虔诚与追思。",
        "全体肃立，向嫘祖圣像行三鞠躬（或跪拜）礼，缅怀嫘祖教民育蚕缫丝、衣被天下的不朽功德。",
        "敬献丝绸、五谷、时令果品等供品，以今日衣食丰足告慰先祖，感恩她开启华夏衣冠文明。",
        "献演礼乐歌舞，颂扬嫘祖功绩。乐舞告祭将大典推向高潮，寓意薪火相传、礼成圆满。"
    };

    private const string intro =
        "嫘祖，黄帝之妃，始教民育蚕、缫丝织帛，被尊为“先蚕娘娘”。后世以祭祀大典缅怀其功德。以下仪程六步，与游戏中六种卡面一一对应，依序而行，缺一不可。";

    public static void Show(int moves)
    {
        if (root == null)
            root = Build(moves);
        else
            root.SetActive(true);
    }

    private static GameObject Build(int moves)
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null)
            uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        GameObject canvasGo = NewUI("VictoryCanvas", null);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();


        GameObject backdrop = NewUI("Backdrop", canvasGo.transform);
        Image bg = backdrop.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

  
        GameObject window = NewUI("Window", canvasGo.transform);
        Image winImg = window.AddComponent<Image>();
        winImg.color = new Color(0.965f, 0.93f, 0.86f);
        RectTransform winRect = (RectTransform)window.transform;
        winRect.anchorMin = winRect.anchorMax = new Vector2(0.5f, 0.5f);
        winRect.sizeDelta = new Vector2(820f, 980f);


        Text title = MakeText(window.transform, "祭 祀 礼 成", 54,
            new Color(0.545f, 0.145f, 0f), FontStyle.Bold, TextAnchor.MiddleCenter);
        SetRect(title.gameObject, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(30f, -105f), new Vector2(-30f, -30f));


        Text sub = MakeText(window.transform, "翻牌共用 " + moves + " 步 · 嫘祖祭祀仪程六礼", 26,
            new Color(0.45f, 0.36f, 0.28f), FontStyle.Normal, TextAnchor.MiddleCenter);
        SetRect(sub.gameObject, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(30f, -165f), new Vector2(-30f, -115f));

        GameObject viewport = NewUI("Viewport", window.transform);
        viewport.AddComponent<Image>().color = Color.white;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        SetRect(viewport, Vector2.zero, Vector2.one,
            new Vector2(35f, 40f), new Vector2(-60f, -185f));

   
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

        ScrollRect scroll = viewport.AddComponent<ScrollRect>();
        scroll.content = contentRect;
        scroll.viewport = (RectTransform)viewport.transform;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.scrollSensitivity = 30f;
        scroll.movementType = ScrollRect.MovementType.Elastic;


        MakeItem(content.transform, intro, 24, new Color(0.3f, 0.24f, 0.18f), TextAnchor.UpperLeft);

        for (int i = 0; i < stepNames.Length; i++)
        {
            string txt = "<b><color=#8B2500>" + stepNums[i] + " · " + stepNames[i] +
                         "（" + stepTitles[i] + "）</color></b>\n" + stepDescs[i];
            MakeItem(content.transform, txt, 24, new Color(0.3f, 0.24f, 0.18f), TextAnchor.UpperLeft);
        }

        MakeItem(content.transform, "<b><color=#8B2500>—— 礼 成 ——</color></b>", 26,
            new Color(0.545f, 0.145f, 0f), TextAnchor.MiddleCenter);

        GameObject slot = NewUI("ReturnSlot", content.transform);
        slot.AddComponent<LayoutElement>().preferredHeight = 92f;
        GameObject btnGo = NewUI("ReturnButton", slot.transform);
        Image btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.545f, 0.145f, 0f);
        RectTransform btnRect = (RectTransform)btnGo.transform;
        btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(320f, 66f);
        btnRect.anchoredPosition = Vector2.zero;
        Button btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        GameObject rootRef = canvasGo;
        btn.onClick.AddListener(() => rootRef.SetActive(false));
        Text btnText = MakeText(btnGo.transform, "返回游览", 30,
            Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetRect(btnText.gameObject, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);


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

        MakeText(parent, content, size, color, FontStyle.Normal, anchor);
    }
}
