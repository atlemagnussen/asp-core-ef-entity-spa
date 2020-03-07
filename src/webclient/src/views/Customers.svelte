<script>
    import { onDestroy, onMount } from "svelte";
    import Link from "../components/Link.svelte";
    import auth from "../services/authentication.js";
    import { userProfile } from "../store";
    let plans = [];
    let newPlanName;

    let promise = getCustomers();

	async function getCustomers() {
		const res = await fetch("https://localhost:5001/api/customers", {
            method: "GET",
            headers: {
                'Authorization': 'Bearer ' + $userProfile.access_token
            }
        });
        if (res.ok) {
            const customers = await res.json();
			return customers;
		} else {
            if (res.status === 401) {
                throw new Error("Forbidden");
            }
            var text = await res.text();
			throw new Error(text);
		}
	}
</script>
<p>This page tries to fetch customers from the web api, you should get "Forbidden" if not logged in and a list of customers if you log in</p>
{#await promise}
	<p>...waiting</p>
{:then customers}
    
    <ul>
	{#each customers as cus, i}
        <li>({cus.id}) {cus.lastName} - {cus.firstName}</li>
    {/each}
    </ul>

{:catch error}
	<p style="color: red">{error.message}</p>
{/await}
