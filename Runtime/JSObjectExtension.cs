using System;
using System.Collections.Generic;
using Puerts;

namespace TsBehavior
{
    public static class JSObjectExtension
    {
        public static void TryCall(this JSObject jsObject, string methodName, params object[] args)
        {
            JsEngine.Instance.TryCallFunction.Invoke(jsObject, methodName, args);
        }
    }
}