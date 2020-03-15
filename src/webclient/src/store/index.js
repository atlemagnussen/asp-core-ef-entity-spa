import { writable } from "svelte/store";

export const curRoute = writable("/");
export const userIsLoggedIn = writable(false);
export const curSearchParam = writable("");

export const plansStore = writable([]);

const createGettableWritableStore = (initialVal) => {
    let val = initialVal;
    const { subscribe, set, update } = writable(val);
    const get = () => val;
    var newSet = (newVal) => {
        val = newVal;
        return set(val);
    };
    return { subscribe, set: newSet, update, get };
};

export const userProfile = createGettableWritableStore({ loggedIn: false, name: "Anon", initials: "U" });