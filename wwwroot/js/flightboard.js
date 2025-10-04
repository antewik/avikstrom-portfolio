// Keep track of changes in search parameters
let lastSearchParams = "";

// Track the active SignalR group
let currentGroup = null;

// Initialize the form once when the script loads
initializeFlightBoardForm();

// Only initialize SignalR once
if (!window.connectionSignalR) {
    window.connectionSignalR = new signalR.HubConnectionBuilder()
        .withUrl("/flightboardhub")
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    window.connectionSignalR.on("ReceiveFlightUpdate", function (newRowsHtml) {
        console.log("Received update from SignalR");
        const tbody = document.querySelector("#flightBoardResults tbody");

        if (tbody) {
            tbody.innerHTML = newRowsHtml;
        }
    });


    window.connectionSignalR.start().then(() => {
        console.log("SignalR connected");
    }).catch(err => console.error(err.toString()));

    window.connectionSignalR.onreconnected(() => {
        const flightBoardForm = document.getElementById("flightBoardForm");
        if (!flightBoardForm) return;

        const iataCode = flightBoardForm.querySelector("select[name='iataCode']").value;
        const direction = flightBoardForm.querySelector("select[name='direction']").value;
        if (iataCode) {
            joinGroup(iataCode, direction);
        }
    });
}

// SignalR: Leave old group and join new one
function joinGroup(iataCode, direction) {
    const newGroup = `${iataCode}:${direction}`;

    if (currentGroup && currentGroup !== newGroup) {
        const [oldIata, oldDirection] = currentGroup.split(":");
        window.connectionSignalR.invoke("LeaveAirportGroup", oldIata, oldDirection)
            .then(() => console.log("Left group:", currentGroup))
            .catch(err => console.error("Group leave failed:", err));
    }

    window.connectionSignalR.invoke("JoinAirportGroup", iataCode, direction)
        .then(() => {
            console.log("Joined group:", newGroup);
            currentGroup = newGroup;
        })
        .catch(err => console.error("Group join failed:", err));
}

// Handle flight board form submission
function initializeFlightBoardForm() {
    const flightBoardForm = document.getElementById("flightBoardForm");
    if (!flightBoardForm) return;

    const submitButton = flightBoardForm.querySelector("button[type='submit']");

    flightBoardForm.addEventListener("submit", function (event) {
        event.preventDefault();

        const params = new URLSearchParams(new FormData(flightBoardForm)).toString();
        const iataCode = flightBoardForm.querySelector("select[name='iataCode']").value;
        const direction = flightBoardForm.querySelector("select[name='direction']").value;

        // Avoid unnecessary calls to API
        if (params === lastSearchParams) {
            console.log("Skipping API call—parameters unchanged.");
            return;
        }

        lastSearchParams = params;

        // Disable the button while loading
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.innerText = "Loading...";
        }

        fetch(`/FlightBoard/Index?${params}&partial=true`)
            .then(res => res.text())
            .then(html => {
                document.querySelector("#flightBoardContainer").innerHTML = html;

                // Rebind form after DOM replacement
                initializeFlightBoardForm();

                // Switch SignalR group immediately on new search
                joinGroup(iataCode, direction);
            })
            .finally(() => {
                // Re-enable the button after update
                const newSubmitButton = document.querySelector("#flightBoardForm button[type='submit']");
                if (newSubmitButton) {
                    newSubmitButton.disabled = false;
                    newSubmitButton.innerText = "Search";
                }
            });
    });
}

