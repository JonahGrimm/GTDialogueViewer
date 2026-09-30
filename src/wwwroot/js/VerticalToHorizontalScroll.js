// Automatically translates vertical mouse wheel scrolling into horizontal scrolling for the tabs container
document.addEventListener('wheel', function (event) {
    const tabsContainer = event.target.closest('.horizontal-scroll-zone');
    if (tabsContainer) {
        if (event.deltaY !== 0) {
        event.preventDefault();
        tabsContainer.scrollLeft += event.deltaY;
        }
    }
}, { passive: false });
