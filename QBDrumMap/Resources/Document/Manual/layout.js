const sidebar = document.getElementById('sidebar');
const toggleBtn = document.getElementById('toggleSidebar');

// CSS変数からサイドバー幅を取得
function getSidebarWidth() {
    return getComputedStyle(document.documentElement)
        .getPropertyValue('--sidebar-width')
        .trim();
}

toggleBtn.addEventListener('click', () => {
    sidebar.classList.toggle('closed');
    toggleBtn.style.left = sidebar.classList.contains('closed')
        ? '0'
        : getSidebarWidth();
});
