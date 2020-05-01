<script>
    import { onMount } from "svelte";
    import Circle from "./Circle.svelte";
    import PopDown from "./PopDown.svelte";
    import Link from "../components/Link.svelte";
    import helper from "../services/helper.js";
    let windowWidth = 1000;
    let windowResize = () => {
        windowWidth = window.innerWidth;
    }
    onMount(() => {
        // if (auth.hasLoggedInUser()) {
        //     userIsLoggedIn.set(true);
        // }
        window.addEventListener('resize', windowResize);
    });
    const authServer = helper.getAuthServerUrl();
    
</script>

<style>
    nav {
        display: block;
        user-select: none;
    }
</style>
<nav>
    <Link page="{{ path: '/customers', name: 'Customers' }}" />
    <Link page="{{ path: '/users', name: 'Users' }}" />
    <a href="{authServer}">Auth</a>
</nav>
<!-- <div>
    {#if $userIsLoggedIn}
        <PopDown>
            <div slot="btnContent">
                <Circle>{$userProfile.initials}</Circle>
            </div>
            <div slot="dlgContent" class="flex column">
                <Link page="{{ path: '/customers', name: 'Customers' }}" />
                <Link page="{{ path: '/users', name: 'Users' }}" />
                <a href="{authServer}">Auth</a>
            </div>
        </PopDown>
    {:else}
        <button on:click="{logIn}">Log in</button>
    {/if}

</div> -->
