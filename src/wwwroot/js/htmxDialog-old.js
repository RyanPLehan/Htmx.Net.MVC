class HtmxDialog {
    #bootstrapModal;
    #dialogElement;

    constructor(modalElementId, dialogElementId) {
        console.log("Constructor invoked");
        if (!modalElementId || !document.getElementById(modalElementId))
            throw new Error("Modal Element Id must be a valid id");

        if (!dialogElementId || !document.getElementById(dialogElementId))
            throw new Error("Dialog Element Id must be a valid id");

        this.#bootstrapModal = new bootstrap.Modal(document.getElementById(modalElementId));
        this.#dialogElement = document.getElementById(dialogElementId);
        //this.#setEventHandlers();
    }

    #cleanup(element) {
        htmx.off(element, "htmx:afterSwap", this.showModalHandler);
        htmx.off(element, "htmx:beforeSwap", this.hideModalHandler);
        htmx.off(element, "hidden.bs.modal", this.clearDialogHandler);
    }

    #setEventHandlers() {
        htmx.on("htmx:afterSwap", this.showModalHandler);
        htmx.on("htmx:beforeSwap", this.hideModalHandler);
        htmx.on("hidden.bs.modal", this.clearDialogHandler);
    }

    showModalHandler(evt) {
        // Response targeting #dialog => show the modal
        if (evt.detail.target.id == this.#dialogElement.id) {
            this.#bootstrapModal.show();
            console.log("Showing Modal");
        }
    }

    hideModalHandler(evt) {
        // Empty response targeting #dialog => hide the modal
        if (evt.detail.target.id == this.#dialogElement.id && !evt.detail.xhr.response) {
            this.#bootstrapModal.hide();
            evt.detail.shouldSwap = false;
            console.log("Hiding Modal");
        }
    }

    clearDialogHandler(evt) {
        // Remove dialog content after hiding
        this.#dialogElement.innerHTML = "";
        console.log("Clearing Dialog");
    }
}


/* Original code that, if gets loaded more than once, each event is fired the same number of times it is loaded

; (function () {
    const modal = new bootstrap.Modal(document.getElementById("modal"))
    //htmx.logAll();

    htmx.on("htmx:afterSwap", (e) => {
        // Response targeting #dialog => show the modal
        if (e.detail.target.id == "dialog") {
            modal.show();
            console.log("htmx:afterSwap: ");
        }
    })

    htmx.on("htmx:beforeSwap", (e) => {
        // Empty response targeting #dialog => hide the modal
        if (e.detail.target.id == "dialog" && !e.detail.xhr.response) {
            modal.hide();
            e.detail.shouldSwap = false;
            console.log("htmx:beforewSwap: ");
        }
    })

    // Remove dialog content after hiding
    htmx.on("hidden.bs.modal", () => {
        document.getElementById("dialog").innerHTML = "";
    })
})()

*/