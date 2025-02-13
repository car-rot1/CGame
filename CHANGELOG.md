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

v1.0.6：\
新增了Vector2IntBitArray位数组类；\
DropDownPopupWindow新增了自定义分隔符功能；\
EditorGUIExtension新增了DrawBorders重载方法；\
优化了地图生成类的算法；

v1.0.7：\
修改了DropDownPopupWindow的分隔符变量名；\
将EditorGUIExtension绘制边框的参数从int改为了float，并修复了bug；\
新增了ScriptAttributeUtilityExtension类，可使用ScriptAttributeUtility的方法；\
优化了CButton的绘制，使其能绘制在最下方，并且CButton点击时会刷新页面；\
修改了CsvFile属性面板绘制的命名空间；\
新增了ShowRichText特性以及对应的Inspector绘制；\
优化了单例拖拽限制的实现；\
为Direction枚举新增了MainFour和OtherFour变量，为Direction扩展了新的ToString方法；\
优化了Enum扩展中取余操作的实现，改为了使用位运算实现；\
优化了Rect扩展中QuadSplit的实现；\
将地图生成信息改为了可序列化，并把房间的类型改为了枚举（目前仅有起点和其他）；\
修改了SceneSwitch添加加载任务的实现；

v1.0.8：\
新增了波函数塌缩算法以及相关示例；

v1.0.9：\
新增了PropertyHandlerExtension类，存放了PropertyHandler部分变量及方法；\
为ScriptAttributeUtilityExtension新增了GetHandler方法；\
删除了ShowScriptableObjectAttribute绘制类中多余的代码；

v1.1.0：\
新增了WFCWindow窗口用于根据邻接关系生成ID；\
新增了MergeWaveBlock类，用于在波函数生成地图时显示当前可选项；\
EditorGUIExtension新增了DrawGridForSize方法；\
暂时关闭了ShowRichText特性的绘制，存在严重Bug；\
修改了Enum扩展中Contain相关的实现方式；\
为Rect扩展新增了ToIntType枚举参数，用于指定分割时的取整方式；\
为SpriteExtension转Texture2D提供了字典缓存；

v1.1.1：\
新增了PriorityQueue；\
优化了NumericalFixedBase和NumericalCustomSortBase发送OnRefreshValue事件的时机；\
修复了RequiredInterfaceMonoAttributeDrawer显示空物体时的报错bug；\
修复了单例拖拽时的窗口title为空的报错bug；

v1.1.2：\
优化了直接拖拽组件至Hierarchy窗口的功能，拖拽后会自动执行重命名命令；\
修复了CButton多选游戏物体删除时报空的bug；

v1.1.3：\
重构了Numerical模块；

v1.1.4：\
重构了本地化系统，可自定义扩展本地化资源；

v1.1.5 (2024/08/28)\
优化了LocalizationConfig的绘制；

v1.1.6 (2024/09/06)\
修复了外部多语言的文件读取Bug；

v1.1.7 (2024/09/17)\
重构了库存系统，将UI与库存绑定，实现UI自动刷新、拖拽UI以控制背包等功能，并支持自定义扩展；（文件夹中有使用示例）

v1.1.8 (2024/09/18)\
新增了一键修改全部Texture切割的Sprite的功能；

v1.1.9 (2024/12/15)\
优化了随机数扩展中取多个随机数的算法；