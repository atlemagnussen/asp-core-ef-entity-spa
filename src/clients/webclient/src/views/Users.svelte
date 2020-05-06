<script>
    import { userProfile } from "../store";
    import restService from "../services/restService.js";
    import Link from "../components/Link.svelte";
    let getAllUsers = async () => {
        return await restService.getWithAuth("user");
    };
    let promise = getAllUsers();

    let ensureRoles = async () => {
        const res = await restService.getWithAuth("user/ensureroles");
        console.log(res);
    }
    let deleteUser = async (id) => {
        console.log(id);
        const res = await restService.deleteWithAuth(`user/${id}`);
        console.log(res);
    };
</script>

<h1>Users</h1>
<button on:click="{ensureRoles}">Ensure Roles</button>
<Link page="{{ path: '/register-user', name: 'Register User' }}" />
{#await promise}
	<p>...waiting</p>
{:then users}
    
    <ul>
	{#each users as user, i}
        <li>
            {#if $userProfile.sub === user.id}
                <span>*ME*</span>
            {/if}    
            {user.name} - {user.email}
            {#if user.isAdmin}
                <span>(admin)</span>
            {:else}
                <span on:click="{() => deleteUser(user.id)}">Delete</span>
            {/if}
        </li>
    {/each}
    </ul>

{:catch error}
	<p style="color: red">{error.message}</p>
{/await}