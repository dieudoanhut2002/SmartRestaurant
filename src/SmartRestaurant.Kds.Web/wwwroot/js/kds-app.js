/**
 * SmartRestaurant - KDS Display Application Logic
 */
const kdsApp = (() => {
    let soundEnabled = true;
    let audioCtx = null;
    let remakeModalInstance = null;
    let modal86Instance = null;

    function init() {
        initClock();
        initModals();
        startTimerRefresher();

        const reasonSelect = document.getElementById('remake-reason-select');
        if (reasonSelect) {
            reasonSelect.addEventListener('change', function () {
                const customContainer = document.getElementById('remake-custom-reason-container');
                if (this.value === 'other') {
                    customContainer.classList.remove('d-none');
                } else {
                    customContainer.classList.add('d-none');
                }
            });
        }
    }

    function initClock() {
        const clockEl = document.getElementById('kds-clock');
        function update() {
            if (clockEl) {
                const now = new Date();
                clockEl.textContent = now.toLocaleTimeString('vi-VN', { hour12: false });
            }
        }
        update();
        setInterval(update, 1000);
    }

    function initModals() {
        const remakeEl = document.getElementById('remakeModal');
        if (remakeEl) remakeModalInstance = new bootstrap.Modal(remakeEl);

        const modal86El = document.getElementById('modal86');
        if (modal86El) modal86Instance = new bootstrap.Modal(modal86El);
    }

    // Web Audio API Synthesizer (2-tone chime)
    function playChime() {
        if (!soundEnabled) return;
        try {
            if (!audioCtx) {
                audioCtx = new (window.AudioContext || window.webkitAudioContext)();
            }
            if (audioCtx.state === 'suspended') {
                audioCtx.resume();
            }

            const now = audioCtx.currentTime;
            const osc = audioCtx.createOscillator();
            const gain = audioCtx.createGain();

            osc.type = 'sine';
            // First note (587.33Hz - D5)
            osc.frequency.setValueAtTime(587.33, now);
            // Second note (880Hz - A5) after 0.15s
            osc.frequency.setValueAtTime(880.00, now + 0.15);

            gain.gain.setValueAtTime(0.3, now);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.6);

            osc.connect(gain);
            gain.connect(audioCtx.destination);

            osc.start(now);
            osc.stop(now + 0.6);
        } catch (e) {
            console.warn('Audio chime could not play:', e);
        }
    }

    function toggleSound() {
        soundEnabled = !soundEnabled;
        const icon = document.getElementById('sound-icon');
        const text = document.getElementById('sound-text');
        if (soundEnabled) {
            icon.className = 'bi bi-volume-up-fill';
            text.textContent = 'Chuông: BẬT';
            playChime();
        } else {
            icon.className = 'bi bi-volume-mute-fill';
            text.textContent = 'Chuông: TẮT';
        }
    }

    function toggleFullScreen() {
        if (!document.fullscreenElement) {
            document.documentElement.requestFullscreen().catch(err => {
                console.warn(`Error attempting to enable fullscreen: ${err.message}`);
            });
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            }
        }
    }

    function startTimerRefresher() {
        setInterval(() => {
            const timerElements = document.querySelectorAll('.ticket-timer');
            const now = new Date();
            timerElements.forEach(el => {
                const createdStr = el.getAttribute('data-timer');
                if (!createdStr) return;
                const created = new Date(createdStr);
                const elapsedMinutes = Math.floor((now - created) / 60000);
                el.textContent = `${elapsedMinutes}'`;

                const card = el.closest('.kds-ticket-card');
                if (card) {
                    card.classList.remove('sla-green', 'sla-yellow', 'sla-red');
                    el.classList.remove('sla-green-text', 'sla-yellow-text', 'sla-red-text');

                    let sla = elapsedMinutes >= 20 ? 'sla-red' : elapsedMinutes >= 10 ? 'sla-yellow' : 'sla-green';
                    card.classList.add(sla);
                    el.classList.add(`${sla}-text`);
                }
            });
        }, 30000);
    }

    // Bump Bar Actions
    async function startPreparation(ticketId) {
        try {
            const staffId = '00000000-0000-0000-0000-000000000000'; // Default kitchen staff
            const res = await fetch('/api/kds/ticket-status', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    ticketId: ticketId,
                    newStatus: 2, // InPreparation
                    staffId: staffId
                })
            });
            const data = await res.json();
            if (data.success) {
                playChime();
                window.location.reload();
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể bắt đầu chế biến', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    async function completeTicket(ticketId) {
        try {
            const staffId = '00000000-0000-0000-0000-000000000000';
            const res = await fetch('/api/kds/ticket-status', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    ticketId: ticketId,
                    newStatus: 3, // Completed
                    staffId: staffId
                })
            });
            const data = await res.json();
            if (data.success) {
                playChime();
                const card = document.getElementById(`ticket-${ticketId}`);
                if (card) {
                    card.style.transition = 'opacity 0.3s, transform 0.3s';
                    card.style.opacity = '0';
                    card.style.transform = 'scale(0.9)';
                    setTimeout(() => card.remove(), 300);
                } else {
                    window.location.reload();
                }
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể hoàn thành phiếu', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    async function toggleItemCheck(itemId, currentStatus) {
        try {
            const staffId = '00000000-0000-0000-0000-000000000000';
            // Status 1 = Pending, 2 = Cooking, 3 = ReadyToServe, 4 = Served
            let newStatus = (currentStatus === 'ReadyToServe' || currentStatus == 3) ? 2 : 3;

            const res = await fetch('/api/kds/item-status', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    orderItemId: itemId,
                    newStatus: newStatus,
                    staffId: staffId
                })
            });
            const data = await res.json();
            if (data.success) {
                playChime();
                window.location.reload();
            }
        } catch (e) {
            console.error('Error toggling item check:', e);
        }
    }

    async function markItemServed(itemId) {
        try {
            const staffId = '00000000-0000-0000-0000-000000000000';
            const res = await fetch('/api/kds/item-status', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    orderItemId: itemId,
                    newStatus: 4, // Served
                    staffId: staffId
                })
            });
            const data = await res.json();
            if (data.success) {
                playChime();
                window.location.reload();
            }
        } catch (e) {
            Swal.fire('Lỗi', e.message, 'error');
        }
    }

    // Remake Modal per OD-09
    function openRemakeModal(itemId, itemName) {
        document.getElementById('remake-item-id').value = itemId;
        document.getElementById('remake-item-name').textContent = itemName;
        document.getElementById('remake-reason-select').value = 'Cháy / Chín quá / Sống';
        document.getElementById('remake-custom-reason-container').classList.add('d-none');
        document.getElementById('remake-custom-reason').value = '';
        if (remakeModalInstance) remakeModalInstance.show();
    }

    async function confirmRemake() {
        const itemId = document.getElementById('remake-item-id').value;
        const select = document.getElementById('remake-reason-select');
        let reason = select.value;
        if (reason === 'other') {
            reason = document.getElementById('remake-custom-reason').value.trim();
        }
        if (!reason) {
            Swal.fire('Cảnh báo', 'Vui lòng cung cấp lý do làm lại món!', 'warning');
            return;
        }

        try {
            const res = await fetch('/api/kds/remake', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    orderItemId: itemId,
                    reason: reason,
                    managerId: '00000000-0000-0000-0000-000000000000'
                })
            });
            const data = await res.json();
            if (data.success) {
                if (remakeModalInstance) remakeModalInstance.hide();
                playChime();
                Swal.fire({
                    icon: 'success',
                    title: 'Đã tạo phiếu làm lại ưu tiên!',
                    text: 'Món lỗi đã được hủy 0đ cho khách và ghi nhận vào sổ hao hụt bếp.',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => window.location.reload());
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể tạo phiếu làm lại', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    // 86 Modal per BR-KIT-006
    async function open86Modal() {
        if (modal86Instance) modal86Instance.show();
        await load86Catalog();
    }

    let catalogCache = [];

    async function load86Catalog() {
        const tbody = document.getElementById('table-86-body');
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Đang tải danh mục món...</td></tr>';
        try {
            // Fetch catalog from POS catalog API or direct
            const res = await fetch('/api/kds/tickets'); // fallback ping
            // In smart restaurant, fetch from db menu items
            const catalogRes = await fetch('http://localhost:58329/api/pos/menu-catalog').catch(() => null);
            let items = [];
            if (catalogRes && catalogRes.ok) {
                const cats = await catalogRes.json();
                cats.forEach(c => c.items.forEach(it => items.push(it)));
            } else {
                // If POS host is separate, fetch from current tickets items
                const tRes = await fetch('/api/kds/tickets');
                const tickets = await tRes.json();
                const set = new Map();
                tickets.forEach(t => t.items.forEach(it => {
                    if (!set.has(it.menuItemId)) {
                        set.set(it.menuItemId, { id: it.menuItemId, code: 'MON', name: it.itemName, isAvailable: true });
                    }
                }));
                items = Array.from(set.values());
            }

            catalogCache = items;
            render86Table(items);
        } catch (e) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Lỗi tải danh mục: ${e.message}</td></tr>`;
        }
    }

    function render86Table(items) {
        const tbody = document.getElementById('table-86-body');
        if (!items || items.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Không tìm thấy món ăn nào</td></tr>';
            return;
        }

        tbody.innerHTML = items.map(it => {
            const isAvail = it.isAvailable !== false;
            return `
                <tr>
                    <td class="font-monospace text-secondary">${it.code || 'MON'}</td>
                    <td class="fw-bold text-light">${it.name}</td>
                    <td><span class="badge bg-secondary">Bếp</span></td>
                    <td>
                        <span class="badge ${isAvail ? 'bg-success' : 'bg-danger'}">
                            ${isAvail ? 'Còn Hàng' : 'HẾT MÓN (86)'}
                        </span>
                    </td>
                    <td class="text-end">
                        <button class="btn btn-sm ${isAvail ? 'btn-outline-danger' : 'btn-outline-success'}"
                                onclick="kdsApp.toggle86Status('${it.id}', ${!isAvail})">
                            <i class="bi ${isAvail ? 'bi-slash-circle' : 'bi-check2'} me-1"></i>
                            ${isAvail ? 'Báo Hết' : 'Mở Lại'}
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    function filter86Items() {
        const query = (document.getElementById('search-86-input')?.value || '').toLowerCase();
        const filtered = catalogCache.filter(it => it.name.toLowerCase().includes(query) || (it.code && it.code.toLowerCase().includes(query)));
        render86Table(filtered);
    }

    async function toggle86Status(menuItemId, newAvailableState) {
        try {
            const res = await fetch('/api/kds/toggle-86', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    menuItemId: menuItemId,
                    isAvailable: newAvailableState,
                    staffId: '00000000-0000-0000-0000-000000000000'
                })
            });
            const data = await res.json();
            if (data.success) {
                playChime();
                // Update local cache
                const item = catalogCache.find(it => it.id === menuItemId);
                if (item) item.isAvailable = newAvailableState;
                render86Table(catalogCache);
                Swal.fire({
                    icon: 'success',
                    title: newAvailableState ? 'Đã mở bán lại món!' : 'Đã báo hết món (86)!',
                    timer: 1500,
                    showConfirmButton: false
                });
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể đổi trạng thái', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi', e.message, 'error');
        }
    }

    return {
        init,
        playChime,
        toggleSound,
        toggleFullScreen,
        startPreparation,
        completeTicket,
        toggleItemCheck,
        markItemServed,
        openRemakeModal,
        confirmRemake,
        open86Modal,
        filter86Items,
        toggle86Status
    };
})();

document.addEventListener('DOMContentLoaded', () => {
    kdsApp.init();
});
