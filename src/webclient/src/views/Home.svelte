<script>
    import Link from "../components/Link.svelte";
    import { userProfile } from "../store";
</script>

<h1>Welcome to Test</h1>
{#if $userProfile.loggedIn}
    <h3>Logged in as {$userProfile.email}</h3>
    <p>
        <b>id_token:</b> {$userProfile.id_token}<br>
        <b>access_token:</b> {$userProfile.access_token}<br>
        <b>idp:</b> {$userProfile.idp}<br>
        <b>sid:</b> {$userProfile.sid}<br>
        <b>sub:</b> {$userProfile.sub}<br>
    </p>
{:else}
    <h3>Not logged in</h3>
{/if}
