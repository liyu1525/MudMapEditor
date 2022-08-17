using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Script.Util
{
    public static class UtilFile
    {
        /// <summary>
        /// 储存信息在本地（重置）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="str"></param>
        public static void ResetFile(string name, string str)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/data/"))
            {
                try
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/data/");
                }
                catch
                {
                }
            }
            string path = Application.persistentDataPath + "/data/" + name;
            File.WriteAllText(path, str);
        }

        /// <summary>
        /// 储存信息在本地（重置）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="str"></param>
        public static void ResetFilePath(string path, string name, string str)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                }
            }
            File.WriteAllText(path + "/" + name, str);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ReadFile(string name)
        {
            string str = string.Empty;
            string path = Application.persistentDataPath + "/data/" + name;
            if (File.Exists(path))
            {
                str = File.ReadAllText(path);
            }
            return str;
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ReadFilePath(string path)
        {
            string str = string.Empty;
            if (File.Exists(path))
            {
                str = File.ReadAllText(path);
            }
            return str;
        }
    }
}