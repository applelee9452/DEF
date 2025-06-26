#if DEF_CLIENT

using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FlyItem
{
    coin,
    coinEvent,
    gem,
    key,
    potion,
    potionEvent,
    crown,
    number,
}

[System.Serializable]
public struct AudioClipSound
{
    public AudioClip audioValue;
    public FlyItem audioKey;
}

public class LineFlyController
{
    private GameObject flyObj;
    private int flyNum;
    private List<GameObject> _flyObjList;

    public LineFlyController(GameObject flyObj, int num)
    {
        this.flyObj = flyObj;
        this.flyNum = num;
        _flyObjList = new List<GameObject>();
    }

    public void RecyleCoinGo(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        _flyObjList.Add(obj);
    }

    public GameObject GetFlyGo()
    {
        if (_flyObjList.Count == 0)
        {
            _flyObjList.Add(GameObject.Instantiate(flyObj));
        }

        var go = _flyObjList[0];
        _flyObjList.Remove(go);
        return go;
    }

    public Transform GetFlyParent()
    {
        return flyObj.transform.parent;
    }

    public void destroy()
    {
        if (_flyObjList != null)
        {
            _flyObjList.Clear();
            _flyObjList = null;
        }
    }

    public float PlayMoveUpNumberEffect(Vector2 begin_pos, string number, bool showBg)
    {
        var step = flyNum;
        GameObject cell = GetFlyGo();
        Transform fly_obj_tf = cell.transform;
        fly_obj_tf.localPosition = begin_pos;
        fly_obj_tf.SetParent(GetFlyParent(), false); 
        fly_obj_tf.gameObject.SetActive(true);
        //set background
        var bg_tf = fly_obj_tf.Find("bgImage");
        bg_tf?.gameObject.SetActive(showBg);
        if(showBg)
        {
            int len = number.Length - 3;
            int width = len < 0 ? 120 : 120 + len * 20;
            bg_tf.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 80);
        }
        //set text
        TextMeshProUGUI fly_obj_tmp = fly_obj_tf.GetComponentInChildren<TextMeshProUGUI>();
        fly_obj_tmp.text = number;
        //tween
        var end_pos_y_1 = fly_obj_tf.localPosition.y + 100;
        var end_pos_y_2 = fly_obj_tf.localPosition.y + 200;
        var start_pos_y = fly_obj_tf.localPosition.y;

        var fly_obj_cv = fly_obj_tf.GetComponent<CanvasGroup>();
        fly_obj_cv.alpha = showBg ? 0.9f : 0.5f;
        fly_obj_tf.localScale = Vector3.one * 0.3f;

        fly_obj_tf.DOLocalMoveY(end_pos_y_1, 1f).SetEase(Ease.OutQuart);
        fly_obj_tf.DOScale(1, 0.3f).SetEase(Ease.OutQuart);
        fly_obj_cv.DOFade(1, 0.3f).SetEase(Ease.OutQuart);
        fly_obj_tf.DOLocalMoveY(end_pos_y_2, 0.3f).SetEase(Ease.InQuart).SetDelay(1f);
        Tween startFade = fly_obj_cv.DOFade(0, 0.3f).SetEase(Ease.InQuart).SetDelay(1f);

        startFade.onComplete = () =>
        {
            GameObject ttObj = cell;
            RecyleCoinGo(ttObj);
        };
        return 1.3f;
    }

    public float PlayFlyEffectSeq(Vector2 begin_pos, Vector2 end_pos)
    {
        var step = flyNum;
        float am_time_ratio = 1;
        if (flyNum > 10)
        {
            am_time_ratio = 1;
        }
        else
        {
            am_time_ratio = 2;
        }

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < step; i++)
        {
            GameObject cell = GetFlyGo();
            Transform fly_obj_tf = cell.transform;
            fly_obj_tf.SetParent(GetFlyParent(), false);
            fly_obj_tf.localPosition = end_pos;
            var end_pos_3d = new Vector3(fly_obj_tf.localPosition.x, fly_obj_tf.localPosition.y, -100f);

            fly_obj_tf.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            fly_obj_tf.localPosition = begin_pos;
            fly_obj_tf.localPosition = new Vector3(fly_obj_tf.localPosition.x, fly_obj_tf.localPosition.y, -100f);
            fly_obj_tf.gameObject.SetActive(true);

            var path = GetPath(fly_obj_tf.localPosition, end_pos_3d);

            var tempSeq = DOTween.Sequence();
            var randomStartPos = new Vector2(fly_obj_tf.localPosition.x, fly_obj_tf.localPosition.y) + Random.insideUnitCircle * new Vector2(20, 20);
            Tween startMove = fly_obj_tf.DOLocalMove(new Vector3(randomStartPos.x, randomStartPos.y, -100f), 0.2f).SetUpdate(true);
            tempSeq.Append(startMove);
            tempSeq.AppendInterval(0.015f * am_time_ratio * i);

            Tween moveT = fly_obj_tf.DOLocalPath(path, 0.6f, PathType.CatmullRom).SetEase(Ease.InSine).SetUpdate(true);

            Tween scaleT = fly_obj_tf.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 1.0f).SetEase(Ease.InBack).SetUpdate(true);
            //  Tween rotateT = fly_obj_tf.DOLocalRotate(new Vector3(0, 360, 0), 0.25f, RotateMode.FastBeyond360).SetLoops(1, LoopType.Restart).SetEase(Ease.Linear).SetUpdate(true);
            tempSeq.Append(moveT);
            tempSeq.Join(scaleT);
            // tempSeq.Join(rotateT);
            tempSeq.SetDelay(0.02f * am_time_ratio * i);
            tempSeq.SetUpdate(true);
            tempSeq.SetLink(cell);
            tempSeq.onComplete = () =>
            {
                GameObject ttObj = cell;
                RecyleCoinGo(ttObj);
            };
            // 加入总序列
            seq.Join(tempSeq);
        }

        seq.SetUpdate(true);
        seq.Play();
        seq.SetAutoKill(true);
        return seq.Duration(false);
    }

    private Vector3[] GetPath(Vector3 start, Vector3 end)
    {
        var middle = (start + end) / 2;
        //  middle += Random.Range(-0.2f, 0.2f) * 5.4f * Vector3.right;
        return new Vector3[] { middle, end };
    }
}

// Ui，飞金币效果：一串金币从起点飞到终点
public class UIFlyElementController : MonoBehaviour
{
    [SerializeField]
    private GameObject _coinTemplateGo;
    [SerializeField]
    private GameObject _coinEventTemplateGo;

    [SerializeField]
    private GameObject _potionTemplateGo;
    [SerializeField]
    private GameObject _potionEventTemplateGo;

    [SerializeField]
    private GameObject _gemTemplateGo;

    [SerializeField]
    private GameObject _keyTemplateGo;

    [SerializeField]
    private GameObject _crownTemplateGo;

    [SerializeField]
    private GameObject _numberTemplateGo;

    [SerializeField]
    private RectTransform _endPosTrans;
    public Vector3 EndPos { get { return _endPosTrans.transform.position; } }

    [SerializeField]
    private RectTransform _centerPosTrans;
    public Vector3 CenterPos { get { return _centerPosTrans.transform.position; } }

    [SerializeField]
    private Canvas _canvas;
    public Canvas Canvas { get { return _canvas; } }

    [SerializeField]
    private Camera _cam;
    public Camera Camera { get { return _cam; } }

    [SerializeField]
    private List<AudioClipSound> audioList;

    [SerializeField]
    private AudioSource flyAudioSource;

    public RectTransform GetEndTransform()
    {
        return _endPosTrans as RectTransform;
    }

    public void Init()
    {
        _coinTemplateGo.SetActive(false);
        gameObject.SetActive(true);
    }
    
    public GameObject GetNumberTemplate()
    {
        return _numberTemplateGo;
    }

    public GameObject GetCoinTemplate()
    {
        return _coinTemplateGo;
    }

    public GameObject GetGemTemplate()
    {
        return _gemTemplateGo;
    }

    public GameObject GetPotionTemplate()
    {
        return _potionTemplateGo;
    }

    public GameObject GetKeyTemplate()
    {
        return _keyTemplateGo;
    }

    public GameObject GetCrownTemplate()
    {
        return _crownTemplateGo;
    }
    public GameObject GetCoinEventTemplate()
    {
        return _coinEventTemplateGo;
    }

    public GameObject GetPotionEventTemplate()
    {
        return _potionEventTemplateGo;
    }

    private AudioClip GetAudioClip(FlyItem type)
    {
        for (int i = 0; i < audioList.Count; i++)
        {
            if (audioList[i].audioKey == type)
            {
                return audioList[i].audioValue;
            }
        }
        return null;
    }

    public void PlayAudio(FlyItem type)
    {
        AudioClip ac = GetAudioClip(type);
        if (flyAudioSource != null && ac != null)
        {
            flyAudioSource.clip = ac;
            flyAudioSource.Play();
        }
    }
}

public class UIFlyElementSys
{
    private Dictionary<FlyItem, LineFlyController> flyCtlDic = new();

    private static UIFlyElementSys _instance;
    public static UIFlyElementSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIFlyElementSys();
            }

            return _instance;
        }
    }

    private UIFlyElementController _uiFlyElementController;
    public UIFlyElementController UIFlyElementController
    {
        get
        {
            if (_uiFlyElementController == null)
            {
                _uiFlyElementController = GameObject.FindObjectOfType<UIFlyElementController>();
                _uiFlyElementController.Init();
            }

            return _uiFlyElementController;
        }
    }

    private UIFlyElementSys()
    {
    }

    public void Destroy()
    {
    }

    private Vector3 GetNodeRefPoint(Vector3 localPoition, RectTransform originTran, RectTransform tarPTrans)
    {
        Vector3 wp = originTran.TransformPoint(localPoition);
        Vector3 newPos = tarPTrans.InverseTransformPoint(wp);
        return newPos;
    }

    private Vector2 WorldPosToUiLocalPos(Vector3 worldPos)
    {
        var screenPos = UIFlyElementController.Camera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIFlyElementController.Canvas.transform as RectTransform, screenPos, UIFlyElementController.Camera, out Vector2 localPos);
        return localPos;
    }

    // 起始点：世界坐标
    public float PlayMoveUpNumberEffect(Vector3 beginPos, string number, bool showBg = false)
    {
        var ctl = GetFlyController(FlyItem.number);
        return ctl.PlayMoveUpNumberEffect(WorldPosToUiLocalPos(beginPos), number, showBg);
    }

    public float PlayFlyCoinEffect(Vector3 beginPos, bool isActity = false)
    {
        var panelName = isActity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panelName}/Topbar/Main/Gold/Image") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(isActity ? FlyItem.coinEvent : FlyItem.coin, beginPos, tran.position);
    }

    public float PlayFlyGemEffect(Vector3 beginPos, bool isActity = false)
    {
        var panelName = isActity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panelName}/Topbar/Main/Diamond/Image") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(FlyItem.gem, beginPos, tran.position);
    }

    public float PlayFlyGemEffectFromCenter(bool is_actity = false)
    {
        Vector3 begin_pos = UIFlyElementController.CenterPos;

        var panel_name = is_actity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panel_name}/Topbar/Main/Diamond/Image") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(is_actity ? FlyItem.gem : FlyItem.gem, begin_pos, tran.position);
    }

    public float PlayFlyPotionEffect(Vector3 beginPos, bool isActity = false)
    {
        var panelName = isActity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panelName}/Topbar/Main/Potion/Image") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(isActity ? FlyItem.potionEvent : FlyItem.potion, beginPos, tran.position);
    }

    public float PlayFlyPotionEffectFromCenter(bool is_actity = false)
    {
        Vector3 begin_pos = UIFlyElementController.CenterPos;

        var panel_name = is_actity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panel_name}/Topbar/Main/Potion/Image") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }

        return PlayFlyEffect(is_actity ? FlyItem.potionEvent : FlyItem.potion, begin_pos, tran.position);
    }

    public float PlayFlyKeyEffect(Vector3 beginPos, bool isActity = false)
    {
        var panelName = isActity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panelName}/KeyProgress/img_icon/") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(FlyItem.key, beginPos, tran.position);
    }

    public float PlayFlyCrownEffect(Vector3 beginPos, bool isActity = false)
    {
        var panelName = isActity ? "PanelMainEvent" : "PanelMain";
        RectTransform tran = UIFlyElementController.Canvas.transform.Find($"TranPanel/Level/{panelName}/ButtonReward/ImageRank") as RectTransform;
        if (tran == null)
        {
            tran = UIFlyElementController.GetEndTransform();
        }
        return PlayFlyEffect(FlyItem.crown, beginPos, tran.position);
    }

    float PlayFlyEffect(FlyItem type, Vector3 beginPos, Vector3 endPos)
    {
        UIFlyElementController.PlayAudio(type);
        var ctl = GetFlyController(type);
        return ctl.PlayFlyEffectSeq(WorldPosToUiLocalPos(beginPos), WorldPosToUiLocalPos(endPos));
    }

    LineFlyController GetFlyController(FlyItem type)
    {
        if (flyCtlDic.TryGetValue(type, out LineFlyController controller))
        {
            return controller;
        }

        int num = 1;
        GameObject tempObj = UIFlyElementController.GetCoinTemplate();

        if (type == FlyItem.coin)
        {
            num = 10;
            tempObj = UIFlyElementController.GetCoinTemplate();
        }
        else if (type == FlyItem.coinEvent)
        {
            num = 10;
            tempObj = UIFlyElementController.GetCoinEventTemplate();
        }
        else if (type == FlyItem.gem)
        {
            num = 4;
            tempObj = UIFlyElementController.GetGemTemplate();
        }
        else if (type == FlyItem.potion)
        {
            num = 6;
            tempObj = UIFlyElementController.GetPotionTemplate();
        }
        else if (type == FlyItem.potionEvent)
        {
            num = 6;
            tempObj = UIFlyElementController.GetPotionEventTemplate();
        }
        else if (type == FlyItem.key)
        {
            num = 1;
            tempObj = UIFlyElementController.GetKeyTemplate();
        }
        else if (type == FlyItem.crown)
        {
            num = 1;
            tempObj = UIFlyElementController.GetCrownTemplate();
        }
        else if (type == FlyItem.number)
        {
            num = 1;
            tempObj = UIFlyElementController.GetNumberTemplate();
        }

        var ctl = new LineFlyController(tempObj, num);
        flyCtlDic.Add(type, ctl);

        return ctl;
    }
}

#endif