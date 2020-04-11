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
        logoSvg.unpauseAnimations();
        toggleAnim = true;
        // logoFig.classList.add("enabled");
    }
    let stop = () => {
        toggleAnim = false;
        logoSvg.pauseAnimations();
        //animateEl.beginElement();
        // logoFig.classList.remove("enabled");
    }
    onMount(() => {
        logoSvg = document.querySelector("svg#digilean-logo-login");
        logoFig = document.querySelector("figure.digilean-logo-login");
        animateEl = document.querySelector("animateTransform");
        setTimeout(() => {
            stop();
            logoFig.classList.remove("enabled");
        }, 1000);
    });
</script>
<style>
    .digilean-logo-login.enabled .spinning-logo {
        animation: spin 1s ease reverse;
        transform-origin: center;
        /* transform-box: fill-box; */
    }
    @keyframes spin {
        from {
            transform:rotate(0deg);
        }
        to {
            transform:rotate(360deg);
        }
    }
</style>
<h1>Welcome to Test</h1>
<figure class="digilean-logo-login" on:click="{toggle}">
    <svg id="digilean-logo-login" viewBox="0 0 5820 5820">
        <g id="logo" class="spinning-logo">
            <path
                id="top-left"
                fill="var(--logo-top-left-color)"
                d="M 605,2813 C 358,2689 153,2586 151,2584 c -2,-2 2,-40 8,-86 63,-438 261,-909 533,-1265 410,-538 1023,-921 1665,-1043 202,-38 533,-65 533,-44 0,3 -105,212 -232,464 l -233,459 217,434 c 119,239 214,438 211,441 -4,3 -25,6 -48,6 -66,0 -224,44 -318,89 -202,97 -358,257 -472,486 -19,38 -35,47 -490,276 l -470,237 z"
                />
            <path
                id="top-right"
                fill="var(--logo-top-right-color)"
                d="m 5195,2678 -460,-230 -432,216 c -238,119 -435,216 -437,216 -2,0 -6,-33 -9,-72 -26,-314 -237,-618 -532,-763 l -89,-44 -232,-463 -232,-463 229,-457 c 208,-416 231,-458 253,-458 50,0 296,51 417,85 1040,302 1810,1184 1969,2255 23,153 38,410 24,409 -5,0 -216,-104 -469,-231 z"
                />
            <path
                id="bottom-left"
                fill="var(--logo-bottom-left-color)"
                d="M 2390,5635 C 1966,5554 1575,5381 1231,5122 1090,5016 858,4792 748,4656 395,4218 191,3712 140,3145 c -12,-130 -13,-215 -3,-215 4,0 211,102 460,228 l 452,227 436,-217 c 239,-120 439,-218 444,-218 4,0 11,30 14,68 29,326 266,646 575,776 l 56,24 235,463 235,464 -230,455 -230,455 -35,2 c -19,1 -90,-9 -159,-22 z"
                />
            <path
                id="bottom-right"
                fill="var(--logo-bottom-right-color)"
                d="m 3154,5212 234,-467 -214,-425 c -117,-234 -213,-430 -213,-435 -1,-6 37,-16 82,-23 308,-45 580,-244 721,-529 l 33,-66 459,-233 c 252,-129 468,-234 478,-234 15,0 899,440 914,455 13,13 -57,341 -105,490 -313,981 -1124,1699 -2133,1890 -110,21 -336,45 -423,45 h -67 z"
                />
            <animateTransform
                xlink:href="#logo"
                attributeName="transform"
                attributeType="XML"
                type="rotate"
                from="0 2910 2910"
                to="360 2910 2910"
                dur="3s"
                begin="1s"
                repeatCount="indefinite"
            />
        </g>
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
