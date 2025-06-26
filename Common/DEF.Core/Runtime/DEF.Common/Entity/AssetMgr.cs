namespace DEF
{
    // 只读的配置信息多个Scene之间可以共享，多个线程之间可以共享

    public class AssetMgr
    {
        string PathAssetsRoot { get; set; }// Assets资源目录路径

        public AssetMgr(string path_assets_root)
        {
            PathAssetsRoot = path_assets_root;
        }

        // 创建空的EntityAssetPrefab对象
        public AssetPrefab CreateAssetPrefab(string entity_name)
        {
            AssetPrefab prefab = new();

            return prefab;
        }

        // 从Prefab文件加载EntityAssetPrefab对象
        public AssetPrefab LoadAssetPrefab(string file_name)
        {
            // 后缀名，json，bson

            AssetPrefab prefab = new();

            return prefab;
        }

        // 将EntityAssetPrefab对象保存到文件中
        public void SaveAssetPrefab(AssetPrefab prefab, string file_name)
        {
        }
    }
}