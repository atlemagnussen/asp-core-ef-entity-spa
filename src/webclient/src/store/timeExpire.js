import { writable } from 'svelte/store';

const formatter = new Intl.DateTimeFormat('nb-NO', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
});

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
        val.nowFormatted = formatter.format(val.now);
        val.expireFormatted = formatter.format(val.expire);
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