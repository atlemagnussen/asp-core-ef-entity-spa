<script>
    import { curRoute } from "../store";
    export let page;

    function redirectTo(event) {
        curRoute.set(event.currentTarget.pathname);
        window.history.pushState({ path: page.path }, "", window.location.origin + page.path);
    }
</script>

<style>
    a {
        color: var(--secondary-text);
    }
</style>

<a href={page.path} on:click|preventDefault={redirectTo}>
    <slot>{page.name}</slot>
</a>
