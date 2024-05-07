using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace CGame
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int structSize;
        public IntPtr dlgOwner;
        public IntPtr instance;
        public string filter;
        public string customFilter;
        public int maxCustFilter;
        public int filterIndex;
        public string file;
        public int maxFile;
        public string fileTitle;
        public int maxFileTitle;
        public string initialDir;
        public string title;
        public int flags;
        public short fileOffset;
        public short fileExtension;
        public string defExt;
        public IntPtr custData;
        public IntPtr hook;
        public string templateName;
        public IntPtr reservedPtr;
        public int reservedInt;
        public int flagsEx;
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenDialogDir
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public String pszDisplayName;
        public String lpszTitle;
        public UInt32 ulFlags;
        public IntPtr lpfn;
        public IntPtr lParam;
        public int iImage;
    }
    
    public static class WindowsAPIUtility
    {
        private static readonly StringBuilder _stringBuilder = new();

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenFileName dialog);
        
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetSaveFileName([In, Out] OpenFileName dialog);
        
        public static string OpenFilePanel(string title, string directory, params string[] extensions)
        {
            var dialog = new OpenFileName
            {
                initialDir = Application.dataPath,
                title = title,
                defExt = extensions.Length > 0 ? extensions[0] : null,
                //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST|OFN_NOCHANGEDIR|OFN_OVERWRITEPROMPT
                flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008 | 0x00000002, 
            };
 
            dialog.structSize = Marshal.SizeOf(dialog);

            if (extensions.Length <= 0)
                dialog.filter = "All Files\0*.*\0\0";
            else
            {
                _stringBuilder.Clear();
                foreach (var extension in extensions)
                {
                    var e = extension[0] is '.' ? extension[1..] : extension;
                    _stringBuilder.Append(e).Append(" files\0");
                    _stringBuilder.Append("*.").Append(e).Append('\0');
                }
                _stringBuilder.Append('\0');
                dialog.filter = _stringBuilder.ToString();
            }

            _stringBuilder.Clear();
            _stringBuilder.Append(directory.Replace('/', Path.DirectorySeparatorChar)).Append(new string(new char[256]));
            dialog.file = _stringBuilder.ToString();
 
            dialog.maxFile = dialog.file.Length;

            return GetOpenFileName(dialog) ? dialog.file.Replace(Path.DirectorySeparatorChar, '/') : "";
        }

        public static string SaveFilePanel(string title, string directory, string defaultName, params string[] extensions)
        {
            var dialog = new OpenFileName
            {
                initialDir = Application.dataPath,
                title = title,
                defExt = extensions.Length > 0 ? extensions[0] : null,
                //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST|OFN_NOCHANGEDIR|OFN_OVERWRITEPROMPT
                flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008 | 0x00000002, 
            };
 
            dialog.structSize = Marshal.SizeOf(dialog);

            if (extensions.Length <= 0)
                dialog.filter = "All Files\0*.*\0\0";
            else
            {
                _stringBuilder.Clear();
                foreach (var extension in extensions)
                {
                    var e = extension[0] is '.' ? extension[1..] : extension;
                    _stringBuilder.Append(e).Append(" files\0");
                    _stringBuilder.Append("*.").Append(e).Append('\0');
                }
                _stringBuilder.Append('\0');
                dialog.filter = _stringBuilder.ToString();
            }

            _stringBuilder.Clear();
            _stringBuilder
                .Append(directory.Replace('/', Path.DirectorySeparatorChar))
                .Append(Path.DirectorySeparatorChar)
                .Append(defaultName)
                .Append(new string(new char[256]));
            dialog.file = _stringBuilder.ToString();
 
            dialog.maxFile = dialog.file.Length;
            
            return GetSaveFileName(dialog) ? dialog.file.Replace(Path.DirectorySeparatorChar, '/') : "";
        }
        
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private static readonly IntPtr ParentWnd = FindWindow(null, Application.productName);
        
        [DllImport("user32.dll")]
        private static extern int SetWindowTextW(IntPtr hWnd, byte[] text);
        
        public static void ChangeTitleText(string text)
        {
            SetWindowTextW(ParentWnd, Encoding.Unicode.GetBytes(text + (char)0));
        }
    }
}