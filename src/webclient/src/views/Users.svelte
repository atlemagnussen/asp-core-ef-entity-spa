<script>
    import restService from "../services/restService.js";
    import Link from "../components/Link.svelte";
    let getAllUsers = async () => {
        return await restService.getWithAuth("https://localhost:5001/api/user");
    };
    let promise = getAllUsers();

    let ensureRoles = async () => {
        const res = await restService.getWithAuth("https://localhost:5001/api/user/ensureroles");
        console.log(res);
    }
</script>

<h1>Users</h1>
<button on:click="{ensureRoles}">Ensure Roles</button>
<Link page="{{ path: '/register-user', name: 'Register User' }}" />
{#await promise}
	<p>...waiting</p>
{:then users}
    
    <ul>
	{#each users as user, i}
        <li>{user.name} - {user.email}</li>
    {/each}
    </ul>

{:catch error}
	<p style="color: red">{error.message}</p>
{/await}