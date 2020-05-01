<script>
    import { onMount, onDestroy } from "svelte";
    import Circle from "./Circle.svelte";
    import PopDown from "./PopDown.svelte";
    import { userIsLoggedIn, userProfile } from "../store";
    import Link from "../components/Link.svelte";
    import auth from "../services/authentication.js";

    onMount(() => {
        // if (auth.hasLoggedInUser()) {
        //     userIsLoggedIn.set(true);
        // }
    });
    const logIn = () => {
        auth.login();
    };
    const logOut = () => {
        auth.logout();
        userIsLoggedIn.set(false);
        userProfile.set({ loggedIn: false, name: "Anon" });
    };
    const silent = () => {
        auth.silentRefresh();
    };
    let usernameOrEmail = "user";
    const unsub = userProfile.subscribe(val => {
        usernameOrEmail = val.email;
    });
    onDestroy(unsub);
</script>

<style>
    div {
        color: white;
    }
    .login-menu-button {
        display: inline-block;
        padding: 1rem;
    }
</style>

<div>
    {#if $userIsLoggedIn}
        <PopDown>
            <div slot="btnContent" class="login-menu-button">
                <Circle>{$userProfile.initials}</Circle>
            </div>
            <div slot="dlgContent" class="flex column">
                <div>
                    {@html usernameOrEmail}
                </div>
                <Link page="{{ path: '/account', name: 'Account' }}" />
                <button on:click="{silent}">Silent renew</button>
                <button on:click="{logOut}">Log out</button>
            </div>
        </PopDown>
    {:else}
        <div class="login-menu-button">
            <button on:click="{logIn}">Log in</button>
        </div>
    {/if}

</div>
