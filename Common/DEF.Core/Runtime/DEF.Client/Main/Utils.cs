#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DEF.Client
{
    public static class Utils
    {
        const string HttpPrefix = "http:";
        const string HttpsPrefix = "https:";

        // 计算头像url
        //public static string CalcIconUrl(bool is_small, string icon, string account_id, bool enable_cache)
        //{
        //    if (!string.IsNullOrEmpty(icon) && (icon.StartsWith(HttpPrefix) || icon.StartsWith(HttpsPrefix)))
        //    {
        //        // 第三方来源的玩家头像
        //        return icon;
        //    }
        //    else if (!string.IsNullOrEmpty(icon))
        //    {
        //        // 机器人头像
        //        if (is_small)
        //        {
        //            return string.Concat(Config.GetBotIconDomain(), "BotIconSmall/", icon, ".jpg");
        //        }
        //        else
        //        {
        //            return string.Concat(Config.GetBotIconDomain(), "BotIcon/", icon, ".jpg");
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(account_id))
        //    {
        //        // 玩家头像
        //        if (is_small)
        //        {
        //            return string.Concat(Config.GetPlayerIconDomain(), account_id, "_thumbnail.jpg");
        //        }
        //        else
        //        {
        //            return string.Concat(Config.GetPlayerIconDomain(), account_id, "_image.jpg");
        //        }
        //    }

        //    return string.Empty;
        //}

        // 格式化玩家Id显示样式
        public static string FormatPlayerId(long actor_id)
        {
            return actor_id.ToString("00-000-00");
        }

        // 格式化UIPackage中的Image路径
        public static string FormatePackageImagePath(string package_name, string image_name)
        {
            return string.Concat("ui://", package_name, "/", image_name);
        }

        // 将总秒数格式化成分:秒形式，或者时:分:秒形式显示
        public static string FormatTmFromSecondToMinute(float tm, bool showhours)
        {
            int m, s;
            if (showhours)
            {
                int h = (int)tm / 3600;
                var temp = (int)tm % 3600;
                m = temp / 60;
                s = temp % 60;

                return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            }
            else
            {
                m = (int)tm / 60;
                s = (int)tm % 60;

                return string.Format("{0:00}:{1:00}", m, s);
            }
        }

        // 在网址后面追加随机数
        public static string FormalUrlWithRandomVersion(string url)
        {
            int rd = UnityEngine.Random.Range(1, 1001);
            return string.Concat(url, "?v=", rd);
        }

        // 普通帐号有效性验证
        public static bool IsValidAccountName(string str)
        {
            bool is_valid = true;
            string pattern = @"^[A-Za-z0-9_@.-]+$";
            var mathes = Regex.Matches(str, pattern);
            if (mathes.Count <= 0)
            {
                return false;
            }

            foreach (Match match in mathes)
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    is_valid = false;
                    break;
                }
            }

            return is_valid;
        }

        // 手机帐号有效性验证
        public static bool IsValidPhoneNum(string phone_num)
        {
            bool is_valid = true;
            string pattern = @"^1[0-9]{10}$";
            var mathes = Regex.Matches(phone_num, pattern);
            if (mathes.Count <= 0)
            {
                return false;
            }

            foreach (Match match in mathes)
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    is_valid = false;
                    break;
                }
            }

            return is_valid;
        }

        public static string AddEllipsisToStr(string str, int max_length, int show_length)
        {
            if (str.Length > show_length)
            {
                str = str.Substring(0, show_length) + "...";
            }
            return str;
        }

        // 修正Windows平台Shader不正确的问题
        // 现象：预制体上的粒子效果显示为紫色方块。
        // 原因：Shader在打成AB包后与指定平台产生相关性，Editor中无法正常读取。
        // 解决办法：遍历所有加载的对象，重新赋值Shader
        public static void ShaderRecover(UnityEngine.GameObject obj)
        {
            if (obj == null) return;

            UnityEngine.Renderer[] meshSkinRenderer = obj.GetComponentsInChildren<UnityEngine.Renderer>(true);
            for (int i = 0; i < meshSkinRenderer.Length; i++)
            {
                if (meshSkinRenderer[i].sharedMaterial != null)
                {
                    meshSkinRenderer[i].sharedMaterial.shader = UnityEngine.Shader.Find(meshSkinRenderer[i].sharedMaterial.shader.name);
                }
            }
        }

        // 修正Windows平台Shader不正确的问题
        // 现象：预制体上的粒子效果显示为紫色方块。
        // 原因：Shader在打成AB包后与指定平台产生相关性，Editor中无法正常读取。
        // 解决办法：遍历所有加载的对象，重新赋值Shader
        public static void ShaderRecover2(UnityEngine.GameObject obj)
        {
            if (obj == null) return;

            UnityEngine.Renderer[] meshSkinRenderer = obj.GetComponentsInChildren<UnityEngine.Renderer>(true);
            for (int i = 0; i < meshSkinRenderer.Length; i++)
            {
                if (meshSkinRenderer[i].sharedMaterial != null)
                {
                    meshSkinRenderer[i].sharedMaterial.shader = UnityEngine.Shader.Find(meshSkinRenderer[i].sharedMaterial.shader.name);
                }

                if (meshSkinRenderer[i].sharedMaterials != null)
                {
                    foreach (var j in meshSkinRenderer[i].sharedMaterials)
                    {
                        j.shader = UnityEngine.Shader.Find(j.shader.name);
                    }
                }
            }
        }

        // 比较版本大小
        public static int CompareVersion(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version2)) return 1;

            string new_version1 = version1.Replace(".", "");
            string new_version2 = version2.Replace(".", "");
            long new_version1_long = long.Parse(new_version1);
            long new_version2_long = long.Parse(new_version2);
            if (new_version1_long > new_version2_long)
            {
                return 1;
            }
            else if (new_version1_long == new_version2_long)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public static string FormatTime(DateTime dtTime)
        {
            string text_day = string.Format("{0:00}-{1:00} {2:00}:{3:00}", dtTime.Month, dtTime.Day, dtTime.Hour, dtTime.Minute);
            return text_day;
        }

        // 返回三个参数的坐标
        public static UnityEngine.Vector2 ParseVector2(string str)
        {
            string[] str_list = str.Split(',');
            var vector = new UnityEngine.Vector2(0, 0);
            if (str_list.Length < 2) return vector;
            vector.x = float.Parse(str_list[0]);
            vector.y = float.Parse(str_list[1]);
            return vector;
        }

        // 返回三个参数的坐标
        public static UnityEngine.Vector3 ParseVector3(string str)
        {
            string[] str_list = str.Split(',');
            var vector = new UnityEngine.Vector3(0, 0, 0);
            if (str_list.Length < 3) return vector;
            vector.x = float.Parse(str_list[0]);
            vector.y = float.Parse(str_list[1]);
            vector.z = float.Parse(str_list[2]);
            return vector;
        }
    }
}

#endif
