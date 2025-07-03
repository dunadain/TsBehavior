using System;
using System.Collections.Generic;
using Puerts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TsBehavior
{
    public static class GameObjExtensions
    {
        // private static Func<JSObject, string, bool> _instanceof;
        // private static Func<JSObject, string, bool> Instanceof
        // {
        //     get
        //     {
        //         _instanceof ??= JsEngine.Instance.JsEnv.ExecuteModule<Func<JSObject, string, bool>>(
        //             "Js/js-utils.mjs",
        //             "isInstanceof");
        //         return _instanceof;
        //     }
        // }
        // public static void Clear()
        // {
        //     _instanceof = null;
        // }
        // /// <summary>
        // /// 获取js component
        // /// </summary>
        // /// <param name="go"></param>
        // /// <param name="jsClassName"></param>
        // /// <param name="includeInheritance">如果为true，js component可以为jsclassName对应类的子类</param>
        // /// <returns></returns>
        // public static JSObject GetJsComponent(this GameObject go, string jsClassName, bool includeInheritance = false)
        // {
        //     var hosts = go.GetComponents<JsMonoBehaviorHost>();
        //     foreach (var jsMonoBehaviorHost in hosts)
        //     {
        //         var jsObj = jsMonoBehaviorHost.MyJsComp();
        //         if (jsMonoBehaviorHost.jsClassName == jsClassName) return jsObj;
        //         if (includeInheritance && Instanceof(jsObj, jsClassName)) return jsObj;
        //     }
        //     return null;
        // }

        // public static List<JSObject> GetJsComponents(this GameObject go, string jsClassName, bool includeInheritance = false)
        // {
        //     var list = new List<JSObject>();
        //     var hosts = go.GetComponents<JsMonoBehaviorHost>();
        //     foreach (var jsMonoBehaviorHost in hosts)
        //     {
        //         var jsObj = jsMonoBehaviorHost.MyJsComp();
        //         if (jsMonoBehaviorHost.jsClassName == jsClassName) list.Add(jsObj);
        //         else if (includeInheritance && Instanceof(jsObj, jsClassName)) list.Add(jsObj);
        //     }
        //     return list;
        // }

        // public static JSObject GetJsComponentInChildren(this GameObject go, string jsClassName, bool includeInheritance = false)
        // {
        //     var hosts = go.GetComponentsInChildren<JsMonoBehaviorHost>();
        //     foreach (var jsMonoBehaviorHost in hosts)
        //     {
        //         var jsObj = jsMonoBehaviorHost.MyJsComp();
        //         if (jsMonoBehaviorHost.jsClassName == jsClassName) return jsObj;
        //         if (includeInheritance && Instanceof(jsObj, jsClassName)) return jsObj;
        //     }
        //     return null;
        // }

        public static JsMonoBehaviorHost GetMonoBehaviourHost(this GameObject go, string jsClassName)
        {
            var hosts = go.GetComponents<JsMonoBehaviorHost>();
            foreach (var jsMonoBehaviorHost in hosts)
            {
                if (jsMonoBehaviorHost.jsClassName == jsClassName) return jsMonoBehaviorHost;
            }
            return null;
        }

        public static List<JsMonoBehaviorHost> GetMonoBehaviourHosts(this GameObject go, string jsClassName)
        {
            var list = new List<JsMonoBehaviorHost>();
            var hosts = go.GetComponents<JsMonoBehaviorHost>();
            foreach (var jsMonoBehaviorHost in hosts)
            {
                if (jsMonoBehaviorHost.jsClassName == jsClassName)
                {
                    list.Add(jsMonoBehaviorHost);
                }
            }
            return list;
        }

        public static JSObject AddJsComponent(this GameObject go, string jsClassName)
        {
            var newComp = go.AddComponent<JsMonoBehaviorHost>();
            newComp.jsClassName = jsClassName;
            if (newComp.enabled)
            {
                newComp.InitWithJsClass(jsClassName);
                newComp.jsAwake();
                newComp.jsOnEnable();
            }
            return newComp.MyJsComp();
        }

        public static bool IsNull(this Object o)
        {
            return o == null;
        }
    }

}