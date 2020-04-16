<script>
    import { onMount } from "svelte";
    import Link from "../components/Link.svelte";
    import { userProfile } from "../store";
    import restService from "../services/restService.js";

    let claims = [];

    let getCurrentClaims = async () => {
        claims = await restService.getWithAuth("user/currentuserclaims");
    };
    let logoFig, logoSvg, animateEl;
    let toggleAnim = false;
    let toggle = () => {
        if (toggleAnim)
            stop();
        else
            start();
    }
    let start = () => {
        //logoSvg.unpauseAnimations();
        //toggleAnim = true;
        // logoFig.classList.add("enabled");
    }
    let stop = () => {
        //toggleAnim = false;
        //logoSvg.pauseAnimations();
        //animateEl.beginElement();
        // logoFig.classList.remove("enabled");
    }
    onMount(() => {
        // logoSvg = document.querySelector("svg#digilean-logo-login");
        // logoFig = document.querySelector("figure.digilean-logo-login");
        // animateEl = document.querySelector("animateTransform");
        // setTimeout(() => {
        //     stop();
        //     logoFig.classList.remove("enabled");
        // }, 1000);
    });
</script>
<style>
    #digilean-logo {
        animation: spin 3s linear infinite;
        transform-origin: center;
        /* transform-box: fill-box; */
    }
    @keyframes spin {
        from { transform:rotate(0deg); }
        to { transform:rotate(360deg); }
    }
</style>
<h1>Welcome to Test</h1>
<figure class="digilean-logo loading">
    <svg xmlns="http://www.w3.org/2000/svg">
        <use xlink:href="#digilean-logo-def" id="digilean-logo" />
    </svg>
    <figcaption></figcaption>
</figure>

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
