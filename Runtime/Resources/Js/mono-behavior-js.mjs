import { arrayUtils } from "./array-utils.mjs";
import { scheduler } from "./scheduler.mjs";
import { TimerPool } from "./timer-pool.mjs";
export class MonoBehaviorJs {
    _timers = [];
    _enabled = false;
    get enabled() {
        return this._enabled;
    }
    _nativeGameObj = null;
    _nativeMonoBehavior = null;
    _valid = true;
    get valid() { return this._valid; }
    get nativeMonoBehavior() {
        return this._nativeMonoBehavior;
    }
    setNativeMonoBehavior(value) {
        this._nativeMonoBehavior = value;
        this._nativeGameObj = value.gameObject;
    }
    get gameObject() {
        return this._nativeGameObj;
    }
    // getComponent(className: string) {
    //     return this.gameObject.GetJsComponent(className);
    // }
    removeComponent(className) {
        const name = typeof className == 'string' ? className : className.name;
        this._nativeMonoBehavior?.DestroyJsComponent(name);
    }
    callByCSharpAwake() {
        this.scheduleFixedUpdate();
        this.scheduleUpdate();
        this.scheduleLateUpdate();
        this.awake?.call(this);
    }
    callByCSharpOnEnable() {
        this._enabled = true;
        this.onEnable?.call(this);
    }
    jsUpdate(dt, unscaleDt) {
        this.update?.call(this, dt, unscaleDt);
        if (this._timers) {
            for (let i = this._timers.length - 1; i >= 0; --i) {
                const timer = this._timers[i];
                timer.update(dt);
                if (!timer.valid) {
                    const removed = arrayUtils.fastRemoveAt(i, this._timers);
                    if (removed) {
                        TimerPool.recycle(removed);
                    }
                }
            }
        }
    }
    callByCSharpOnDisable() {
        this._enabled = false;
        this.onDisable?.call(this);
    }
    callByCSharpOnDestroy() {
        this.onDestroy?.call(this);
        this._nativeGameObj = null;
        this._nativeMonoBehavior = null;
        this.unscheduleFixedUpdate();
        this.unscheleUpdate();
        this.unscheduleLateUpdate();
        this.unscheduleAll();
        // 关键是吧c#的引用删了，防止循环引用
        // 确实应该这样，不然有人会在这个对象已被摧毁的情况下 继续这里的load方法 这样会造成泄漏
        for (const key in this) {
            delete this[key];
        }
        this._valid = false;
    }
    scheduleFixedUpdate() {
        scheduler.scheduleFixedUpdate(this);
    }
    unscheduleFixedUpdate() {
        scheduler.unScheduleFixedUpdate(this);
    }
    scheduleUpdate() {
        scheduler.scheduleUpdate(this);
    }
    unscheleUpdate() {
        scheduler.unScheduleUpdate(this);
    }
    scheduleLateUpdate() {
        scheduler.scheduleLateUpdate(this);
    }
    unscheduleLateUpdate() {
        scheduler.unScheduleLateUpdate(this);
    }
    /**
     * 每间隔一段时间，调一下回调方法
     * @param callback 回调
     * @param interval 间隔（秒）
     * @param repeat 重复次数，-1为永远重复
     * @param delay 延迟触发时间
     */
    schedule(callback, interval, repeat = -1, delay = 0) {
        // 检查是否已经注册
        for (let i = 0; i < this._timers.length; ++i) {
            if (this._timers[i].callback == callback) {
                return;
            }
        }
        const timer = TimerPool.get();
        timer.callback = callback;
        timer.interval = interval;
        timer.repeat = repeat;
        timer.delay = delay;
        timer.target = this;
        this._timers.push(timer);
    }
    unschedule(callback) {
        for (let i = 0; i < this._timers.length; ++i) {
            if (this._timers[i].callback == callback) {
                const removed = arrayUtils.fastRemoveAt(i, this._timers);
                if (removed)
                    TimerPool.recycle(removed);
            }
        }
    }
    unscheduleAll() {
        for (let i = 0; i < this._timers.length; ++i) {
            TimerPool.recycle(this._timers[i]);
        }
        this._timers.length = 0;
    }
    /**
     * 延迟
     * @param period 延迟的时间，单位秒
     * @returns
     */
    delay(period) {
        return new Promise(resolve => {
            this.schedule(() => {
                resolve();
            }, 0, 0, period);
        });
    }
}
