<script>
    let email, pass;
    let regged = false;
    let message = "...";
    const regEmail = () => {
        regged = true;
        firebase.auth().createUserWithEmailAndPassword(email, pass)
        .then(() => {
            message = `Registration of ${email} complete, check your email to confirm`;
        })
        .catch((error) => {
            console.error(error);
            message =  `${error.message}(${error.code})`;
        });
    };
</script>

{#if regged}
    <p>{message}</p>
{:else}
    <h2>Registrer deg</h2>

    <form>
        <input bind:value="{email}" placeholder="Epost" />
        <input bind:value="{pass}" placeholder="Passord" />
        <button on:click|preventDefault="{regEmail}">Registrer</button>
    </form>
{/if}
