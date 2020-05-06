<script>
    import { onMount } from "svelte";
    import Link from "../components/Link.svelte";
    import { userProfile } from "../store";
    import { expireClock } from "../store/timeExpire.js";
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
    .accesstoken {
        display: inline-block;
        padding: 1rem;
        background-color: darkolivegreen;
    }
    .accesstoken h3 {
        margin: 0;
        margin-bottom: 0.5rem;
    }

    .accesstoken.expired {
        background-color: indianred;
    }
</style>
<h1>Welcome to Test</h1>

<div class:expired="{$userProfile.expired}" class="accesstoken">
    <h3>Access token</h3>
    <b>Time now:</b> {$expireClock.nowFormatted}<br>
    <b>Expires at:</b> {$expireClock.expireFormatted}<br>
    <b>Expire in:</b> {$expireClock.diff}s<br>
    <b>expired:</b> {$userProfile.expired}
</div>
{#if $userProfile.loggedIn}
    <h3>Logged in as {$userProfile.email}</h3>
    <p>
        {#each Object.keys($userProfile) as field, i}
            <b>{field}:</b> {$userProfile[field]}<br>
        {/each}
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
