<script>
    import { onDestroy, onMount } from "svelte";
    import Link from "../components/Link.svelte";
    import auth from "../services/authentication.js";
    import { userIsLoggedIn, plansStore } from "../store";
    export let param;
    let plans = [];
    let newPlanName;

    const unsubscribe = plansStore.subscribe(value => {
        plans = value;
    });
    onMount(async () => {
        const currentUser = await auth.getUser();
        if (currentUser) {
            console.log(currentUser);
        } else {
            auth.login();
        }
    });
    onDestroy(unsubscribe);
    const addNew = async () => {
        await service.create(newPlanName);
        // plans = [...plans, { name: newPlanName }];
        // plansStore.set(plans);
    };
</script>
{#if userIsLoggedIn}
    {#if param}
        <p>{param}</p>
    {:else}
        <input type="text" placeholder="new task" bind:value="{newPlanName}" />
        <button on:click="{addNew}">Create</button>
        <ul>
            {#each plans as { name, _id }, i}
                <li><Link page="{{ path: '/plans/' + _id, name }}"></Link></li>
            {/each}
        </ul>
    {/if}
{:else}
    <p>Not logged in</p>
{/if}
