export class Timer {
    valid = true;
    delay = 0;
    repeat = -1;
    interval = 0;
    target = null;
    callback = null;
    _counter = 0;
    _delayPassed = false;
    update(dt) {
        if (!this._delayPassed) {
            this._counter += dt;
            if (this._counter >= this.delay) {
                this._delayPassed = true;
                this.callback?.call(this.target, this._counter);
                this._counter = 0;
                if (this.repeat == 0)
                    this.valid = false;
            }
        }
        else {
            this._counter += dt;
            while (this._counter >= this.interval) {
                this._counter -= this.interval;
                this.callback?.call(this.target, this.interval);
                if (this.repeat > 0) {
                    --this.repeat;
                    if (this.repeat == 0) {
                        this.valid = false;
                        break;
                    }
                }
            }
        }
    }
    reset() {
        this.valid = true;
        this.delay = 0;
        this.repeat = -1;
        this.interval = 0;
        this.target = null;
        this.callback = null;
        this._counter = 0;
        this._delayPassed = false;
    }
}
export class TimerPool {
    static Pool = [];
    static get() {
        const timer = this.Pool.pop();
        return timer ?? new Timer();
    }
    static recycle(timer) {
        timer.reset();
        this.Pool.push(timer);
    }
}
