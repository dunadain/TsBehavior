import { arrayUtils } from "./array-utils.mjs";
import type { MonoBehaviorJs } from "./mono-behavior-js.mjs";


interface NativeJsEngine {
    jsFixUpdate(dt: number, unscaleDt: number): void;
    jsUpdate(dt: number, unscaleDt: number): void;
    jsLateUpdate(dt: number, unscaleDt: number): void;
}

class Scheduler {
    private _fixedUpdateList: MonoBehaviorJs[] = [];
    private _updateList: MonoBehaviorJs[] = [];
    private _lateUpdateList: MonoBehaviorJs[] = [];

    fixedUpdate(dt: number, unscaleDt: number) {
        const len = this._fixedUpdateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._fixedUpdateList[i];
                if (!mono.enabled) continue;
                mono.fixedUpdate?.call(mono, dt, unscaleDt);
            } catch (e) {
                console.error((e as Error).stack);
            }
        }
    }

    update(dt: number, unscaleDt: number) {
        const len = this._updateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._updateList[i];
                if (!mono.enabled) continue;
                (mono as any).jsUpdate(dt, unscaleDt);
            } catch (e) {
                console.error((e as Error).stack);
            }
        }
    }

    lateUpdate(dt: number, unscaleDt: number) {
        const len = this._lateUpdateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._lateUpdateList[i];
                if (!mono.enabled) continue;
                mono.lateUpdate?.call(mono, dt, unscaleDt);
            } catch (e) {
                console.error((e as Error).stack);
            }
        }
    }

    scheduleFixedUpdate(mono: MonoBehaviorJs) {
        this._fixedUpdateList.push(mono);
    }

    scheduleUpdate(mono: MonoBehaviorJs) {
        this._updateList.push(mono);
    }

    scheduleLateUpdate(mono: MonoBehaviorJs) {
        this._lateUpdateList.push(mono);
    }

    unScheduleFixedUpdate(mono: MonoBehaviorJs) {
        this.removeMono(this._fixedUpdateList, mono);
    }

    unScheduleUpdate(mono: MonoBehaviorJs) {
        this.removeMono(this._updateList, mono);
    }

    unScheduleLateUpdate(mono: MonoBehaviorJs) {
        this.removeMono(this._lateUpdateList, mono);
    }

    private removeMono(list: MonoBehaviorJs[], mono: MonoBehaviorJs) {
        const i = list.indexOf(mono);
        if (i >= 0)
            arrayUtils.fastRemoveAt(i, list);
    }

    initWithJsEngine(engine: NativeJsEngine) {
        engine.jsFixUpdate = this.fixedUpdate.bind(this);
        engine.jsUpdate = this.update.bind(this);
        engine.jsLateUpdate = this.lateUpdate.bind(this);
    }
}

export const scheduler = new Scheduler();

if (CS.UnityEngine.Debug.isDebugBuild) {
    // Expose objects for console debugging
    (globalThis as any).__debug = {
        scheduler,
        // Add other objects you want to debug here
    };
}

export function initScheduler(engine: NativeJsEngine) {
    scheduler.initWithJsEngine(engine);
}