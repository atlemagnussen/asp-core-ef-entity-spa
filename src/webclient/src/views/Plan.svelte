<script>
    import { userIsLoggedIn } from "../store";
    import PlanEdit from "./PlanEdit.svelte";
    import service from "../services/planService.js";
    export let id;
    let editMode = false;
    let plan = {
        "name": "empty",
        "_id": id
    };
    const toggleEdit = () => {
        editMode = !editMode;
    };

    const getPlan = async () => {
        try {
            plan = await service.get(id);
        }
        catch(e) {
            plan.name = "disallowed";
        }
    };
    getPlan();

</script>

{#if userIsLoggedIn}
    <button on:click="{toggleEdit}">Edit</button>
    {#if editMode}
        <PlanEdit id="{id}"></PlanEdit>
    {:else}
        <p>
            <label for="name">Name:</label>
            <span>{plan.name}</span><br>

            <label for="id">id:</label>
            <span>{plan._id}</span>
        </p>
    {/if}
{:else}
    <p>Not logged in</p>
{/if}
