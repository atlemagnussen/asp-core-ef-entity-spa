<script>
    import { onDestroy } from "svelte";
    import Plan from "./Plan.svelte";
    import Link from "../components/Link.svelte";
    import service from "../services/planService.js";
    import { userIsLoggedIn, plansStore } from "../store";
    export let param;
    let plans = [];
    let newPlanName;

    const unsubscribe = plansStore.subscribe(value => {
        plans = value;
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
            <Plan id="{param}"></Plan>
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
