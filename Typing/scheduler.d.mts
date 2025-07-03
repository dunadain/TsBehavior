import type { MonoBehaviorJs } from "./mono-behavior-js.mjs";
interface NativeJsEngine {
    jsFixUpdate(dt: number, unscaleDt: number): void;
    jsUpdate(dt: number, unscaleDt: number): void;
    jsLateUpdate(dt: number, unscaleDt: number): void;
}
declare class Scheduler {
    private _fixedUpdateList;
    private _updateList;
    private _lateUpdateList;
    fixedUpdate(dt: number, unscaleDt: number): void;
    update(dt: number, unscaleDt: number): void;
    lateUpdate(dt: number, unscaleDt: number): void;
    scheduleFixedUpdate(mono: MonoBehaviorJs): void;
    scheduleUpdate(mono: MonoBehaviorJs): void;
    scheduleLateUpdate(mono: MonoBehaviorJs): void;
    unScheduleFixedUpdate(mono: MonoBehaviorJs): void;
    unScheduleUpdate(mono: MonoBehaviorJs): void;
    unScheduleLateUpdate(mono: MonoBehaviorJs): void;
    private removeMono;
    initWithJsEngine(engine: NativeJsEngine): void;
}
export declare const scheduler: Scheduler;
export declare function initScheduler(engine: NativeJsEngine): void;
export {};
