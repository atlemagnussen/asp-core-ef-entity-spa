<script>
    import restService from "../services/restService.js";
    let email, password, name;
    let errorMsg = "";
    let reggedOk = false;
    let callRegisterUser = async () => {
        try {
            const res = await restService.postWithAuth("https://localhost:5001/api/user/register", {name, email, password});
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

    <p>User {name} ({email}) registered!</p>

{:else}
    <form>
        <input bind:value="{name}" placeholder="Name" />
        <input bind:value="{email}" placeholder="Epost" />
        <input bind:value="{password}" placeholder="Passord" />
        <button on:click|preventDefault="{callRegisterUser}">Registrer</button>
    </form>
{/if}


<div>{errorMsg}</div>