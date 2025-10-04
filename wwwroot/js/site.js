// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {

    // Reset submenu when navbar toggler is clicked
    $('.navbar-toggler').on('click', function () {
        subMenuReset();
    });

    // Define submenu items for each main menu category
    const subMenus = {
        AboutMeMenu: [
            { label: "Bio", url: "/Bio/Index" },
            { label: "Resume", url: "/Resume/Index" },
            { label: "Contact", url: "/Contact/Index" }
        ],
        PortfolioMenu: [
            { label: "AI Chat", url: "/AI/Index" },
            { label: "API Demo (FastF1)", url: "/F1/Index" }
        ]
    };

    const $mainMenu = $('#mainMenu');
    let submenuTimeout;
    let lastClickedMenu = null;

    // Handle main menu clicks
    $('.nav-link[data-submenu]').on('click', function (e) {
        const $this = $(this);

        const submenuKey = $this.data('submenu');
        if (!submenuKey || submenuKey === "none") return;

        const items = subMenus[submenuKey];
        if (!items) return;

        e.preventDefault();

        // Remove existing submenu items
        $mainMenu.find('.submenu-item').remove();

        // Toggle off if same item clicked again
        if (lastClickedMenu === submenuKey) {
            lastClickedMenu = null;
            $this.removeClass('active');
            return;
        }

        // Reset all main menu active states
        $('.nav-link[data-submenu]').removeClass('active');

        // Apply active state immediately
        $this.addClass('active');

        // Inject submenu items
        items.forEach(item => {
            $mainMenu.append(`
                <li class="nav-item submenu-item ms-3">
                    <a class="nav-link sub-link" href="${item.url}">${item.label}</a>
                </li>
            `);
        });

        lastClickedMenu = submenuKey;
    });

    // Handle submenu link clicks with AJAX content loading
    $(document).on('click', '.sub-link', function (e) {
        e.preventDefault();
        const url = $(this).attr('href');

        subMenuReset();

        // Collapse navbar on mobile
        $('.navbar-collapse').collapse('hide');

        // Show loading spinner
        $('#main-content').html(`
            <div class="d-flex justify-content-center align-items-center" style="height: 200px;">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        `);

        // Load content via AJAX
        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                /*history.pushState(null, '', url);*/
                $('#main-content').html(result);

                if (url === "/Resume/Index") {
                    loadPageStylesheet("/css/resume.css");
                } else if (url === "/F1/Index") {
                    loadPageStylesheet("/css/f1.css");
                    loadF1Section('NextRace');
                } else if (url === "/Contact/Index") {
                    loadPageStylesheet("/css/contact.css");
                    initializeContactForm();
                } else if (url === "/Bio/Index") {
                    loadPageStylesheet("/css/bio.css");
                }
            },
            error: function () {
                $('#main-content').html('<p class="text-danger">Failed to load content.</p>');
            }
        });
    });

    // Hide submenu when clicking outside
    $(document).on('click', function (e) {
        const clickedInsideMenu = $(e.target).closest('.nav-link[data-submenu], .submenu-item, .navbar-collapse').length > 0;

        if (!clickedInsideMenu) {
            subMenuReset();
        }
    });


    // Sub menu reset
    function subMenuReset() {
        $('#mainMenu').find('.submenu-item').remove();
        lastClickedMenu = null;
    }
});



// ******************************************



// Contact
function initializeContactForm() {
    const contactForm = document.querySelector("form[action='/Contact/SaveMessage']");

    if (contactForm) {
        contactForm.addEventListener("submit", function (event) {
            event.preventDefault();

            const formData = new FormData(this);
            const submitButton = this.querySelector("button[type='submit']");
            const alertContainer = document.querySelector("#alertContainer");

            // Lock the button
            submitButton.disabled = true;

            setTimeout(() => {
            fetch("/Contact/SaveMessage", {
                method: "POST",
                body: formData
            })
                .then(response => response.text())
                .then(html => {
                    alertContainer.innerHTML = html;

                    // Unlock the button
                    submitButton.disabled = false;

                    // Auto-dismiss after 4 seconds
                    setTimeout(() => {
                        const alert = alertContainer.querySelector(".alert");
                        if (alert) {
                            alert.classList.add("fade-out");
                            setTimeout(() => alert.remove(), 500);
                        }
                    }, 4000);
                })
                .catch(error => {
                    console.error("Error saving message:", error);
                    alertContainer.innerHTML =
                        "<div class='alert alert-danger mt-auto'>Unexpected error occurred.</div>";

                    // Unlock the button on error
                    submitButton.disabled = false;
                });
            }, 2000); // 2-second delay
        });
    }
}


// F1 - Race locations
function loadF1Section(section) {
    const contentDiv = document.getElementById('f1-content');

    contentDiv.innerHTML = `
    <div class="d-flex justify-content-center align-items-center" style="height: 200px;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>`;

    // Remove 'active' from all nav links
    document.querySelectorAll('.f1-nav-link').forEach(link => {
        link.classList.remove('active');
    });

    // Add 'active' to the clicked link
    const clickedLink = document.querySelector(`[onclick="loadF1Section('${section}')"]`);
    if (clickedLink) {
        clickedLink.classList.add('active');
    }


    fetch(`/F1/${section}`)
        .then(response => response.text())
        .then(html => {
            contentDiv.innerHTML = html;

            if (section === "NextRace") {
                initCountdown();
                initializeTimeToggle();
            }
        })
        .catch(error => {
            contentDiv.innerHTML = '<div class="alert alert-danger">Failed to load section.</div>';
            console.error(error);
        });
}

function initCountdown() {
    // Update countdown — always use UTC
    const countdown = document.querySelector(".f1-countdown");

    if (countdown) {
        const targetStr = countdown.dataset.utc;
        const raceDateTime = new Date(targetStr);

        startCountdown(raceDateTime);
    }

}

let countdownInterval = null;

function startCountdown(raceDateTime) {
    if (countdownInterval) {
        clearInterval(countdownInterval);
        countdownInterval = null;
    }

    const raceEndTime = new Date(raceDateTime.getTime() + 2 * 60 * 60 * 1000); // 2 hours

    function updateCountdown(now = new Date()) {
        const diff = raceDateTime - now;

        const elDays = document.getElementById("days");
        const elHours = document.getElementById("hours");
        const elMinutes = document.getElementById("minutes");
        const elSeconds = document.getElementById("seconds");
        const elCountdownTime = document.querySelector(".f1-countdownTime");

        if (!elDays || !elHours || !elMinutes || !elSeconds || !elCountdownTime) {
            clearInterval(countdownInterval);
            return;
        }

        if (diff <= 0 && now < raceEndTime) {
            elCountdownTime.innerHTML = `<span class="f1-count-label">Race is live!</span>`;
            clearInterval(countdownInterval);
            return;
        }

        if (now >= raceEndTime) {
            fetchNextRaceData();
            clearInterval(countdownInterval);
            return;
        }

        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
        const minutes = Math.floor((diff / (1000 * 60)) % 60);
        const seconds = Math.floor((diff / 1000) % 60);

        elDays.textContent = String(days).padStart(2, '0');
        elHours.textContent = String(hours).padStart(2, '0');
        elMinutes.textContent = String(minutes).padStart(2, '0');
        elSeconds.textContent = String(seconds).padStart(2, '0');
    }

    updateCountdown();
    countdownInterval = setInterval(() => updateCountdown(), 1000);
}


async function fetchNextRaceData() {
    try {
        const configElement = document.getElementById("config");
        const _baseUrl = configElement?.dataset.baseurl;
        const response = await fetch(`${_baseUrl}/raceinfo`);
        const data = await response.json();

        // Update countdown target
        if (data.next_race && data.race_day && data.race_time) {
            const countdownElement = document.getElementById("countdown");
            if (!countdownElement) return;

            const targetUtc = `${data.race_day}T${data.race_time}Z`;
            countdownElement.dataset.target = targetUtc;
            initCountdown(); // Reinitialize countdown with new race time
        }
    } catch (err) {
        console.error("Failed to fetch next race data:", err);
    }
}

function initializeTimeToggle() {
    const toggle = document.getElementById("toggleTimeMode");
    const wrapper = document.getElementById("toggleWrapper");

    if (!toggle || !wrapper) return;

    // Default: show track time
    wrapper.classList.remove("my-active");
    toggle.checked = false;

    function updateSessionDisplay(showLocal) {
        document.querySelectorAll(".f1-session-day").forEach(el => {
            const utc = el.dataset.utc;
            const track = el.dataset.track;

            const dateStr = showLocal ? utc : track;
            const date = new Date(dateStr);

            // Try localized format first
            const localized = date.toLocaleDateString(undefined, {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });

            // Detect if it's day-first (e.g., "22 sep. 2025")
            const dayFirst = /^\d{1,2}\D/.test(localized);

            if (dayFirst) {
                // Fallback to ISO-style
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                el.textContent = `${year}-${month}-${day}`;
            } else {
                el.textContent = localized;
            }
        });

        document.querySelectorAll(".f1-session-time").forEach(el => {
            const utc = el.dataset.utc;
            const track = el.dataset.track;

            const dateStr = showLocal ? utc : track;
            const date = new Date(dateStr);

            el.textContent = date.toLocaleTimeString(undefined, {
                hour: 'numeric',
                minute: '2-digit'
            });
        });

        // Update countdown — always use UTC
        const countdown = document.querySelector(".f1-countdown");
        if (countdown) {
            const raceDateTime = new Date(countdown.dataset.utc);
            startCountdown(raceDateTime);
        }
    }

    // Initial render (track time)
    updateSessionDisplay(false);

    // Toggle handler
    toggle.addEventListener("change", () => {
        const showLocal = toggle.checked;
        wrapper.classList.toggle("my-active", showLocal);
        updateSessionDisplay(showLocal);
    });

    // Make sides clickable
    const leftSide = document.querySelector(".toggle-side.left");
    const rightSide = document.querySelector(".toggle-side.right");

    if (leftSide && rightSide) {
        leftSide.addEventListener("click", () => {
            toggle.checked = false;
            toggle.dispatchEvent(new Event("change"));
        });

        rightSide.addEventListener("click", () => {
            toggle.checked = true;
            toggle.dispatchEvent(new Event("change"));
        });
    }
}

// ***************************************************

function loadPageStylesheet(href) {
    const existing = document.querySelector(`link[data-page-style="${href}"]`);
    if (!existing) {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.setAttribute("data-page-style", href);
        document.head.appendChild(link);
    }
}
