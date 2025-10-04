// F1 Section Loader and Countdown Logic
const f1Content = document.getElementById("f1-content");
if (f1Content && !f1Content.dataset.initialized) {
    initializeF1Page();
    f1Content.dataset.initialized = "true";
}

function initializeF1Page() {
    // Load default section
    loadF1Section("NextRace");
}

let countdownInterval = null;

async function loadF1Section(section) {
    const contentDiv = document.getElementById('f1-content');
    contentDiv.innerHTML = `
        <div class="d-flex justify-content-center align-items-center" style="height: 200px;">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`;

    document.querySelectorAll('.f1-nav-link').forEach(link => link.classList.remove('active'));
    const clickedLink = document.querySelector(`[onclick="loadF1Section('${section}')"]`);
    if (clickedLink) clickedLink.classList.add('active');

    try {
        const response = await fetch(`/F1/${section}`);
        const html = await response.text();
        contentDiv.innerHTML = html;

        if (section === "NextRace") {
            initCountdown();
            initializeTimeToggle();
        }
    } catch (error) {
        contentDiv.innerHTML = '<div class="alert alert-danger">Failed to load section.</div>';
        console.error(error);
    }
}

function initCountdown() {
    const countdown = document.querySelector(".f1-countdown");
    if (countdown) {
        const raceDateTime = new Date(countdown.dataset.utc);
        startCountdown(raceDateTime);
    }
}

function startCountdown(raceDateTime) {
    if (countdownInterval) clearInterval(countdownInterval);

    const raceEndTime = new Date(raceDateTime.getTime() + (2 * 60 + 15) * 60 * 1000);

    function updateCountdown(now = new Date()) {
        const elDays = document.getElementById("days");
        const elHours = document.getElementById("hours");
        const elMinutes = document.getElementById("minutes");
        const elSeconds = document.getElementById("seconds");
        const countdownPanel = document.getElementById("countdownPanel");
        const livePanel = document.getElementById("raceLivePanel");

        if (!elDays || !elHours || !elMinutes || !elSeconds || !countdownPanel || !livePanel) {
            clearInterval(countdownInterval);
            return;
        }

        if (now >= raceDateTime && now < raceEndTime) {
            countdownPanel.style.display = "none";
            livePanel.style.display = "block";
            livePanel.classList.add("show");
            clearInterval(countdownInterval);
            return;
        }

        if (now >= raceEndTime) {
            fetchNextRaceData();
            clearInterval(countdownInterval);
            livePanel.style.display = "none";
            countdownPanel.style.display = "block";
            livePanel.classList.remove("show");
            return;
        }

        const diff = raceDateTime - now;
        elDays.textContent = String(Math.floor(diff / (1000 * 60 * 60 * 24))).padStart(2, '0');
        elHours.textContent = String(Math.floor((diff / (1000 * 60 * 60)) % 24)).padStart(2, '0');
        elMinutes.textContent = String(Math.floor((diff / (1000 * 60)) % 60)).padStart(2, '0');
        elSeconds.textContent = String(Math.floor((diff / 1000) % 60)).padStart(2, '0');
    }

    updateCountdown();
    countdownInterval = setInterval(updateCountdown, 1000);
}

async function fetchNextRaceData() {
    try {
        const configElement = document.getElementById("config");
        const baseUrl = configElement?.dataset.baseurl;
        const response = await fetch(`${baseUrl}/nextrace`);
        const data = await response.json();

        const raceSession = data.sessions?.find(s => s.name?.toLowerCase() === "race");
        if (!raceSession || !raceSession.utc?.day || !raceSession.utc?.time) return;

        const targetIso = `${raceSession.utc.day}T${raceSession.utc.time}Z`;
        const countdownElement = document.getElementById("countdown");
        if (!countdownElement) return;

        countdownElement.dataset.utc = targetIso;
        initCountdown();
    } catch (err) {
        console.error("Failed to fetch next race data:", err);
    }
}

function formatDate(date, fallbackIso = false) {
    const localized = date.toLocaleDateString(undefined, {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });

    const dayFirst = /^\d{1,2}\D/.test(localized);
    if (fallbackIso && dayFirst) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    return localized;
}

function initializeTimeToggle() {
    const toggle = document.getElementById("toggleTimeMode");
    const wrapper = document.getElementById("toggleWrapper");
    if (!toggle || !wrapper) return;

    wrapper.classList.remove("my-active");
    toggle.checked = false;

    function updateSessionDisplay(showLocal) {
        document.querySelectorAll(".f1-session-day").forEach(el => {
            const dateStr = showLocal ? el.dataset.utc : el.dataset.track;
            el.textContent = formatDate(new Date(dateStr), true);
        });

        document.querySelectorAll(".f1-session-time").forEach(el => {
            const dateStr = showLocal ? el.dataset.utc : el.dataset.track;
            el.textContent = new Date(dateStr).toLocaleTimeString(undefined, {
                hour: 'numeric',
                minute: '2-digit'
            });
        });

        const countdown = document.querySelector(".f1-countdown");
        if (countdown) {
            const raceDateTime = new Date(countdown.dataset.utc);
            startCountdown(raceDateTime);
        }
    }

    updateSessionDisplay(false);

    toggle.addEventListener("change", () => {
        const showLocal = toggle.checked;
        wrapper.classList.toggle("my-active", showLocal);
        updateSessionDisplay(showLocal);
    });

    document.querySelector(".toggle-side.left")?.addEventListener("click", () => {
        toggle.checked = false;
        toggle.dispatchEvent(new Event("change"));
    });

    document.querySelector(".toggle-side.right")?.addEventListener("click", () => {
        toggle.checked = true;
        toggle.dispatchEvent(new Event("change"));
    });
}
