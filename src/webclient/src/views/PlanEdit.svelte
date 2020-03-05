<script>
    import { userIsLoggedIn } from "../store";
    import service from "../services/planService.js";
    export let id;
    let plan = {
        "name": "empty",
        "_id": id
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

    const update = async () => {
        await service.update(plan);
    };

</script>
{#if userIsLoggedIn}
    <p>
        <label for="name">Name:</label>
        <input bind:value={plan.name}><br>

        <label for="id">id:</label>
        <span>{plan._id}</span>
    </p>
    <button on:click="{update}">Update</button>
{:else}
    <p>Not logged in</p>
{/if}
