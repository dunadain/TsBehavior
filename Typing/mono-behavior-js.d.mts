import type { Constructor } from "./typedef.mjs";
export declare class MonoBehaviorJs {
    private _timers;
    private _enabled;
    get enabled(): boolean;
    private _nativeGameObj;
    private _nativeMonoBehavior;
    private _valid;
    get valid(): boolean;
    get nativeMonoBehavior(): CS.TsBehavior.JsMonoBehaviorHost | null;
    setNativeMonoBehavior(value: CS.TsBehavior.JsMonoBehaviorHost): void;
    get gameObject(): CS.UnityEngine.GameObject | null;
    removeComponent(className: string | Constructor): void;
    protected callByCSharpAwake(): void;
    protected awake?(): void;
    protected callByCSharpOnEnable(): void;
    protected onEnable?(): void;
    protected start?(): void;
    fixedUpdate?(dt: number, unscaleDt: number): void;
    private jsUpdate;
    protected update?(dt: number, unscaleDt: number): void;
    lateUpdate?(dt: number, unscaleDt: number): void;
    private callByCSharpOnDisable;
    protected onDisable?(): void;
    private callByCSharpOnDestroy;
    protected onDestroy?(): void;
    private scheduleFixedUpdate;
    private unscheduleFixedUpdate;
    private scheduleUpdate;
    private unscheleUpdate;
    private scheduleLateUpdate;
    private unscheduleLateUpdate;
    /**
     * 每间隔一段时间，调一下回调方法
     * @param callback 回调
     * @param interval 间隔（秒）
     * @param repeat 重复次数，-1为永远重复
     * @param delay 延迟触发时间
     */
    protected schedule(callback: (dt?: number) => void, interval: number, repeat?: number, delay?: number): void;
    protected unschedule(callback: (dt?: number) => void): void;
    protected unscheduleAll(): void;
    /**
     * 延迟
     * @param period 延迟的时间，单位秒
     * @returns
     */
    delay(period: number): Promise<void>;
}
