# CGame
v1.0.0: 为该插件的正式版本，提供了如下功能

v1.0.1：\
新增了SerializedPropertyInfo类；\
完善了CButton的绘制，使其支持数组；

v1.0.2：\
新增了EditorGUIExtension；\
新增了ShowScriptableObject特性以及对应的绘制；\
优化了CButton的绘制；\
SerializedPropertyInfo新增了visible并修改了Value属性；\
完善了Button方法绘制的IMGUI方法；

v1.0.3：\
WindowAPIUtility新增了修改窗口名称的方法；\
优化了ValueToStringUtility中的StringToValue方法，使用了FormatterServices.GetUninitializedObject初始化返回值

v1.0.4：\
MapInfo新增了Start属性；\
MapGenerateBase新增了Seed变量；\
优化了CButton绘制的判空操作；

v1.0.5：\
EditorGUIExtension新增了DrawSolidRect和DrawBorders方法；\
修复了CoroutineObject失效bug；\
优化并完善了DropDownPopupWindow；\
优化了CButton的值判断；\
将地图生成传入的参数从RectInt改为了int width和int height；