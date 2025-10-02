class HtmxDialog {
    static #bootstrapModal;
    static #dialogElement;

    static initialize(modalElementId, dialogElementId) {
        console.log("Initializer invoked");
        if (!modalElementId || !document.getElementById(modalElementId))
            throw new Error("Modal Element Id must be a valid id");

        if (!dialogElementId || !document.getElementById(dialogElementId))
            throw new Error("Dialog Element Id must be a valid id");

        HtmxDialog.#bootstrapModal = new bootstrap.Modal(document.getElementById(modalElementId));
        HtmxDialog.#dialogElement = document.getElementById(dialogElementId);

        return true;
    }

    static showModal(evt) {
        if (!evt || !HtmxDialog.#bootstrapModal || !HtmxDialog.#dialogElement)
            return;

        // Response targeting #dialog => show the modal
        if (evt.detail.target.id == HtmxDialog.#dialogElement.id) {
            HtmxDialog.#bootstrapModal.show();
            //console.log("Showing Modal");
        }
    }

    static hideModal(evt) {
        if (!evt || !HtmxDialog.#bootstrapModal || !HtmxDialog.#dialogElement)
            return;

        // Empty response targeting #dialog => hide the modal
        if (evt.detail.target.id == HtmxDialog.#dialogElement.id && !evt.detail.xhr.response) {
            HtmxDialog.#bootstrapModal.hide();
            evt.detail.shouldSwap = false;
            //console.log("Hiding Modal");
        }
    }

    static clearDialog(evt) {
        if (!HtmxDialog.#dialogElement)
            return;

        // Remove dialog content after hiding
        HtmxDialog.#dialogElement.innerHTML = "";
        //console.log("Clearing Dialog");
    }
}
