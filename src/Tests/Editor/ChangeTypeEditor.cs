using System;
using System.Collections;
using System.Reflection;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

namespace CGame
{
    public class ChangeTypeEditor
    {
        public void Change()
        {
            var customEditorAttributesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CustomEditorAttributes");
            var monoEditorTypeType = customEditorAttributesType.GetNestedType("MonoEditorType", BindingFlags.NonPublic);
        
            var kSCustomEditors = (IDictionary)customEditorAttributesType.GetField("kSCustomEditors", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var odinTestMonoEditorTypes = (IList)kSCustomEditors[typeof(Transform)];
            foreach (var odinTestMonoEditorType in odinTestMonoEditorTypes)
            {
                Debug.Log(monoEditorTypeType.GetField("m_InspectedType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_InspectorType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_RenderPipelineType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_EditorForChildClasses").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_IsFallback").GetValue(odinTestMonoEditorType));
            }
        }
        
        // public class CGameEditor : UnityEditor.Editor
        // {
        //     public override void OnInspectorGUI()
        //     {
        //         base.OnInspectorGUI();
        //         Debug.Log("-=-=-=-");
        //     }
        // }
        //
        // [InitializeOnLoadMethod]
        // public static void Init()
        // {
        //     EditorApplication.delayCall += Changee;
        // }
        //
        // public static void Changee()
        // {
        //     var customEditorAttributesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CustomEditorAttributes");
        //     var monoEditorTypeType = customEditorAttributesType.GetNestedType("MonoEditorType", BindingFlags.NonPublic);
        //     
        //     var kSCustomEditors = (IDictionary)customEditorAttributesType.GetField("kSCustomEditors", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        //     var odinTestMonoEditorTypes = (IList)kSCustomEditors[typeof(Transform)];
        //     Debug.Log(odinTestMonoEditorTypes);
        //     odinTestMonoEditorTypes.Clear();
        //     var monoEditorType = Activator.CreateInstance(monoEditorTypeType);
        //     
        //     monoEditorTypeType.GetField("m_InspectedType").SetValue(monoEditorType, typeof(Transform));
        //     monoEditorTypeType.GetField("m_InspectorType").SetValue(monoEditorType, typeof(CGameEditor));
        //     monoEditorTypeType.GetField("m_RenderPipelineType").SetValue(monoEditorType, null);
        //     monoEditorTypeType.GetField("m_EditorForChildClasses").SetValue(monoEditorType, false);
        //     monoEditorTypeType.GetField("m_IsFallback").SetValue(monoEditorType, false);
        //     odinTestMonoEditorTypes.Add(monoEditorType);
        // }
    }
}