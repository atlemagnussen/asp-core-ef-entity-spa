import { writable } from 'svelte/store';
import helper from "../services/helper.js";


const calcDiff = (e, n) => {
    let diff = e - n;
    return Math.floor(diff / 1000);
}

const createExpireClock = () => {
    let now = new Date();
    let expire = new Date();
    let val = { now, expire };
    const { subscribe, set } = writable(val);
    
    const refresh = () => {
        val.diff = calcDiff(val.expire, val.now);
        val.nowFormatted = helper.getYyyMmDdHhMmSs(val.now);
        val.expireFormatted = helper.getYyyMmDdHhMmSs(val.expire);
        return set(val);
    }
    
    const newSet = (newExpireUnixTimestamp) => {
        val.expire = new Date(newExpireUnixTimestamp*1000);
        return refresh();
    };
    
    setInterval(() => {
        val.now = new Date();
        refresh()
	}, 1000);
    return { subscribe, set: newSet };
};

export const expireClock = createExpireClock();