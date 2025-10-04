// ==============================
// WikstromIT Site Scripts
// ==============================

const ajaxCache = {};

$(function () {

    ensureInitialPageStyle();

    // ==============================
    // Navbar Toggler Behavior
    // ==============================
    $('.component-navbar-toggler').on('click', function () {
        subMenuReset();
    });

    // ==============================
    // Submenu Definitions
    // ==============================
    const subMenus = {
        AboutMeMenu: [
            { label: "Bio", url: "/Bio/Index" },
            { label: "Resume", url: "/Resume/Index" },
            { label: "Contact", url: "/Contact/Index" }
        ],
        PortfolioMenu: [
            { label: "FlightBoard (signalR demo)", url: "/FlightBoard/Index" },
            { label: "Formula 1 (api demo)", url: "/F1/Index" }
        ]
    };

    const $mainMenu = $('#mainMenu');
    let lastClickedMenu = null;

    // ==============================
    // Main Menu Click Handling
    // ==============================
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

    // ==============================
    // Submenu Link AJAX Loading
    // ==============================
    $(document).on('click', '.sub-link', function (e) {
        e.preventDefault();
        const url = $(this).attr('href');
        subMenuReset();
        loadContent(url, true);
    });

    // ==============================
    // Dismiss Submenu on Outside Click
    // ==============================
    $(document).on('click', function (e) {
        const clickedInsideMenu = $(e.target).closest('.nav-link[data-submenu], .submenu-item, .navbar-collapse').length > 0;
        if (!clickedInsideMenu) {
            subMenuReset();
        }
    });

    // ==============================
    // Submenu Reset Function
    // ==============================
    function subMenuReset() {
        $('#mainMenu').find('.submenu-item').remove();
        lastClickedMenu = null;
    }
});

// ==============================
// Dynamic Asset Loaders
// ==============================

function loadPageStylesheet(href, callback) {
    const existing = document.querySelector(`link[data-page-style="${href}"]`);
    if (!existing) {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.setAttribute("data-page-style", href);

        if (typeof callback === "function") {
            link.onload = callback;
        }

        document.head.appendChild(link);
    } else if (typeof callback === "function") {
        callback();
    }
}

function loadPageScript(src, callback) {
    const existing = document.querySelector(`script[data-page-script="${src}"]`);
    if (!existing) {
        const script = document.createElement("script");
        script.src = src;
        script.setAttribute("data-page-script", src);
        script.async = true;

        if (typeof callback === "function") {
            script.onload = callback;
        }

        document.head.appendChild(script);
    } else if (typeof callback === "function") {
        callback();
    }
}

function getRequiredStyle($html) {
    const rootAttr = $html.filter("[data-require-style]").data("require-style");
    if (rootAttr) return rootAttr;

    const childAttr = $html.find("[data-require-style]").data("require-style");
    if (childAttr) return childAttr;

    return null;
}

// ==============================
// Unified Content Loader
// ==============================
function loadContent(url, pushState = true) {
    // Collapse navbar on mobile
    $('.navbar-collapse').collapse('hide');

    // Check cache first
    if (ajaxCache[url]) {
        $('#main-content').html(ajaxCache[url]);
        if (pushState) {
            window.history.pushState({ url: url }, "", url);
        }
        return;
    }

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
            const $html = $($.parseHTML(result));
            const cssHref = getRequiredStyle($html);

            function injectContent() {
                $('#main-content').html(result);

                if (pushState) {
                    window.history.pushState({ url: url }, "", url);
                }

                // Conditional asset loading
                if (url === "/F1/Index") {
                    loadPageScript("/js/f1.js");
                } else if (url === "/Contact/Index") {
                    loadPageScript("/js/contact.js");
                    initializeF1Page();
                } else if (url === "/FlightBoard/Index") {
                    loadPageScript(window.signalRUrl, () => {
                        loadPageScript("/js/flightboard.js");
                    });
                }

                // Cache the result
                ajaxCache[url] = result;
            }

            if (cssHref) {
                loadPageStylesheet(cssHref, injectContent);
            } else {
                injectContent();
            }
        },
        error: function () {
            $('#main-content').html('<p class="text-danger">Failed to load content.</p>');
        }
    });
}

// ==============================
// Handle Back/Forward navigation
// ==============================
window.addEventListener("popstate", function (event) {
    const state = event.state;
    if (state && state.url) {
        loadContent(state.url, false);
    }
});

// ==============================
// Ensure CSS on full page load
// ==============================
function ensureInitialPageStyle() {
    const styleDiv = document.querySelector("[data-require-style]");
    if (styleDiv) {
        const href = styleDiv.getAttribute("data-require-style");
        if (href && !document.querySelector(`link[data-page-style="${href}"]`)) {
            const link = document.createElement("link");
            link.rel = "stylesheet";
            link.href = href;
            link.setAttribute("data-page-style", href);
            document.head.appendChild(link);
        }
    }
}

