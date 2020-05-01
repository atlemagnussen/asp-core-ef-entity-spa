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
        windowResize();
        window.addEventListener('resize', windowResize);
    });
    const authServer = helper.getAuthServerUrl();
    $: smallDevice = windowWidth < 769;
</script>

<style>
    nav {
        user-select: none;
    }
    .main-menu-button {
        padding: 1rem;
    }
    .main-menu-button:hover {
        cursor: pointer;
    }
    .main-menu-button:hover svg g {
        fill: var(--secondary-color);
    }
    .flat {
        padding: 1rem;
    }
</style>

{#if smallDevice}
    <PopDown>
        <div slot="btnContent" class="main-menu-button">
            <svg viewBox="0 0 100 80" width="40" height="40">
                <g class="hamburger-icon" fill="white">
                    <rect width="100" height="15" rx="8"></rect>
                    <rect y="30" width="100" height="15" rx="8"></rect>
                    <rect y="60" width="100" height="15" rx="8"></rect>
                </g>
            </svg>
        </div>
        <nav slot="dlgContent" class="flex column">
            <Link page="{{ path: '/customers', name: 'Customers' }}" />
            <Link page="{{ path: '/users', name: 'Users' }}" />
            <a href="{authServer}">Auth</a>
        </nav>
    </PopDown>
{:else}
    <nav class="flat">
        <Link page="{{ path: '/customers', name: 'Customers' }}" />
        <Link page="{{ path: '/users', name: 'Users' }}" />
        <a href="{authServer}">Auth</a>
    </nav>
{/if}
