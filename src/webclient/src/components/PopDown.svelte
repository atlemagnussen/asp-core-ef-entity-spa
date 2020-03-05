<script>
    let dialogEl;
    let open = false;
    let toggleDialog = () => {
        open = !open;
    };
    $: style = open
        ? `left: ${dialogEl.parentNode.offsetLeft - dialogEl.offsetWidth + dialogEl.parentNode.offsetWidth}px;
              top: ${dialogEl.parentNode.offsetTop + dialogEl.parentNode.offsetHeight}px`
        : "";
</script>

<style>
    div {
        display: inline-block;
        user-select: none;
    }
    .popdown {
        visibility: hidden;
        z-index: -2;
        position: fixed;
        background-color: var(--secondary-color);
        left: 2rem;
        color: white;
        text-align: center;
        border: none;
        padding: 1rem;
        border-radius: 6px;
        box-shadow: 0 0 40px rgba(0, 0, 0, 0.1), 0 0 10px rgba(0, 0, 0, 0.25);
        max-width: 90vw;
    }
    .popdown.open {
        z-index: 1;
        visibility: visible;
    }
</style>

<div id="popdown-wrapper">
    <div on:click="{toggleDialog}">
        <slot name="btnContent">Open dlg</slot>
    </div>
    <div class="popdown" id="popdown" class:open {style} bind:this="{dialogEl}" on:click="{toggleDialog}">
        <slot name="dlgContent">dialog content</slot>
    </div>
</div>
