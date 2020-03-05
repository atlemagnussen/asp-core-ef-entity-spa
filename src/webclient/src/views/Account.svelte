<script>
    import { onMount, onDestroy } from "svelte";
    import { userProfile } from "../store";
    import userprofileService from "../services/userprofile.js";
    let userId, email, name, initials;
    const setFormVariables = (up) => {
        userId = up.id;
        email = up.email;
        initials = up.initials;
        if ($userProfile.name)
            name = $userProfile.name;
    };
    onMount(() => {
        if ($userProfile.loggedIn) {
            setFormVariables($userProfile);
        }
    });
    const unsub = userProfile.subscribe(up => {
        setFormVariables(up);
    });
    const updateUserProfile = async () => {
        try {
            userprofileService.updateCurrentUser(name);
            userProfile.set({
                loggedIn: true,
                id: userId,
                initials,
                email,
                name,
            })
        } catch (err) {
            console.error(err);
        }
    }
</script>
<style>

</style>
<h1>Konto</h1>
{#if $userProfile.loggedIn}
    <h3>Userprofile for {$userProfile.email}</h3>
    <form>
        <label>Name</label>
        <input bind:value="{name}" placeholder="Navn" />
        <br />
        <button on:click|preventDefault="{updateUserProfile}">Lagre</button>
    </form>
{:else}
    <p>Not logged in</p>
{/if}
