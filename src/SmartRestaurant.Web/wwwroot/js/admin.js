// SmartRestaurant Admin Script
document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.preventDefault();
            const sidebar = document.getElementById('sidebar-wrapper');
            if (sidebar) {
                sidebar.classList.toggle('toggled');
            }
        });
    }
});
