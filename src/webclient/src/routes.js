import Home from "./views/Home.svelte";
import About from "./views/About.svelte";
import SignIn from "./views/SignIn.svelte";
import SignUp from "./views/SignUp.svelte";
import Plans from "./views/Plans.svelte";
import Account from "./views/Account.svelte";

export default [
    {
        path: "/",
        name: "Home",
        component: Home,
    },
    {
        path: "about",
        name: "About",
        component: About,
    },
    {
        path: "signin",
        name: "Logginn",
        component: SignIn,
    },
    {
        path: "signup",
        name: "Reg",
        component: SignUp,
    },
    {
        path: "plans",
        name: "Plans",
        component: Plans
    },
    {
        path: "account",
        name: "Konto",
        component: Account
    }
];
