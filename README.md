CGame 
===
[![Releases](https://img.shields.io/github/release/car-rot1/CGame.svg)](https://github.com/car-rot1/CGame/releases)\
一个Unity游戏开发工具箱

## 通过 git URL 安装
您可以添加`https://github.com/car-rot1/CGame.git?path=src`到包管理器

## 目录
### Editor
- [AnimatorController](#animatorController)
- [ChangeSpriteRect](#changeSpriteRect)
- [DropDown](#dropDown)
- [AssetDatabaseExtension](#assetDatabaseExtension)
- [EditorGUIExtension](#editorGUIExtension)
- [GenericMenuExtension](#genericMenuExtension)

AnimatorController
---
<b>相关文件路径：</b>`CGame/src/Editor/AnimatorController/AnimatorControllerExtension.cs`\
简介：一个可获取动画器相关Editor属性的静态类。\
使用方法：
```csharp
using CGame.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class EditorTest : EditorWindow
{
    private void OnGUI()
    {
        // 当前动画器面板的AnimatorController文件
        AnimatorController currentAnimatorController = AnimatorControllerExtension.CurrentAnimatorController;
        
        // 当前动画器面板所选中的AnimatorLayerIndex
        int selectedAnimatorLayerIndex = AnimatorControllerExtension.SelectedAnimatorLayerIndex;
        
        // 当前动画器面板的所有State名称
        string[] allStateName = AnimatorControllerExtension.AllStateName;
        
        Debug.Log(currentAnimatorController + " " + selectedAnimatorLayerIndex + " " + allStateName);
    }
}
```

ChangeSpriteRect
---
<b>相关文件路径：</b>`CGame/src/Editor/ChangeSpriteRect/ChangeSpriteRect.cs`\
简介：扩展了Sprite2D插件切割Sprite的面板。在右下角可将部分属性应用于所有Sprite；同时新增了EditorWindow`Tools/Change Sprite Rect`，可将属性应用于目标纹理下的所有Sprite。

DropDown
---
<b>相关文件路径：</b>`CGame/src/Editor/DropDown/DropDownPopupWindow.cs`\
简介：封装了一个可自定义的下拉窗口（可自定义列表元素、选中时回调方法、分隔符、是否带搜索栏、选中后是否自动关闭）。\
使用方法：
```csharp
using System;
using System.Collections.Generic;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

public class EditorTest : EditorWindow
{
    private void OnGUI()
    {
        /* 调用Show方法显示下拉框 */
        
        Rect activatorRect = new Rect(0, 0, 50, 18);    // 下拉框显示位置的Rect
        IReadOnlyCollection<string> content = new[] { "a", "b", "c" };  // 下拉框显示的内容
        Action<string> callback = item => Debug.Log(item);  //选中项时触发的回调方法
        char splitChar = '/';   // 默认值为'/'，显示内容的分隔符，用于多级折叠
        bool hasSearch = true;  // 默认值为true，是否显示搜索框
        bool autoClose = true;  // 默认值为true，选中项后是否自动关闭下拉框
        
        DropDownPopupWindow.Show(activatorRect, content, callback, splitChar, hasSearch, autoClose);
    }
}
```

AssetDatabaseExtension
---
<b>相关文件路径：</b>`CGame/src/Editor/Extension/AssetDatabaseExtension.cs`\
简介：扩展了AssetDatabase类，提供了获取绝对路径的方法。\
使用方法：
```csharp
using CGame.Editor;
using UnityEditor;
using UnityEngine;

public class EditorTest : EditorWindow
{
    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        // 通过InstanceID获取资源所在的绝对路径
        var instanceIDToAbsolutePath = AssetDatabaseExtension.GetAssetAbsolutePath(instanceID);

        // 通过Asset资源本身获取资源所在的绝对路径
        Object asset = EditorUtility.InstanceIDToObject(instanceID);
        var assetToAbsolutePath = AssetDatabaseExtension.GetAssetAbsolutePath(asset);
        
        Debug.Log(instanceIDToAbsolutePath + " " + assetToAbsolutePath);
        return true;
    }
}
```

EditorGUIExtension
---
<b>相关文件路径：</b>`CGame/src/Editor/Extension/EditorGUIExtension.cs`\
简介：扩展了EditorGUI类，提供了部分内部静态属性、静态方法以及额外的绘制方法。\
使用方法：
```csharp
using CGame;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

public class EditorTest : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 绘制多个元素时的行间距，默认值为2
        float controlVerticalSpacing = EditorGUIExtension.ControlVerticalSpacing;
        
        // 绘制单个元素时的内部行间距
        float verticalSpacingMultiField = EditorGUIExtension.VerticalSpacingMultiField;
        
        // 多个层级时的缩进大小，值为常数15
        float indentPerLevel = EditorGUIExtension.IndentPerLevel;
        
        Debug.Log(controlVerticalSpacing + " " + verticalSpacingMultiField + " " + indentPerLevel);
        
        /* 全部展开或折叠当前属性 */

        bool expanded = true;   // true为全部展开；false为全部折叠
        
        EditorGUIExtension.SetExpandedRecurse(property, expanded);
        
        /* 绘制实心矩形（线条） */

        Rect solidRectRect = position;  // 绘制的位置Rect
        Color solidRectColor = Color.cyan;  // 线条的颜色
        bool solidRectUsePlaymodeTint = true;    // 默认值为true，是否使用Playmode的色调
        
        EditorGUIExtension.DrawSolidRect(solidRectRect, solidRectColor, solidRectUsePlaymodeTint);
        
        /* 绘制边框 */

        Rect borderRect = position; // 绘制的位置Rect
        float borderWidth = 1;  // 存在重载方法，可分别传入四条边框的宽度。边框的宽度
        Color borderColor = Color.cyan; // 边框的颜色
        bool borderUsePlaymodeTint = true;    // 默认值为true，是否使用Playmode的色调
        
        EditorGUIExtension.DrawBorders(borderRect, borderWidth, borderColor, borderUsePlaymodeTint);
        
        /* 根据网格的尺寸绘制网格 */

        Rect gridRect = position;  // 绘制的位置Rect
        float gridLineWidth = 1;    // 网格线条的宽度
        Color gridColor = Color.cyan;   // 网格线条的颜色
        float gridWidth = 5;    // 网格的宽度
        float gridHeight = 5;    // 网格的高度
        ToIntType gridToIntType = ToIntType.Floor;  // 默认值为ToIntType.Floor，网格取整时的方式
        bool gridUsePlaymodeTint = true;    // 默认值为true，是否使用Playmode的色调
        
        EditorGUIExtension.DrawGridsForSize(gridRect, gridLineWidth, gridColor, gridWidth, gridHeight, gridToIntType, gridUsePlaymodeTint);
    }
}
```

GenericMenuExtension
---
<b>相关文件路径：</b>`CGame/src/Editor/Extension/GenericMenuExtension.cs`\
简介：扩展了GenericMenu类，提供了清空菜单项的方法，避免重复创建GenericMenu。\
使用方法：
```csharp
using CGame.Editor;
using UnityEditor;
using UnityEngine;

public class EditorTest : EditorWindow
{
    // 将GenericMenu定义为全局变量，避免重复创建
    private GenericMenu _genericMenu;
    
    private void OnEnable()
    {
        // 只需初始化时创建一次
        _genericMenu = new GenericMenu();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,50,18), "GenericMenu 0"))
        {
            // 多次使用时调用ClearItem扩展方法，清空菜单项
            _genericMenu.ClearItem();
            _genericMenu.AddItem(new GUIContent("GenericMenu 0"), false, () => { });
            _genericMenu.ShowAsContext();
        }
        
        if (GUI.Button(new Rect(0,20,50,18), "GenericMenu 1"))
        {
            // 多次使用时调用ClearItem扩展方法，清空菜单项
            _genericMenu.ClearItem();
            _genericMenu.AddItem(new GUIContent("GenericMenu 1"), false, () => { });
            _genericMenu.ShowAsContext();
        }
    }
}
```