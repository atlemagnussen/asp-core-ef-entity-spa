<script>
    import { userIsLoggedIn, userProfile } from "../store";
    import auth from "../services/authentication.js";
    import helper from "../services/helper.js";
    import Link from "../components/Link.svelte";
    let email, pass, msg = "";
    const loginGoogle = () => {
        auth.loginGoogle().then(function(result) {
            console.log("logged in ok");
            // const token = result.credential.accessToken;
            // const user = result.user;
          }).catch(function(error) {
            msg = `${err.message} (${err.code})`;
            msg = `${msg}, email:${error.email}`;
            msg = `${msg}, cred:${error.credential}`;
          });
    };
    const loginEmail = () => {
        auth.loginEmailPass(email, pass)
        .then(res => {
            console.log(res);
            userIsLoggedIn.set(true);
            setUserProfile(res.user);
        })
        .catch(err => {
            console.error(err);
            msg = `${err.message} (${err.code})`;
        });
    };
    const setUserProfile = loggedInUser => {
        let user = {
            loggedIn: true,
            id: loggedInUser.uid,
            email: loggedInUser.email,
            initials: helper.getInitials(loggedInUser.email)
        };
        userProfile.set(user);
    };
</script>

{#if $userIsLoggedIn}
    <p>already logged in</p>
{:else}
    <div>
        <h1>Logg inn</h1>
        <p>
            <button on:click="{loginGoogle}">Logg inn med Google</button>
        </p>
        <form>
            <p>{msg}</p>
            <input bind:value="{email}" placeholder="Epost" />
            <input bind:value="{pass}" placeholder="Passord" />
            <button on:click|preventDefault="{loginEmail}">Logg inn</button>
        </form>
        <Link page="{{ path: '/signup', name: 'Ny bruker?' }}" />
    </div>
{/if}
