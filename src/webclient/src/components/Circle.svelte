<script>
    let active = false;
    let toggleActive = () => {
        active = !active;
    };
</script>

<style>
    span {
        display: inline-block;
        height: 2.5rem;
        width: 2.5rem;
        line-height: 2.5rem;
        vertical-align: middle;
        border-radius: 50%;
        background: var(--main-color);
        color: var(--secondary-text);
        text-align: center;
        font-family: "Roboto Mono", monospace;
        cursor: pointer;
        border: 1px solid white;
    }
    span:hover,
    span.active {
        background: var(--secondary-color);
        border-color: var(--main-color);
    }
</style>

<span on:click="{toggleActive}" class:active>
    <slot>*</slot>
</span>
