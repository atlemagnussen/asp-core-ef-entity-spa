<script>
    import Link from "../components/Link.svelte";
    import { userProfile } from "../store";
    import restService from "../services/restService.js";

    let claims = [];

    let getCurrentClaims = async () => {
        claims = await restService.getWithAuth("https://localhost:5001/api/user/currentuserclaims");
    };

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
    <h3>All Claims</h3>
    <button on:click="{getCurrentClaims}">Get/Refresh</button>
    <ul>
        {#each claims as claim, i}
            <li>{claim.type}:{claim.value}</li>
        {/each}
    </ul>
{:else}
    <h3>Not logged in</h3>
{/if}
