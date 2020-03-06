<script>
    let dialog;
    let toggleDialog = () => {
        dialog = document.getElementById("popdown");
        dialog.classList.toggle("open");
        if (dialog.open) dialog.close();
        else dialog.show();
    };
</script>

<style>
    div {
        display: inline-block;
    }
    dialog {
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

    dialog::backdrop {
        background: linear-gradient(45deg, rgba(0, 143, 104, 0.5), rgba(250, 224, 66, 0.5));
    }
</style>

<div on:click="{toggleDialog}">
    <slot name="btnContent">Open dlg</slot>
</div>
<dialog class="my-modal" id="popdown">
    <slot name="dlgContent">dialog content</slot>
</dialog>
