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
</style>

<div>
    {#if $userIsLoggedIn}
        <PopDown>
            <div slot="btnContent">
                <Circle>{$userProfile.initials}</Circle>
            </div>
            <div slot="dlgContent" class="flex column">
                <div>
                    {@html usernameOrEmail}
                </div>
                <Link page="{{ path: '/account', name: 'Konto' }}" />
                <button on:click="{logOut}">Log out</button>
            </div>
        </PopDown>
    {:else}
        <button on:click="{logIn}">Log in</button>
    {/if}

</div>
