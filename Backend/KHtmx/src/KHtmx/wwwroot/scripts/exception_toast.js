// TODO: Add toast for network errors (Server down, shoddy internet, etc.)
htmx.on("htmx:afterRequest", (e) => {
    if (e.detail.xhr.status >= 400) {
        createErrorToast(e);
    }
});

const dataRoles = {
    toastContainer: "toast-container",
    toast: "toast",
    statusCode: "Status Code",
    statusText: "Status Text",
    responseText: "Response Text",
    copyError: "Copy Error",
    dismissError: "Dismiss Error",
    toggleFullscreen: "Toggle Fullscreen"
};

const getDataRoleSelector = (role) => `[data-role='${role}']`;

const detailsOpenFlag = "data-details-was-open";

const toastContainerTemplate = `
    <div data-role="${dataRoles.toastContainer}" class="fixed bottom-0 right-0 p-4 flex flex-col gap-4"></div>
`;

const detailsTemplate = `
    <details data-role="${dataRoles.toast}" class="bg-invalid rounded-2xl px-4 py-2 cursor-pointer max-w-screen-lg">
        <summary class="flex flex-row gap-20 p-4 place-content-between place-items-center font-bold">
            <span>
                <span data-role="${dataRoles.statusCode}">{Status}</span>
                <span data-role="${dataRoles.statusText}">{StatusText}</span>
            </span>
            <div class="flex flex-row gap-8 place-content-center">
                <button data-role="${dataRoles.copyError}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Copy</button>
                <button data-role="${dataRoles.toggleFullscreen}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Fullscreen</button>
                <button data-role="${dataRoles.dismissError}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Dismiss</button>
            </div>
        </summary>
        <pre data-role="${dataRoles.responseText}" class="border-2 border-primary rounded-b bg-invalid p-4 text-primary cursor-auto overflow-y-auto max-h-96 font-code">{ResponseText}</pre>
    </details>
`;

const dialogTemplate = `
    <dialog data-role="${dataRoles.toast}" class="p-4 rounded-xl bg-invalid max-w-[80vw] max-h-[80vh]">
        <header class="flex flex-row gap-20 p-4 place-content-between place-items-center font-bold">
            <span>
                <span data-role="${dataRoles.statusCode}">{Status}</span>
                <span data-role="${dataRoles.statusText}">{StatusText}</span>
            </span>
            <div class="flex flex-row gap-8 place-content-center">
                <button data-role="${dataRoles.copyError}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Copy</button>
                <button data-role="${dataRoles.toggleFullscreen}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Minimize</button>
                <button data-role="${dataRoles.dismissError}" class="font-bold py-2 px-4 rounded bg-action hover:bg-action-active focus:bg-action-active">Dismiss</button>
            </div>
        </header>
        <main>
            <pre data-role="${dataRoles.responseText}" class="border-2 border-primary rounded-b bg-invalid p-4 text-primary cursor-auto overflow-y-auto max-h-96 font-code">{ResponseText}</pre>
        </main>
    </dialog>
`;

function createErrorToast(e) {
    const toastContainer = ensureToastContainerExists();

    const templateElement = document.createElement("template");
    templateElement.innerHTML = detailsTemplate
        .replace("{Status}", e.detail.xhr.status)
        .replace("{StatusText}", e.detail.xhr.statusText)
        .replace("{ResponseText}", e.detail.xhr.responseText);

    const detailsElement = templateElement.content.querySelector("details");
    toastContainer.appendChild(detailsElement);

    addClickListeners(
        toastContainer,
        detailsElement.querySelector(getDataRoleSelector(dataRoles.copyError)),
        detailsElement.querySelector(getDataRoleSelector(dataRoles.dismissError)),
        detailsElement.querySelector(getDataRoleSelector(dataRoles.toggleFullscreen)),
        detailsElement
    );
}

function addClickListeners(toastContainer, copyBtn, closeBtn, fullscreenBtn, toastElement) {
    copyBtn.addEventListener("click", async (e) => {
        const text = e.target.closest(getDataRoleSelector(dataRoles.toast)).querySelector("pre").innerHTML;
        await navigator.clipboard.writeText(text);
    });

    closeBtn.addEventListener("click", (e) => {
        e.target.closest(getDataRoleSelector(dataRoles.toast)).remove();
    });

    fullscreenBtn.addEventListener("click", (e) => {
        const wrapperElement = e.target.closest(getDataRoleSelector(dataRoles.toast));
        if (wrapperElement.tagName === "DETAILS") {
            convertToDialog(toastContainer, wrapperElement);
        } else {
            convertToDetails(toastContainer, wrapperElement);
        }
    });

    toastElement.addEventListener("click", () => {
        if (toastElement.tagName !== "DETAILS") return;

        // un-open all other details elements when this one is opened
        toastContainer.querySelectorAll("details").forEach((details) => {
            if (details !== toastElement) {
                details.removeAttribute("open");
            }
        });
    });
}

function convertToDialog(toastContainer, existingDetailsElement) {
    // Get values from existing details element
    const values = {
        Status: existingDetailsElement.querySelector(getDataRoleSelector(dataRoles.statusCode)).innerHTML,
        StatusText: existingDetailsElement.querySelector(getDataRoleSelector(dataRoles.statusText)).innerHTML,
        ResponseText: existingDetailsElement.querySelector(getDataRoleSelector(dataRoles.responseText)).innerHTML
    };

    const wasOpen = existingDetailsElement.hasAttribute("open");

    // Create new elements
    const templateElement = document.createElement("template");
    templateElement.innerHTML = dialogTemplate
        .replace("{Status}", values.Status)
        .replace("{StatusText}", values.StatusText)
        .replace("{ResponseText}", values.ResponseText);
    const newDialogElement = templateElement.content.querySelector("dialog");
    existingDetailsElement.replaceWith(newDialogElement);
    if (wasOpen) {
        newDialogElement.setAttribute(detailsOpenFlag, "");
    }

    // Add event listeners to new elements
    const newCopyButtonElement = newDialogElement.querySelector(getDataRoleSelector(dataRoles.copyError));
    const newCloseButtonElement = newDialogElement.querySelector(getDataRoleSelector(dataRoles.dismissError));
    const newFullscreenButtonElement = newDialogElement.querySelector(getDataRoleSelector(dataRoles.toggleFullscreen));

    addClickListeners(
        toastContainer,
        newCopyButtonElement,
        newCloseButtonElement,
        newFullscreenButtonElement,
        newDialogElement
    );

    // Open the dialog
    newDialogElement.showModal();
}

function convertToDetails(toastContainer, existingDialogElement) {
    // Get values from existing dialog element
    const values = {
        Status: existingDialogElement.querySelector(getDataRoleSelector(dataRoles.statusCode)).innerHTML,
        StatusText: existingDialogElement.querySelector(getDataRoleSelector(dataRoles.statusText)).innerHTML,
        ResponseText: existingDialogElement.querySelector(getDataRoleSelector(dataRoles.responseText)).innerHTML
    };

    // Create new elements
    const templateElement = document.createElement("template");
    templateElement.innerHTML = detailsTemplate
        .replace("{Status}", values.Status)
        .replace("{StatusText}", values.StatusText)
        .replace("{ResponseText}", values.ResponseText);
    const newDetailsElement = templateElement.content.querySelector("details");
    existingDialogElement.replaceWith(newDetailsElement);

    // Add event listeners to new elements
    const newCopyButtonElement = newDetailsElement.querySelector(getDataRoleSelector(dataRoles.copyError));
    const newCloseButtonElement = newDetailsElement.querySelector(getDataRoleSelector(dataRoles.dismissError));
    const newFullscreenButtonElement = newDetailsElement.querySelector(getDataRoleSelector(dataRoles.toggleFullscreen));

    // Open the details
    if (existingDialogElement.hasAttribute(detailsOpenFlag)) {
        newDetailsElement.setAttribute("open", "");
    }

    addClickListeners(
        toastContainer,
        newCopyButtonElement,
        newCloseButtonElement,
        newFullscreenButtonElement,
        newDetailsElement
    );
}

function ensureToastContainerExists() {
    const existingToastContainer = document.querySelector(getDataRoleSelector(dataRoles.toastContainer));
    if (existingToastContainer) {
        return existingToastContainer;
    }

    const templateElement = document.createElement("template");
    templateElement.innerHTML = toastContainerTemplate;
    const newToastContainer = templateElement.content.querySelector(getDataRoleSelector(dataRoles.toastContainer));
    document.body.appendChild(newToastContainer);

    // add mutation observer to delete the container if it's empty.
    const observer = new MutationObserver((mutations) => {
        if (mutations[0].target.children.length === 0) {
            mutations[0].target.remove();
        }
    });
    observer.observe(newToastContainer, {
        childList: true
    });

    return newToastContainer;
}
