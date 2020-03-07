<script>
    curSearchParam.set(window.location.search);
    import { curRoute, curSearchParam } from "./store";
    import up from "./services/userprofile.js";
    import Link from "./components/Link.svelte";
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
    nav {
        display: block;
        user-select: none;
    }
</style>

<svelte:window on:popstate="{handlerBackNavigation}" />
<main>
    <header>
        <nav>
            <Link page="{{ path: '/', name: 'Home' }}">
                <div class="logo">TestSPA</div>
            </Link>
            <Link page="{{ path: '/customers', name: 'Customers' }}" />
            <Link page="{{ path: '/about', name: 'About' }}" />
        </nav>
        <Login />
    </header>

    <Container />
</main>
