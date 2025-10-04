
const contactForm = document.getElementById("contactForm");
if (contactForm && !contactForm.dataset.initialized) {
    initializeContactForm(contactForm);
    contactForm.dataset.initialized = "true";
}

function initializeContactForm() {
    const contactForm = document.getElementById("contactForm");
    if (!contactForm) return;

    contactForm.addEventListener("submit", function (event) {
        event.preventDefault();

        const formData = new FormData(this);
        const submitButton = this.querySelector("button[type='submit']");
        const alertContainer = document.querySelector("#alertContainer");

        // Lock the button
        submitButton.disabled = true;

        fetch("/Contact/SaveMessage", {
            method: "POST",
            body: formData
        })
            .then(response => response.text())
            .then(html => {
                alertContainer.innerHTML = html;

                // Unlock the button
                submitButton.disabled = false;

                // Reset the form fields
                contactForm.reset();

                // Auto-dismiss after 10 seconds
                setTimeout(() => {
                    const alert = alertContainer.querySelector(".alert");
                    if (alert) {
                        alert.classList.add("fade-out");
                        setTimeout(() => alert.remove(), 500);
                    }
                }, 10000);
            })
            .catch(error => {
                console.error("Error saving message:", error);
                alertContainer.innerHTML =
                    "<div class='alert alert-danger mt-auto'>Unexpected error occurred.</div>";

                // Unlock the button on error
                submitButton.disabled = false;
            });
    });
}