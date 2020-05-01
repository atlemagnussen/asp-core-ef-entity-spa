<script>
    export let name = "";
    curSearchParam.set(window.location.search);
    import { curRoute, curSearchParam } from "./store";
    import up from "./services/userprofile.js";
    import Link from "./components/Link.svelte";
    import MainMenu from "./components/MainMenu.svelte";
    import Login from "./components/LoginMenu.svelte";
    import Container from "./Container.svelte";
    import { onMount } from "svelte";
    
    onMount(() => {
        curRoute.set(window.location.pathname);
        if (!history.state) {
            window.history.replaceState({ path: window.location.pathname }, "", window.location.href);
        }
    });
    function handlerBackNavigation(event) {
        curRoute.set(event.state.path);
    }
</script>

<style>
    .logo {
        display: inline-block;
        height: 2.5rem;
        width: 4rem;
        line-height: 2.5rem;
        vertical-align: middle;
        text-align: center;
        display: inline-block;
        color: var(--main-color);
        background: var(--main-text);
    }
</style>

<svelte:window on:popstate="{handlerBackNavigation}" />
<main>
    <header>
        <Link page="{{ path: '/', name: 'Home' }}">
            <div class="logo">{name}</div>
        </Link>
        <MainMenu />
        <Login />
    </header>

    <Container />
</main>
