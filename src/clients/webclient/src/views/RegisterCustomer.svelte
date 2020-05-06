<script>
    import restService from "../services/restService.js";
    import Link from "../components/Link.svelte";
    let firstName, lastName;
    let errorMsg = "";
    let reggedOk = false;
    let callRegisterCustomer = async () => {
        try {
            const res = await restService.postWithAuth("customers", {firstName, lastName});
            console.log(res);
            reggedOk = true;
        } catch (ex) {
            if (ex.message) {
                errorMsg = ex.message;
            } else
                errorMsg = "Something boo";
        }
    };
</script>

<style>
    div {
        color: red;
    }
</style>

<h2>Register user</h2>

{#if reggedOk}

    <p>Customer {firstName} {lastName} registered!</p>

    <Link page="{{ path: '/customers', name: 'Customers' }}" />
{:else}
    <form>
        <input bind:value="{firstName}" placeholder="First name" />
        <input bind:value="{lastName}" placeholder="Last name" />
        <button on:click|preventDefault="{callRegisterCustomer}">Register</button>
    </form>
{/if}


<div>{errorMsg}</div>