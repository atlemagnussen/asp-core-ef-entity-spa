<script>
    import Link from "../components/Link.svelte";
    import { userProfile, plansStore } from "../store";
</script>

<h1>Welcome to Test</h1>
{#if $userProfile.loggedIn}
    <h3>Logged in as {$userProfile.name} ({$userProfile.email})</h3>
    <h4>Plans</h4>

    <ul>
        {#each $plansStore as { name, _id }, i}
            <li>{name}</li>
        {/each}
    </ul>
{:else}
    <h3>Not logged in</h3>
{/if}
