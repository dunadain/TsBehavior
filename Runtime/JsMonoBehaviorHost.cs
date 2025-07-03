using System;
using System.Collections.Generic;
using System.Linq;
using Puerts;
using UnityEngine;

namespace TsBehavior
{
    public class JsMonoBehaviorHost : MonoBehaviour
    {
        public string jsClassName = "";
#if UNITY_EDITOR
        [SerializeField]
        private string jsClassPath = "";
        // public List<string> singletonList = new();
#endif

        public List<Property> properties;
        public List<ListProperty> listProperties;

        /// <summary>
        /// c# 与 js 数据共享数据缓存
        /// </summary>
        public Dictionary<string, object> shareData = new();

        public Action jsAwake;
        public Action jsOnEnable;
        public Action jsStart;
        public Action jsOnDisable;
        public Action jsOnDestroy;

        // 对应的js对象
        private JSObject _jsComp;

        public JSObject JsComponent()
        {
            return _jsComp;
        }

        /// <summary>
        /// 获取ts内定义的序列化对象
        /// </summary>
        protected T GetObjProperty<T>(string propertyName) where T : class
        {
            // return (from property in properties where property.name == propertyName select property.value.objValue as T).FirstOrDefault();
            return properties.FirstOrDefault(property => property.name == propertyName)?.value.objValue as T;
        }

        public void DestroyJsComponent(string jsClazz)
        {
            var c = gameObject.GetMonoBehaviourHost(jsClazz);
            if (c) Destroy(c);
        }

        public void InitWithJsClass(string className)
        {
            _jsComp = JsEngine.Instance.CreateJsMono(className, this);
        }
        protected void Awake()
        {
            if (jsClassName != "") InitWithJsClass(jsClassName);
            try
            {
                jsAwake?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnEnable()
        {
            try
            {
                jsOnEnable?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void Start()
        {
            try
            {
                jsStart?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnDisable()
        {
            try
            {
                jsOnDisable?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

        }

        private void OnDestroy()
        {
            shareData.Clear();
            jsAwake = null;
            jsOnEnable = null;
            jsStart = null;
            jsOnDisable = null;
            _jsComp = null;
            try
            {
                jsOnDestroy?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            jsOnDestroy = null;
        }
    }
}