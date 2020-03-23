import Home from "./views/Home.svelte";
import About from "./views/About.svelte";
import SignIn from "./views/SignIn.svelte";
import RegisterUser from "./views/RegisterUser.svelte";
import RegisterCustomer from "./views/RegisterCustomer.svelte";
import Customers from "./views/Customers.svelte";
import Users from "./views/Users.svelte";
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
        path: "register-user",
        name: "RegisterUser",
        component: RegisterUser,
    },
    {
        path: "register-customer",
        name: "RegisterCustomer",
        component: RegisterCustomer
    },
    {
        path: "customers",
        name: "Customers",
        component: Customers
    },
    {
        path: "users",
        name: "Users",
        component: Users
    },
    {
        path: "account",
        name: "Konto",
        component: Account
    }
];
