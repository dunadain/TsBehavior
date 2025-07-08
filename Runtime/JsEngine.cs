using System;
using Puerts;
using UnityEngine;

namespace TsBehavior
{
    public class JsEngine : MonoBehaviour
    {
        public static JsEngine Instance { get; private set; } = null;

        private JsEnv _jsEnv;
        public JsEnv JsEnv => _jsEnv;

        public Action<float, float> jsFixUpdate;
        public Action<float, float> jsUpdate;
        public Action<float, float> jsLateUpdate;

        private Func<string, JsMonoBehaviorHost, JSObject> _createJs;

        public JSObject CreateJsMono(string jsClassName, JsMonoBehaviorHost host)
        {
            _createJs ??= _jsEnv.ExecuteModule<Func<string, JsMonoBehaviorHost, JSObject>>("tsbehavior/js-utils.mjs", "createMonoJs");
            return _createJs(jsClassName, host);
        }

        private Action<JSObject, string, object[]> _trycallFun;

        public Action<JSObject, string, object[]> TryCallFunction
        {
            get
            {
                _trycallFun ??= _jsEnv.ExecuteModule<Action<JSObject, string, object[]>>("tsbehavior/js-utils.mjs", "tryCall");
                return _trycallFun;
            }
        }

        public void Init(JsEnv jsEnv)
        {
            _jsEnv = jsEnv;
            _jsEnv.ExecuteModule("tsbehavior/array-utils.mjs");
            _jsEnv.ExecuteModule("tsbehavior/js-utils.mjs");
            _jsEnv.ExecuteModule("tsbehavior/mono-behavior-js.mjs");
            _jsEnv.ExecuteModule("tsbehavior/scheduler.mjs");
            _jsEnv.ExecuteModule("tsbehavior/timer-pool.mjs");
            _jsEnv.ExecuteModule("tsbehavior/index.mjs");

            var initScheduler = _jsEnv.ExecuteModule<Action<JsEngine>>("tsbehavior/scheduler.mjs", "initScheduler");
            initScheduler(this);
        }

        void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            jsFixUpdate?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
        }
        private void Update()
        {
            _jsEnv?.Tick();
            jsUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }
        private void LateUpdate()
        {
            jsLateUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        void OnDestroy()
        {
            jsFixUpdate = null;
            jsUpdate = null;
            jsLateUpdate = null;
            _createJs = null;
            _trycallFun = null;
            _jsEnv?.Dispose();
            _jsEnv = null;
            Instance = null;
        }
    }
}