// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".main-link").forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            let menuType = this.getAttribute("data-menu");

            // Fetch submenu content directly into `#dynamic-menu`
            fetch(`/SubMenu/LoadSubMenu?type=${menuType}`)
                .then(response => response.text())
                .then(html => {
                    document.getElementById("dynamic-menu").innerHTML = html;
                })
                .catch(error => console.error("Error loading submenu:", error));
        });
    });

    // Ensure submenu links dynamically update @RenderBody() without reload
    document.addEventListener("click", function (event) {
        if (event.target.matches(".sub-link")) {
            event.preventDefault();
            let pageUrl = event.target.getAttribute("href");

            fetch(pageUrl)
                .then(response => response.text())
                .then(html => {
                    document.querySelector("main").innerHTML = html;
                })
                .catch(error => console.error("Error loading content:", error));
        }
    });
});

function loadPage(url) {
    fetch(url)
        .then(response => response.text())
        .then(html => {
            const mainContent = document.querySelector("main");
            mainContent.innerHTML = html; // Inject view into @RenderBody()

            if (url.includes("/F1/Index")) {
                fetchRaceLocations();
            }

            if (url.includes("/Email/Index")) {
                initializeEmailForm();
            }

            // Execute inline scripts manually after the view is loaded
            const scripts = mainContent.querySelectorAll("script");
            scripts.forEach(script => {
                const newScript = document.createElement("script");
                newScript.textContent = script.innerText; // Execute script content
                document.body.appendChild(newScript); // Run the script
                script.remove(); // Clean up old script reference
            });
        })
        .catch(error => console.error("Error loading content:", error));
}

function loadIframe(url) {
    document.getElementById("contentFrame").src = url;
}

function initializeEmailForm() {
    const emailForm = document.querySelector("form[action='/Email/SendEmail']");
    if (emailForm) {
        emailForm.addEventListener("submit", function (event) {
            event.preventDefault(); // Prevent default page reload
            const formData = new FormData(this);

            fetch("/Email/SendEmail", {
                method: "POST",
                body: formData
            })
                .then(response => response.text())
                .then(message => {
                    alert("Email sent successfully!");
                })
                .catch(error => console.error("Error sending email:", error));
        });
    }
}

// F1 - Race locations
async function fetchRaceLocations() {
    const response = await fetch('/F1/GetRaceLocations');
    const races = await response.json();

    const raceList = document.getElementById("raceList");
    raceList.innerHTML = "";

    races.forEach(race => {
        const listItem = document.createElement("li");
        const link = document.createElement("span");
        link.innerText = `${race.countryName} - ${race.circuitShortName}`;
        link.style.cursor = "pointer";
        link.addEventListener("click", () => fetchRaceDetails(race.meetingKey));
        listItem.appendChild(link);
        raceList.appendChild(listItem);
    });
}

// F1 - Race details
async function fetchRaceDetails(meetingKey) {
    const response = await fetch(`/F1/GetRaceDetails?meetingKey=${meetingKey}`);
    const race = await response.json();

    if (!race) {
        document.getElementById("raceInfo").innerText = "No race details found.";
        return;
    }

    let details = `<h2>${race.meetingName}</h2>
                   <p><strong>Location:</strong> ${race.location}</p>
                   <p><strong>Date:</strong> ${race.dateStart}</p>
                   <p><strong>Meeting Key:</strong> ${race.meetingKey}</p>`;

    document.getElementById("raceInfo").innerHTML = details;
}

// F1 - Driver info
async function fetchDriverData() {
    const response = await fetch('/F1/GetDriver?driverNumber=1&sessionKey=9158');
    const data = await response.json();
    document.getElementById("driverInfo").innerText = JSON.stringify(data, null, 2);
}

// F1 - Positions
document.addEventListener("DOMContentLoaded", function () {
    if (document.getElementById("myButton")) {
        document.getElementById("myButton").addEventListener("click", function () {
            fetch('/F1/GetPositions?meetingKey=1254', { method: 'GET' })
                .then(response => response.json())
                .then(data => {
                    let tbody = document.getElementById("positionsTableBody");
                    tbody.innerHTML = "";
                    data.forEach(item => {
                        tbody.innerHTML += `<tr><td>${item.positionNr}</td><td>${item.driverNumber}</td><td>${item.sessionKey}</td></tr>`;
                    });
                    document.getElementById("positionsContainer").style.display = "block"; // Show table
                })
                .catch(error => console.error('Error:', error));
        });
    }
});