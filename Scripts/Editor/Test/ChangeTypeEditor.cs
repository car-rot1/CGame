using System.Collections;
using System.Reflection;
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
            var odinTestMonoEditorTypes = (IList)kSCustomEditors[typeof(Test)];
            foreach (var odinTestMonoEditorType in odinTestMonoEditorTypes)
            {
                Debug.Log(monoEditorTypeType.GetField("m_InspectedType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_InspectorType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_RenderPipelineType").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_EditorForChildClasses").GetValue(odinTestMonoEditorType));
                Debug.Log(monoEditorTypeType.GetField("m_IsFallback").GetValue(odinTestMonoEditorType));
            }
        }
    }
}