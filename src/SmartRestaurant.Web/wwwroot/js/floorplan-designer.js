// SmartRestaurant — Konva.js Floor Plan 2D Designer
function initFloorPlan() {
    const container = document.getElementById('floorplan-container');
    if (!container) return;
    if (typeof Konva === 'undefined') {
        console.error('Konva library is not loaded');
        return;
    }
    if (typeof tableData === 'undefined' || !Array.isArray(tableData)) {
        console.warn('tableData is not defined or not an array');
        return;
    }

    // Clear any previous Konva stage if re-initializing
    container.innerHTML = '';

    const width = container.clientWidth || container.offsetWidth || 850;
    const height = container.clientHeight || container.offsetHeight || 600;

    // Initialize Konva Stage
    const stage = new Konva.Stage({
        container: 'floorplan-container',
        width: width,
        height: height
    });

    const layer = new Konva.Layer();
    stage.add(layer);

    let selectedTable = null;

    // Status Colors mapping
    const statusColors = {
        0: '#198754', // Available (Green)
        1: '#ffc107', // Occupied (Yellow)
        2: '#0d6efd', // Reserved (Blue)
        3: '#dc3545', // CleaningNeeded (Red)
        4: '#6f42c1', // BillingRequested (Purple)
        5: '#6c757d'  // OutOfService (Gray)
    };

    const statusNames = {
        0: 'Trống (Available)',
        1: 'Có Khách (Occupied)',
        2: 'Đã Đặt (Reserved)',
        3: 'Cần Dọn (Cleaning)',
        4: 'Tạm Tính (Billing)',
        5: 'Ngừng Phục Vụ'
    };

    // Render Tables supporting both camelCase and PascalCase
    tableData.forEach(function (t) {
        const id = t.id || t.Id;
        const code = t.code || t.Code || 'Bàn';
        const name = t.name || t.Name || code;
        const capacity = t.capacity ?? t.Capacity ?? 4;
        const status = t.status ?? t.Status ?? 0;
        const shape = t.shape ?? t.Shape ?? 0;
        const posX = t.positionX ?? t.PositionX ?? 50;
        const posY = t.positionY ?? t.PositionY ?? 50;
        const w = (t.width ?? t.Width ?? 90) > 0 ? (t.width ?? t.Width ?? 90) : 90;
        const h = (t.height ?? t.Height ?? 80) > 0 ? (t.height ?? t.Height ?? 80) : 80;
        const rotation = t.rotation ?? t.Rotation ?? 0;
        const sessionCode = t.currentSessionCode || t.CurrentSessionCode;
        const sessionTotal = t.currentSessionTotal ?? t.CurrentSessionTotal;

        const group = new Konva.Group({
            x: posX,
            y: posY,
            rotation: rotation,
            draggable: true,
            id: id
        });

        const color = statusColors[status] || '#198754';

        let shapeNode;
        if (shape === 1) { // Round
            const radius = Math.min(w, h) / 2;
            shapeNode = new Konva.Circle({
                x: radius,
                y: radius,
                radius: radius,
                fill: '#ffffff',
                stroke: color,
                strokeWidth: 4,
                shadowColor: 'black',
                shadowBlur: 6,
                shadowOffset: { x: 2, y: 2 },
                shadowOpacity: 0.15
            });
        } else { // Rectangle or Square
            shapeNode = new Konva.Rect({
                x: 0,
                y: 0,
                width: w,
                height: h,
                cornerRadius: shape === 2 ? 8 : 12,
                fill: '#ffffff',
                stroke: color,
                strokeWidth: 4,
                shadowColor: 'black',
                shadowBlur: 6,
                shadowOffset: { x: 2, y: 2 },
                shadowOpacity: 0.15
            });
        }

        // Table Code Text
        const codeText = new Konva.Text({
            x: 0,
            y: h / 2 - 16,
            width: w,
            text: code,
            fontSize: 16,
            fontFamily: 'sans-serif',
            fontStyle: 'bold',
            fill: '#212529',
            align: 'center'
        });

        // Capacity Text
        const capText = new Konva.Text({
            x: 0,
            y: h / 2 + 6,
            width: w,
            text: `${capacity} chỗ`,
            fontSize: 11,
            fontFamily: 'sans-serif',
            fill: '#6c757d',
            align: 'center'
        });

        group.add(shapeNode);
        group.add(codeText);
        group.add(capText);

        const tableItem = {
            id: id,
            code: code,
            name: name,
            capacity: capacity,
            status: status,
            shape: shape,
            positionX: posX,
            positionY: posY,
            width: w,
            height: h,
            rotation: rotation,
            currentSessionCode: sessionCode,
            currentSessionTotal: sessionTotal
        };

        // Click / Touch interaction
        group.on('click tap', function () {
            selectTable(tableItem);
        });

        // Drag end: save coordinates to server via AJAX
        group.on('dragend', function () {
            saveTablePosition(id, group.x(), group.y(), w, h, group.rotation());
        });

        // Hover cursor
        group.on('mouseenter', function () {
            stage.container().style.cursor = 'move';
        });
        group.on('mouseleave', function () {
            stage.container().style.cursor = 'default';
        });

        layer.add(group);
    });

    layer.draw();

    function selectTable(table) {
        selectedTable = table;
        const noSel = document.getElementById('no-table-selected');
        const details = document.getElementById('table-details');
        if (noSel) noSel.style.display = 'none';
        if (details) details.style.display = 'block';

        const codeEl = document.getElementById('inspect-code');
        if (codeEl) codeEl.textContent = table.code;

        const nameEl = document.getElementById('inspect-name');
        if (nameEl) nameEl.textContent = table.name;

        const capEl = document.getElementById('inspect-capacity');
        if (capEl) capEl.textContent = `${table.capacity} khách`;

        const shapeEl = document.getElementById('inspect-shape');
        if (shapeEl) shapeEl.textContent = table.shape === 1 ? 'Bàn Tròn' : (table.shape === 2 ? 'Bàn Vuông' : 'Bàn Chữ Nhật');

        const sessEl = document.getElementById('inspect-session');
        if (sessEl) sessEl.textContent = table.currentSessionCode || 'Chưa mở phiên';

        const totEl = document.getElementById('inspect-total');
        if (totEl) totEl.textContent = table.currentSessionTotal ? table.currentSessionTotal.toLocaleString('vi-VN') + ' ₫' : '0 ₫';

        const badge = document.getElementById('inspect-status-badge');
        if (badge) {
            badge.textContent = statusNames[table.status] || 'Trống';
            badge.style.backgroundColor = statusColors[table.status] || '#198754';
            badge.className = 'badge px-3 py-2 fs-6 text-white';
        }

        const delId = document.getElementById('delete-table-id');
        if (delId) delId.value = table.id;
    }

    async function saveTablePosition(tableId, x, y, width, height, rotation) {
        if (!tableId) return;
        try {
            await fetch('/FloorPlan/UpdatePosition', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    tableId: tableId,
                    positionX: Math.round(x),
                    positionY: Math.round(y),
                    width: width,
                    height: height,
                    rotation: rotation
                })
            });
        } catch (e) {
            console.error('Failed to save position:', e);
        }
    }

    window.quickChangeStatus = async function (newStatus) {
        if (!selectedTable || !selectedTable.id) return;
        try {
            const response = await fetch('/FloorPlan/UpdateStatus', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    tableId: selectedTable.id,
                    status: newStatus
                })
            });
            const result = await response.json();
            if (result.success) {
                location.reload();
            }
        } catch (e) {
            console.error(e);
        }
    };

    window.openEditTableModal = function () {
        if (!selectedTable) return;
        const editId = document.getElementById('edit-table-id');
        if (editId) editId.value = selectedTable.id;
        const editName = document.getElementById('edit-table-name');
        if (editName) editName.value = selectedTable.name;
        const editCap = document.getElementById('edit-table-capacity');
        if (editCap) editCap.value = selectedTable.capacity;
        const editShape = document.getElementById('edit-table-shape');
        if (editShape) editShape.value = selectedTable.shape;
        new bootstrap.Modal(document.getElementById('editTableModal')).show();
    };

    // Auto-select first table if available
    if (tableData.length > 0) {
        const first = tableData[0];
        selectTable({
            id: first.id || first.Id,
            code: first.code || first.Code,
            name: first.name || first.Name,
            capacity: first.capacity ?? first.Capacity ?? 4,
            status: first.status ?? first.Status ?? 0,
            shape: first.shape ?? first.Shape ?? 0,
            currentSessionCode: first.currentSessionCode || first.CurrentSessionCode,
            currentSessionTotal: first.currentSessionTotal ?? first.CurrentSessionTotal
        });
    }
}

// Ensure execution whether DOM is already ready or loading
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initFloorPlan);
} else {
    initFloorPlan();
}
