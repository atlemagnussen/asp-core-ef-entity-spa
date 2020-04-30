import { writable } from 'svelte/store';
import helper from "../services/helper.js";

const calcDiff = (e, n) => {
    let diff = e - n;
    return Math.floor(diff / 1000);
}

const createExpireClock = () => {
    let now = new Date();
    let expire = new Date();
    let diff = calcDiff(expire, now)
    let val = { now, expire, diff };
    val.nowFormatted = helper.getYyyMmDdHhMmSs(val.now);
    val.expireFormatted = helper.getYyyMmDdHhMmSs(val.expire);
    const { subscribe, set } = writable(val);
    const get = () => val;
    var newSet = (newExpireUnixTimestamp) => {
        val.expire = new Date(newExpireUnixTimestamp*1000);
        val.diff = calcDiff(val.expire, val.now);
        val.expireFormatted = helper.getYyyMmDdHhMmSs(val.expire);
        return set(val);
    };
    setInterval(() => {
        val.now = new Date();
        val.nowFormatted = helper.getYyyMmDdHhMmSs(val.now);
        val.diff = calcDiff(val.expire, val.now);
        set(val);
	}, 1000);
    return { subscribe, set: newSet, get };
};

export const expireClock = createExpireClock();