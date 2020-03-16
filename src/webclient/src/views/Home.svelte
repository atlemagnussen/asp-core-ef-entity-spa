<script>
    import Link from "../components/Link.svelte";
    import { userProfile } from "../store";
</script>

<h1>Welcome to Test</h1>
{#if $userProfile.loggedIn}
    <h3>Logged in as {$userProfile.email}</h3>
    <p>
        {#each Object.keys($userProfile) as field, i}
            <b>{field}:</b> {$userProfile[field]}<br>
        {/each}
        <b>expires_at:</b> {new Date($userProfile.expires_at*1000)}<br>
    </p>
{:else}
    <h3>Not logged in</h3>
{/if}
