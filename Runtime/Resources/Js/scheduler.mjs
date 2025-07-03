import { arrayUtils } from "./array-utils.mjs";
class Scheduler {
    _fixedUpdateList = [];
    _updateList = [];
    _lateUpdateList = [];
    fixedUpdate(dt, unscaleDt) {
        const len = this._fixedUpdateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._fixedUpdateList[i];
                if (!mono.enabled)
                    continue;
                mono.fixedUpdate?.call(mono, dt, unscaleDt);
            }
            catch (e) {
                console.error(e.stack);
            }
        }
    }
    update(dt, unscaleDt) {
        const len = this._updateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._updateList[i];
                if (!mono.enabled)
                    continue;
                mono.jsUpdate(dt, unscaleDt);
            }
            catch (e) {
                console.error(e.stack);
            }
        }
    }
    lateUpdate(dt, unscaleDt) {
        const len = this._lateUpdateList.length;
        for (let i = 0; i < len; ++i) {
            try {
                const mono = this._lateUpdateList[i];
                if (!mono.enabled)
                    continue;
                mono.lateUpdate?.call(mono, dt, unscaleDt);
            }
            catch (e) {
                console.error(e.stack);
            }
        }
    }
    scheduleFixedUpdate(mono) {
        this._fixedUpdateList.push(mono);
    }
    scheduleUpdate(mono) {
        this._updateList.push(mono);
    }
    scheduleLateUpdate(mono) {
        this._lateUpdateList.push(mono);
    }
    unScheduleFixedUpdate(mono) {
        this.removeMono(this._fixedUpdateList, mono);
    }
    unScheduleUpdate(mono) {
        this.removeMono(this._updateList, mono);
    }
    unScheduleLateUpdate(mono) {
        this.removeMono(this._lateUpdateList, mono);
    }
    removeMono(list, mono) {
        const i = list.indexOf(mono);
        if (i >= 0)
            arrayUtils.fastRemoveAt(i, list);
    }
    initWithJsEngine(engine) {
        engine.jsFixUpdate = this.fixedUpdate.bind(this);
        engine.jsUpdate = this.update.bind(this);
        engine.jsLateUpdate = this.lateUpdate.bind(this);
    }
}
export const scheduler = new Scheduler();
if (CS.UnityEngine.Debug.isDebugBuild) {
    // Expose objects for console debugging
    globalThis.__debug = {
        scheduler,
        // Add other objects you want to debug here
    };
}
export function initScheduler(engine) {
    scheduler.initWithJsEngine(engine);
}
