using UnityEngine;

namespace JHYJ_HotFix.Script.Util
{
    public class UtilObject
    {
        /// <summary>
        /// 销毁obj下所有子ui
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_all(Transform obj)
        {
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                Object.Destroy(ob);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 销毁obj下所有子ui
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_all(Transform obj, string name)
        {
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                if (obj.name.Equals(name))
                    Object.Destroy(ob);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 销毁obj下所有子ui
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_all(GameObject objf, string name)
        {
            Transform obj = objf.transform;
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                if (obj.name.Equals(name))
                    Object.Destroy(ob);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 销毁obj下所有子ui
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_all(GameObject o)
        {
            Transform obj = o.transform;
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                Object.Destroy(ob);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 销毁obj下指定对象
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_Obj(GameObject o, string name)
        {
            Transform obj = o.transform;
            int count = obj.childCount;
            GameObject ob;
            for (int i = count - 1; i >= 0; i--)
            {
                ob = obj.GetChild(i).gameObject;
                if (ob.name.Equals(name))
                    Object.Destroy(ob);
            }
        }

        /// <summary>
        /// 销毁obj下指定对象
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy_Obj(Transform obj, string name)
        {
            int count = obj.childCount;
            GameObject ob;
            for (int i = count - 1; i >= 0; i--)
            {
                ob = obj.GetChild(i).gameObject;
                if (ob.name.Equals(name))
                    Object.Destroy(ob);
            }
        }

        /// <summary>
        /// 寻找对象下指定名称的对象
        /// </summary>
        public static GameObject Find_Obj(GameObject o, string name)
        {
            Transform obj = o.transform;
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                if (ob.name.Equals(name))
                    return ob;
            }
            return null;
        }

        /// <summary>
        /// 寻找对象下指定名称的对象
        /// </summary>
        public static GameObject Find_Obj(Transform obj, string name)
        {
            int count = obj.childCount;
            GameObject ob;
            for (int i = count; i > 0; i--)
            {
                ob = obj.GetChild(i - 1).gameObject;
                if (ob.name.Equals(name))
                    return ob;
            }
            return null;
        }
    }
}
