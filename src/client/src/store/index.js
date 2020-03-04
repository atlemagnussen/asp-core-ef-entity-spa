import { writable } from "svelte/store";

export const curRoute = writable("/");
export const userIsLoggedIn = writable(false);
export const userProfile = writable({ loggedIn: false, name: "Anon", initials: "U" });
export const curSearchParam = writable("");

export const plansStore = writable([]);