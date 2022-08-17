using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public string pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public static class FolderBrowserHelper
{

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenDialogFile ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenDialogFile ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);


    /// <summary>
    /// 打开一个窗口选择一个文件
    /// </summary>
    /// <param name="fileType">需要选择的文件类型</param>
    /// <param name="openPath">设置打开的默认路径</param>
    /// <returns>返回选择文件的路径</returns>
    public static string SelectFile(string openPath = null)
    {
        OpenFileName pth = new OpenFileName();
        string filepath = null;
        pth.structSize = Marshal.SizeOf(pth);
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.filter = "Excel Files(*.json)\0*.json\0";
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        if (string.IsNullOrEmpty(openPath))
            pth.initialDir = System.Environment.CurrentDirectory; //打开的默认路径自行更改
        else
            pth.initialDir = openPath;
        pth.title = "请选择游戏";
        pth.defExt = "json";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        //0x00080000   是否使用新版文件选择窗口
        //0x00000200   是否可以多选文件
        if (GetSaveFileName(pth))
        {
            filepath = pth.file;//选择的文件路径;  
        }
        return filepath;
    }

    /// <summary>
    /// 调用WindowsExploer 并返回所选文件夹路径
    /// </summary>
    /// <param name="dialogtitle">打开对话框的标题</param>
    /// <returns>所选文件夹路径</returns>
    public static string GetPathFromWindowsExplorer(string dialogtitle = "请选择下载路径")
    {
        try
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string(new char[2048]);
            ; // 存放目录路径缓冲区  
            ofn2.lpszTitle = dialogtitle; // 标题  
            ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
            IntPtr pidlPtr = SHBrowseForFolder(ofn2);

            char[] charArray = new char[2048];

            for (int i = 0; i < 2048; i++)
            {
                charArray[i] = '\0';
            }

            SHGetPathFromIDList(pidlPtr, charArray);
            string res = new string(charArray);
            res = res.Substring(0, res.IndexOf('\0'));
            return res;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return string.Empty;
    }
}