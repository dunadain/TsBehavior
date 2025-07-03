using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TsBehavior
{
    public static class TsUtils
    {
        [DllImport("tsparser")]
        private static extern string get_ast_from_src(string src, string filepath);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAstFromSrc(string src, string filepath)
        {
            return get_ast_from_src(src, filepath);
        }


        [DllImport("tsparser")]
        private static extern string get_ast_from_path(string filepath);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAstFromPath(string filepath)
        {
            return get_ast_from_path(filepath);
        }

        public static List<string> GetEnumDic(string enumAnnotation, string classPath, Dictionary<string, object> root)
        {
            var enumBody = FindFromRoot("TSEnumDeclaration", enumAnnotation, root);
            if (enumBody == null)
            {
                var firstDotIndex = enumAnnotation.IndexOf('.');
                var importName = firstDotIndex >= 0 ? enumAnnotation[..firstDotIndex] : enumAnnotation;
                var body = (List<object>)root["body"];
                foreach (var item in body)
                {
                    var itemDict = (Dictionary<string, object>)item;
                    if (itemDict["type"].ToString() == "ImportDeclaration")
                    {
                        var specifiers = (List<object>)itemDict["specifiers"];
                        foreach (var specifier in specifiers)
                        {
                            var specifierDict = (Dictionary<string, object>)specifier;
                            if (specifierDict["type"].ToString() == "ImportSpecifier")
                            {
                                var imported = (Dictionary<string, object>)specifierDict["imported"];
                                if (imported["name"].ToString() == importName)
                                {
                                    var sourceDict = (Dictionary<string, object>)itemDict["source"];
                                    var importPath = sourceDict["value"].ToString();
                                    var arr = importPath.Split('/');
                                    var index = classPath.LastIndexOf(Path.DirectorySeparatorChar);
                                    var enumFilePath = classPath[..index];
                                    for (var i = 0; i < arr.Length; ++i)
                                    {
                                        enumFilePath = Path.Join(enumFilePath, arr[i]);
                                    }
                                    enumFilePath = Path.ChangeExtension(enumFilePath, ".mts");
                                    if (!File.Exists(enumFilePath))
                                    {
                                        throw new Exception($"ts file {enumFilePath} not found");
                                    }
                                    var ast = GetAstFromPath(enumFilePath);
                                    var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(ast);
                                    enumBody = FindFromRoot("TSEnumDeclaration", enumAnnotation, json);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (enumBody == null)
            {
                throw new Exception($"Enum {enumAnnotation} not found");
            }

            var list = new List<string>();
            var members = (List<object>)enumBody["members"];
            // var curValue = 0;
            foreach (var member in members)
            {
                var memberDict = (Dictionary<string, object>)member;
                var initializer = (Dictionary<string, object>)memberDict["initializer"];
                if (initializer != null)
                {
                    if ((string)initializer["type"] == "StringLiteral")
                    {
                        throw new Exception("enum value must be number");
                    }

                }
                var idDict = (Dictionary<string, object>)memberDict["id"];
                var name = idDict["name"].ToString();
                list.Add(name);
                // var initializer = (Dictionary<string, object>)memberDict["initializer"];
                // if (initializer == null)
                // {
                //     dic.Add(name, curValue++);
                // }
                // else
                // {
                //     dic.Add(name, initializer["value"]);
                //     if (initializer["type"].ToString() == "Literal")
                //     {
                //         curValue = Convert.ToInt32(initializer["value"]) + 1;
                //     }
                // }
            }
            return list;
        }

        private static Dictionary<string, object> FindFromRoot(string typeName, string annotation, Dictionary<string, object> body)
        {
            if ((string)body["type"] == typeName)
            {
                var idDict = (Dictionary<string, object>)body["id"];
                if (idDict["name"].ToString() == annotation)
                {
                    return body;
                }
            }
            else if ((string)body["type"] == "Program")
            {
                var bodyList = body["body"] as List<object>;
                foreach (var item in bodyList)
                {
                    var result = FindFromRoot(typeName, annotation, (Dictionary<string, object>)item);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else if ((string)body["type"] == "TSModuleDeclaration")
            {
                var index = annotation.IndexOf('.');
                var idDict = (Dictionary<string, object>)body["id"];
                if (idDict["name"].ToString() == annotation[..index])
                {
                    var moduleBlock = body["body"] as Dictionary<string, object>;
                    var bodyList = moduleBlock["body"] as List<object>;
                    foreach (var item in bodyList)
                    {
                        var result = FindFromRoot(typeName, annotation[(index + 1)..], (Dictionary<string, object>)item);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            else if ((string)body["type"] == "ExportNamedDeclaration")
            {
                var declaration = (Dictionary<string, object>)body["declaration"];
                var result = FindFromRoot(typeName, annotation, declaration);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static string GetTypeAnnotation(Dictionary<string, object> typeName)
        {
            var results = new List<string>();
            ExtractAnnotationParts(typeName, results);
            return string.Join(".", results);
        }

        private static void ExtractAnnotationParts(Dictionary<string, object> typeName, List<string> results)
        {
            if ((string)typeName["type"] == "TSQualifiedName")
            {
                ExtractAnnotationParts((Dictionary<string, object>)typeName["left"], results);
                var right = (Dictionary<string, object>)typeName["right"];
                results.Add((string)right["name"]);
            }
            else if ((string)typeName["type"] == "Identifier")
            {
                results.Add((string)typeName["name"]);
            }
        }

        public static List<string> GetDecoratorArgs(List<object> args)
        {
            var result = new List<string>();
            foreach (var arg in args)
            {
                var argDict = (Dictionary<string, object>)arg;
                result.Add(GetArg(argDict));
            }

            return result;
        }

        private static string GetArg(Dictionary<string, object> arg)
        {
            var list = new List<string>();
            ExtractArg(arg, list);
            return string.Join(".", list);
        }

        private static void ExtractArg(Dictionary<string, object> arg, List<string> results)
        {
            if ((string)arg["type"] == "StaticMemberExpression")
            {
                ExtractArg((Dictionary<string, object>)arg["object"], results);
                var property = (Dictionary<string, object>)arg["property"];
                results.Add(property["name"].ToString());
            }
            else if ((string)arg["type"] == "Identifier")
            {
                results.Add((string)arg["name"]);
            }
        }
    }
}