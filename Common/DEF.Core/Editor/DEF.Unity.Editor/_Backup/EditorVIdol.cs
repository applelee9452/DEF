//#if UNITY_EDITOR 

//using UnityEditor;
//using UnityEngine;

//public class EditorVIdol
//{
//    //[MenuItem("DEF/һ������ѡ����ɫ�����", false, 500)]
//    static void MenuConfigAvatarComponents()
//    {
//        UnityEngine.Object[] select_objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
//        if (select_objs == null || select_objs.Length == 0)
//        {
//            Debug.LogWarning("��ѡ�н�ɫ��GameObject��");
//            return;
//        }

//        foreach (var i in select_objs)
//        {
//            Debug.Log("Name=" + i.name);

//            GameObject go = (GameObject)i;
//            var animator = go.GetComponent<Animator>();
//            if (animator == null)
//            {
//                Debug.LogWarning("��ѡ�н�ɫ��GameObject��");
//                continue;
//            }

//            var com_lppv = go.GetComponentInChildren<LightProbeProxyVolume>();
//            if (com_lppv == null)
//            {
//                Debug.LogWarning("��ѡ�н�ɫû�����LightProbeProxyVolume�����");
//                continue;
//            }

//            var arr1 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
//            foreach (var j in arr1)
//            {
//                j.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
//                j.lightProbeProxyVolumeOverride = com_lppv.gameObject;

//                Debug.Log("����LPPV��" + j.name);

//                //var mat = AssetDatabase.LoadAssetAtPath<Material>(j.material.name);
//                //if (mat == null)
//                //{
//                //    Debug.LogWarning("ô���ҵ����ʣ�" + j.material.name);
//                //}
//                //j.material
//                //Material.f
//            }

//            var arr2 = go.GetComponentsInChildren<MeshRenderer>();
//            foreach (var j in arr2)
//            {
//                j.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
//                j.lightProbeProxyVolumeOverride = com_lppv.gameObject;

//                Debug.Log("����LPPV��" + j.name);
//            }
//        }
//    }
//}

//#endif
