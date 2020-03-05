import Home from "./views/Home.svelte";
import About from "./views/About.svelte";
import SignIn from "./views/SignIn.svelte";
import SignUp from "./views/SignUp.svelte";
import Customers from "./views/Customers.svelte";
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
        path: "customers",
        name: "Customers",
        component: Customers
    },
    {
        path: "account",
        name: "Konto",
        component: Account
    }
];
