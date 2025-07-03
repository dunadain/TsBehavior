using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    [CustomEditor(typeof(JsMonoBehaviorHost), true)]
    public class MonoJsInspector : UnityEditor.Editor
    {
        private List<JsProperty> _jsProperties;
        private List<JsProperty> _jsListProperties;
        private SerializedProperty _monoProperties;
        private SerializedProperty _monoListProperties;
        private SerializedProperty _jsClassProp;
        private SerializedProperty _jsClassPathProp;
        private VisualElement customInspectorContainer;

        [SerializeField] private VisualTreeAsset monoInspectorXML;
        [SerializeField] private VisualTreeAsset listViewXML;

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            _jsClassProp = serializedObject.FindProperty("jsClassName");
            _jsClassPathProp = serializedObject.FindProperty("jsClassPath");
            _monoProperties = serializedObject.FindProperty("properties");
            _monoListProperties = serializedObject.FindProperty("listProperties");

            var myInspector = new VisualElement();
            monoInspectorXML.CloneTree(myInspector);
            customInspectorContainer = myInspector.Q("Custom_Inspector");

            setupInspector();

            // var defaultInspectorFoldout = myInspector.Q("Default_Inspector");
            // InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);

            var reloadBtn = myInspector.Q<Button>("reloadBtn");
            reloadBtn.clicked += () =>
            {
                myInspector.Unbind();
                customInspectorContainer.Clear();
                setupInspector();
                myInspector.Bind(serializedObject);
            };
            return myInspector;
        }

        private void setupInspector()
        {
            var className = _jsClassProp.stringValue;
            if (string.IsNullOrEmpty(className)) return;
            var classPath = GetClassPath(className);
            if (classPath == "")
            {
                Debug.LogError(className + " does not exist!");
                return;
            }

            if (GetClassNameFromPath(_jsClassPathProp.stringValue) != className + ".mts")
            {
                _jsClassPathProp.stringValue = classPath;
                if (_monoProperties.arraySize > 0)
                    _monoProperties.arraySize = 0;
                if (_monoListProperties.arraySize > 0)
                    _monoListProperties.arraySize = 0;
            }

            _jsProperties = new List<JsProperty>();
            _jsListProperties = new List<JsProperty>();

#if UNITY_EDITOR_WIN
                classPath = classPath.Replace("/", "\\");
#endif
            GenJsPropLists(classPath, className);
            updateProperties(_monoProperties, _jsProperties);
            updateProperties(_monoListProperties, _jsListProperties);
            serializedObject.ApplyModifiedProperties();
            var size = _jsProperties.Count;
            for (var i = 0; i < size; ++i)
            {
                var jsProp = _jsProperties[i];
                var creator = FieldFactory.Create(jsProp, _monoProperties.GetArrayElementAtIndex(i));
                customInspectorContainer.Add(creator.getField());
            }

            size = _jsListProperties.Count;
            for (var i = 0; i < size; ++i)
            {
                var jsProp = _jsListProperties[i];
                var templateContainer = listViewXML.CloneTree();
                var listView = templateContainer.Q<ListView>();

                listView.bindingPath = "listProperties.Array.data[" + i + "].list";
                listView.headerTitle = jsProp.name;
                var creator = FieldFactory.Create(jsProp, null);
                listView.makeItem = creator.makeItem;
                customInspectorContainer.Add(templateContainer);
            }
        }

        // 序列化的都是login/loginview.mts, mac linux格式的路径
        private string GetClassNameFromPath(string relativePath)
        {
            var index = relativePath.LastIndexOf("/", StringComparison.Ordinal);
            return relativePath[(index + 1)..];
        }

        private void updateProperties(SerializedProperty monoProperty, IReadOnlyList<JsProperty> list)
        {
            // set all items in old properties to true, so all the false properties are newly added
            var nameDic = new Dictionary<string, bool>();
            var propName2IndexDic = new Dictionary<string, int>();
            var jsPropDic = new Dictionary<string, JsProperty>();
            // 保存一些基础信息
            for (var i = 0; i < list.Count; ++i)
            {
                var key = list[i].name;
                nameDic[key] = false;
                propName2IndexDic[key] = i;
                jsPropDic[key] = list[i];
            }

            // delete unused items in the serialized property array
            var size = monoProperty.arraySize;
            for (var i = size - 1; i >= 0; --i)
            {
                var singleProp = monoProperty.GetArrayElementAtIndex(i);
                var nameProp = singleProp.FindPropertyRelative("name");
                var key = nameProp.stringValue;
                if (nameDic.ContainsKey(key) && singleProp.FindPropertyRelative("typeName").stringValue == jsPropDic[key].type.FullName)
                {
                    nameDic[key] = true;
                }
                else
                {
                    monoProperty.DeleteArrayElementAtIndex(i);
                }
            }

            // add new properties in the serialized property array. And reset newly added properties' serialized properties
            foreach (var pair in nameDic.Where(pair => pair.Value == false))
            {
                monoProperty.InsertArrayElementAtIndex(monoProperty.arraySize);
                var newProp = monoProperty.GetArrayElementAtIndex(monoProperty.arraySize - 1);
                var jsProp = list[propName2IndexDic[pair.Key]];
                newProp.FindPropertyRelative("name").stringValue = jsProp.name;
                newProp.FindPropertyRelative("typeName").stringValue = jsProp.type.FullName;
                var valueProp = newProp.FindPropertyRelative("value");
                if (valueProp is null) continue;
                valueProp.FindPropertyRelative("objValue").objectReferenceValue = null;
                valueProp.FindPropertyRelative("primitiveValue").stringValue = jsProp.defaultValue;
            }

            // sort
            var len = list.Count;
            for (var i = 0; i < len; ++i)
            {
                var jsProp = list[i];
                var propName = monoProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                if (jsProp.name == propName) continue;
                for (var j = i + 1; j < len; ++j)
                {
                    propName = monoProperty.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue;
                    if (propName != jsProp.name) continue;
                    monoProperty.MoveArrayElement(j, i);
                    break;
                }
            }
        }

        private void GenJsPropLists(string relativeClassPath, string className)
        {
            var jsPath = Path.Join(Application.dataPath, "..", "tsscripts");
            var classPath = Path.Join(jsPath, "src", relativeClassPath);
            var ast = TsUtils.GetAstFromPath(classPath);

            ///////// FOR DEBUGGING ONLY/////////
            // // Get the path to the file
            // string filePath = jsPath + $"/{className}.txt";
            // // Check if the file exists
            // if (File.Exists(filePath))
            // {
            //     // Delete the file
            //     File.Delete(filePath);
            // }

            // // Create a StreamWriter to write to the file
            // StreamWriter writer = new StreamWriter(filePath, true); // True for appending

            // // Write the text to the file
            // writer.WriteLine(ast);

            // // Close the StreamWriter
            // writer.Close();
            ///////// FOR DEBUGGING ONLY/////////

            if (string.IsNullOrEmpty(ast))
            {
                Debug.LogError("Failed to get ast from " + classPath);
                return;
            }
            var dict = MiniJSON.Json.Deserialize(ast) as Dictionary<string, object>;
            var body = (List<object>)dict["body"];
            object classBody = null;
            foreach (var item in body)
            {
                var itemDict = (Dictionary<string, object>)item;
                if (!itemDict.ContainsKey("declaration")) continue;
                var declarationDict = itemDict["declaration"] as Dictionary<string, object>;
                var idDict = declarationDict?["id"] as Dictionary<string, object>;
                var declarationType = declarationDict?["type"].ToString();
                if (itemDict["type"].ToString() != "ExportNamedDeclaration"
                    || declarationType != "ClassDeclaration"
                    || idDict?["name"].ToString() != className) continue;
                var decorators = declarationDict["decorators"] as List<object>;
                if (decorators.Count == 0)
                {
                    Debug.LogError("缺少类装饰器, 请添加：" + @"@mapleclass('" + className + "')");
                    return;
                }
                foreach (var decorator in decorators)
                {
                    var decoratorDict = decorator as Dictionary<string, object>;
                    if (decoratorDict["expression"] is Dictionary<string, object> expressionDict
                        && expressionDict["callee"] is Dictionary<string, object> calleeDict
                        && calleeDict["name"].ToString() != "mapleclass")
                    {
                        Debug.LogError("缺少类装饰器, 请添加：" + @"@mapleclass('" + className + "')");
                    }
                }
                var bodyDict = declarationDict["body"] as Dictionary<string, object>;
                classBody = bodyDict?["body"];
                break;
            }

            if (classBody is null)
            {
                Debug.LogError("Failed to get class body from " + classPath);
                return;
            }

            var classBodyList = classBody as List<object>;
            foreach (var item in classBodyList)
            {
                var itemDict = item as Dictionary<string, object>;
                if (itemDict["type"].ToString() != "PropertyDefinition") continue;
                var decorators = itemDict["decorators"] as List<object>;
                if (decorators.Count == 0) continue;

                var decoratorName = "";
                var decorator = decorators[0] as Dictionary<string, object>;

                if (decorator["expression"] is Dictionary<string, object> expressionDict)
                {
                    if (expressionDict["type"].ToString() == "Identifier")
                    {
                        decoratorName = expressionDict["name"].ToString();
                    }
                    else if (expressionDict["type"].ToString() == "CallExpression")
                    {
                        var calleeDict = expressionDict["callee"] as Dictionary<string, object>;
                        decoratorName = calleeDict["name"].ToString();
                    }
                }
                if (decoratorName != "property" && decoratorName != "listproperty" && decoratorName != "enumfield") continue;
                var expressionDict1 = decorator["expression"] as Dictionary<string, object>;
                var decoratorArgs = TsUtils.GetDecoratorArgs(expressionDict1["arguments"] as List<object>);
                var propName = (itemDict["key"] as Dictionary<string, object>)["name"].ToString();
                if (decoratorName == "enumfield")
                {
                    var typeAnnotationDict = itemDict["typeAnnotation"] as Dictionary<string, object>;
                    var typeAnnotationInnerDict = typeAnnotationDict?["typeAnnotation"] as Dictionary<string, object>;
                    var typeAnnotation = TsUtils.GetTypeAnnotation(typeAnnotationInnerDict["typeName"] as Dictionary<string, object>);
                    var enumNameList = TsUtils.GetEnumDic(typeAnnotation, classPath, dict);
                    var jsProp = new JsProperty
                    {
                        name = propName,
                        type = typeof(string),
                        isEnum = true,
                        info = enumNameList,
                    };
                    _jsProperties.Add(jsProp); // 应该不会有enumlist这种离谱需求吧
                }
                else
                {
                    var typeName = decoratorArgs[0];

                    var jsProp = new JsProperty
                    {
                        name = propName,
                        type = AssemblyUtils.GetType(typeName[3..]),
                    };
                    if (typeName == "TsBehavior.JsMonoBehaviorHost")
                    {
                        var typeAnnotationDict = itemDict["typeAnnotation"] as Dictionary<string, object>;
                        var typeAnnotationInnerDict = typeAnnotationDict?["typeAnnotation"] as Dictionary<string, object>;
                        var typeAnnotation = "";
                        if (typeAnnotationInnerDict is not null)
                        {
                            if ((string)typeAnnotationInnerDict["type"] == "TSArrayType")
                            {
                                var typeDict = (typeAnnotationInnerDict["elementType"] as Dictionary<string, object>)["typeName"] as Dictionary<string, object>;
                                typeAnnotation = TsUtils.GetTypeAnnotation(typeDict);
                            }
                            else if ((string)typeAnnotationInnerDict["type"] == "TSTypeReference")
                            {
                                typeAnnotation = TsUtils.GetTypeAnnotation(typeAnnotationInnerDict["typeName"] as Dictionary<string, object>);
                            }
                        }
                        if (typeAnnotation == "") throw new Exception("Failed to get type annotation for " + propName);
                        jsProp.info = typeAnnotation;
                    }
                    jsProp.defaultValue = itemDict["value"] is not Dictionary<string, object> valueDict ? "" : valueDict["value"] != null ? valueDict["value"].ToString() : "";
                    if (decoratorName == "listproperty") _jsListProperties.Add(jsProp);
                    else _jsProperties.Add(jsProp);
                }
            }
        }

        private string GetClassPath(string className)
        {
            var tsSrc = Path.Join(Application.dataPath, "..", "tsscripts");
            tsSrc = Path.Join(tsSrc, "src");
            var path = _jsClassPathProp.stringValue;
            var index = path.LastIndexOf("/", StringComparison.Ordinal);
            if (path[(index + 1)..] != className + ".mts") path = "";
#if UNITY_EDITOR_WIN
            path = path.Replace("/", "\\");
#endif
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) && File.Exists(Path.Join(tsSrc, path))) return _jsClassPathProp.stringValue;
            if (string.IsNullOrEmpty(className) || string.IsNullOrWhiteSpace(className)) return "";
            return FindTsPathInDir(className, tsSrc, "");
        }

        private static string FindTsPathInDir(string tsClassName, string dirPath, string relativePath)
        {
            var tsFileName = tsClassName + ".mts";
            var dirInfo = new DirectoryInfo(dirPath);
            var infos = dirInfo.GetFiles("*.mts");
            for (var i = 0; i < infos.Length; ++i)
            {
                var fileInfo = infos[i];
                if (fileInfo.Name == tsFileName)
                {
                    var finalPath = Path.Join(relativePath, fileInfo.Name);
#if UNITY_EDITOR_WIN
                    finalPath = finalPath.Replace("\\", "/");
#endif
                    return finalPath;
                }
            }

            var subDirs = dirInfo.GetDirectories();
            for (var i = 0; i < subDirs.Length; ++i)
            {
                var subDirInfo = subDirs[i];
                var result = FindTsPathInDir(tsClassName, Path.Join(dirPath, subDirInfo.Name), Path.Join(relativePath, subDirInfo.Name));
                if (result != "") return result;
            }

            return "";
        }
    }
}