#if DEF_CLIENT

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using static DEF.Client.ViewMgr;

public class GameObjectPool : MonoBehaviour
{
    private static GameObjectPool instance { get; set; }
    private Dictionary<string, Queue<GameObject>> PoolDictionary { get; set; } = new();
    private Dictionary<int, GameObject> CardPoolDictionary { get; set; } = new();
    private Dictionary<int, GameObject> EventCardPoolDictionary { get; set; } = new();
    private Dictionary<string, Sprite> TexturePoolDictionary { get; set; } = new();

    public static GameObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameObjectPool>();
                if (instance == null)
                {
                    GameObject obj = new("GameObjectPool");
                    instance = obj.AddComponent<GameObjectPool>();
                }
            }
            return instance;
        }
    }

    public void CreatePool(GameObject prefab, int size)
    {
        string prefab_name = prefab.name;
        if (!PoolDictionary.ContainsKey(prefab_name))
        {
            PoolDictionary.Add(prefab_name, new Queue<GameObject>());
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                PoolDictionary[prefab_name].Enqueue(obj);
            }
        }
    }

    public GameObject GetFromPool(string prefab_name)
    {
        if (PoolDictionary.ContainsKey(prefab_name) && PoolDictionary[prefab_name].Count > 0)
        {
            GameObject obj = PoolDictionary[prefab_name].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("No available object in the pool.");
            return null;
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        string prefab_name = obj.name;
        if (PoolDictionary.ContainsKey(prefab_name))
        {
            obj.SetActive(false);
            PoolDictionary[prefab_name].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Object not found in the pool.");
        }
    }

    public void CreateCardPool(GameObject prefab, int size)
    {
        CreateCards(prefab, size);
        CreateCards(prefab, size, true);
    }

    private void CreateCards(GameObject prefab, int size, bool is_activity = false)
    {
        var card_pool = is_activity ? EventCardPoolDictionary : CardPoolDictionary;
        for (int i = 0; i < size; i++)
        {
            int id = 1001 + i;
            if (!card_pool.ContainsKey(id))
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);

                var rect_tran = obj.transform.GetComponent<RectTransform>();
                rect_tran.localRotation = Quaternion.identity;
                rect_tran.localScale = Vector3.zero;
                obj.name = $"{id}";
                card_pool[id] = obj;
                //添加卡牌图片
                var img_card_icon = obj.transform.Find("!CardBg/CommonItems/ImageCard").GetComponent<Image>();
                string texture_name = is_activity ? $"Event{id}" : id.ToString();
                Sprite sprite = TexturePoolDictionary[$"Assets/Arts/Ui/DynamicCards/{texture_name}.png"];
                if (sprite != null)
                {
                    img_card_icon.sprite = sprite;
                }
            }
        }
    }

    private void SetName(Transform parent, string new_name)
    {
        parent.GetComponent<RectTransform>().localScale = Vector3.one;
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name != "Sfx")
            {
                child.name = new_name;
                child.GetComponent<Button>().enabled = true;
                child.Find("ChooseUnlock").gameObject.SetActive(false);
                child.Find("ChooseLock").gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public GameObject GetCardFromPool(int id, bool is_activity)
    {
        var card_pool = is_activity ? EventCardPoolDictionary : CardPoolDictionary;
        if (card_pool.ContainsKey(id))
        {
            GameObject obj = card_pool[id];
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("No available object in the card pool.");
            return null;
        }
    }

    public void ReturnCardToPool(GameObject obj, int id, bool is_activity)
    {
        var card_pool = is_activity ? EventCardPoolDictionary : CardPoolDictionary;
        if (card_pool.ContainsKey(id))
        {
            obj.SetActive(false);
            SetName(obj.transform, "!CardBg");
            obj.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning("Object not found in the pool.");
        }
    }

    public async Task CreateTexturePool(List<string> texture_list, int size)
    {
        List<Task> load_task = new();
        List<LoadAssetData> handle_list = new();
        for (int i = 0; i < size; i++)
        {
            int id = 1001 + i;
            var card_name = $"Assets/Arts/Ui/DynamicCards/{id}.png";
            if (!TexturePoolDictionary.ContainsKey(card_name))
            {
                //Debug.Log($"--------id:{id}");
                var handle0 = YooAssets.LoadAssetAsync<Sprite>(card_name);
                handle_list.Add(new LoadAssetData { LKey = card_name, LTask = handle0.Task, LHandle = handle0 });
                load_task.Add(handle0.Task);
            }
        }

        for (int i = 0; i < size; i++)
        {
            int id = 1001 + i;
            var card_name = $"Assets/Arts/Ui/DynamicCards/Event{id}.png";
            if (!TexturePoolDictionary.ContainsKey(card_name))
            {
                //Debug.Log($"--------id:Event{id}");
                var handle1 = YooAssets.LoadAssetAsync<Sprite>(card_name);
                handle_list.Add(new LoadAssetData { LKey = card_name, LTask = handle1.Task, LHandle = handle1 });
                load_task.Add(handle1.Task);
            }
        }

        for (int i = 0; i < texture_list.Count; i++)
        {
            var texture_name = texture_list[i];
            if (!TexturePoolDictionary.ContainsKey(texture_name))
            {
                //Debug.Log($"--------texture_name:{texture_name}");
                var handle2 = YooAssets.LoadAssetAsync<Sprite>(texture_name);
                handle_list.Add(new LoadAssetData { LKey = texture_name, LTask = handle2.Task, LHandle = handle2 });
                load_task.Add(handle2.Task);
            }
        }

        await Task.WhenAll(load_task);

        foreach (var d in handle_list)
        {
            var sprite = (Sprite)d.LHandle.AssetObject;
            if (!TexturePoolDictionary.ContainsKey(d.LKey))
            {
                TexturePoolDictionary[d.LKey] = sprite;
            }
        }

    }

    public Sprite GetTextureFromPool(string key)
    {
        if (TexturePoolDictionary.ContainsKey(key))
        {
            Sprite obj = TexturePoolDictionary[key];
            return obj;
        }
        else
        {
            Debug.LogWarning("No available Sprite in the card pool.");
            return null;
        }
    }
}

#endif