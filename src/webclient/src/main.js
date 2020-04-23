import App from "./App.svelte";

const app = new App({
    target: document.body,
    props: {
        name: "Test SPA"
    }
});

export default app;