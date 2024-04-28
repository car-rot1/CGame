using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace CGame
{
    public class ChangeEditorFont
    {
        public Font font;
        public FontAsset fontAsset;
        
        public void ChangeEditorResourcesFont()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));

            var editorResourceType = typeof(EditorResources);
            var supportedFonts = editorResourceType
                .GetField("s_SupportedFonts", BindingFlags.Static | BindingFlags.NonPublic)
                !.GetValue(null);

            var fontDefType = assembly.GetType("UnityEditor.Experimental.FontDef");
            var createFromResources =
                fontDefType.GetMethod("CreateFromResources", BindingFlags.Static | BindingFlags.Public)!;

            var fontDefStyleType = fontDefType.GetNestedType("Style");
            var fontDefStyleValue = fontDefStyleType.GetEnumValues();

            var fontsType = typeof(Dictionary<,>).MakeGenericType(fontDefStyleType, typeof(string));
            var fonts = Activator.CreateInstance(fontsType);
            var addMethod = fontsType.GetMethod("Add")!;
            addMethod.Invoke(fonts,
                new[] { fontDefStyleValue.GetValue(0), AssetDatabase.GetAssetPath(font) });
            addMethod.Invoke(fonts,
                new[] { fontDefStyleValue.GetValue(1), AssetDatabase.GetAssetPath(font) });
            addMethod.Invoke(fonts,
                new[] { fontDefStyleValue.GetValue(2), AssetDatabase.GetAssetPath(font) });
            addMethod.Invoke(fonts,
                new[] { fontDefStyleValue.GetValue(3), AssetDatabase.GetAssetPath(font) });
            addMethod.Invoke(fonts,
                new[] { fontDefStyleValue.GetValue(4), AssetDatabase.GetAssetPath(font) });

            var fontDef = createFromResources.Invoke(null, new[] { "CustomFont", fonts });

            supportedFonts.GetType().GetProperty("Item")!.GetSetMethod()
                .Invoke(supportedFonts, new[] { "Custom", fontDef });

            typeof(EditorResources).GetField("s_CurrentFontName", BindingFlags.Static | BindingFlags.NonPublic)
                !.SetValue(null, "Custom");
        }

        public void RecoverEditorResourcesFont()
        {
            var editorResourcesType = typeof(EditorResources);
            editorResourcesType
                .GetField("s_CurrentFontName", BindingFlags.Static | BindingFlags.NonPublic)!
                .SetValue(null,
                    editorResourcesType.GetMethod("GetDefaultFont", BindingFlags.Static | BindingFlags.NonPublic)!
                        .Invoke(null, null));
        }

        private StyleSheet styleSheet;
        
        public void ChangeStyleSheetFontAsset()
        {
            var defaultString = "UIPackageResources/Fonts/Inter/Inter-Regular SDF.asset";
            styleSheet = (StyleSheet)EditorGUIUtility.Load($"StyleSheets/Generated/DefaultCommonDark_inter.uss.asset");
            var strings = (string[])styleSheet
                .GetType()
                .GetField("strings", BindingFlags.Instance | BindingFlags.NonPublic)
                !.GetValue(styleSheet);
            for (var i = 0; i < strings.Length; i++)
            {
                if (strings[i].Equals(defaultString))
                {
                    strings[i] = AssetDatabase.GetAssetPath(fontAsset);
                }
            }
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        public void RecoverStyleSheetFontAsset()
        {
            var defaultString = "UIPackageResources/Fonts/Inter/Inter-Regular SDF.asset";
            var strings = (string[])styleSheet
                .GetType()
                .GetField("strings", BindingFlags.Instance | BindingFlags.NonPublic)
                !.GetValue(styleSheet);
            for (var i = 0; i < strings.Length; i++)
            {
                if (strings[i].Equals(AssetDatabase.GetAssetPath(fontAsset)))
                    strings[i] = defaultString;
            }
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}