// SmartRestaurant — Konva.js Floor Plan 2D Designer
document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('floorplan-container');
    if (!container || typeof Konva === 'undefined' || typeof tableData === 'undefined') {
        return;
    }

    const width = container.offsetWidth || 800;
    const height = container.offsetHeight || 600;

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
        3: '#dc3545', // CleaningNeeded (Red/Orange)
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

    // Render Tables
    tableData.forEach(function (t) {
        const group = new Konva.Group({
            x: t.positionX || 50,
            y: t.positionY || 50,
            draggable: true,
            id: t.id
        });

        const color = statusColors[t.status] || '#198754';
        const w = t.width > 0 ? t.width : 90;
        const h = t.height > 0 ? t.height : 80;

        let shapeNode;
        if (t.shape === 1) { // Round
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
                cornerRadius: t.shape === 2 ? 8 : 12,
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
            text: t.code,
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
            text: `${t.capacity} chỗ`,
            fontSize: 11,
            fontFamily: 'sans-serif',
            fill: '#6c757d',
            align: 'center'
        });

        group.add(shapeNode);
        group.add(codeText);
        group.add(capText);

        // Click / Touch interaction
        group.on('click tap', function () {
            selectTable(t);
        });

        // Drag end: save coordinates to server via AJAX
        group.on('dragend', function () {
            saveTablePosition(t.id, group.x(), group.y(), w, h, group.rotation());
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
        document.getElementById('no-table-selected').style.display = 'none';
        document.getElementById('table-details').style.display = 'block';

        document.getElementById('inspect-code').textContent = table.code;
        document.getElementById('inspect-name').textContent = table.name;
        document.getElementById('inspect-capacity').textContent = `${table.capacity} khách`;
        document.getElementById('inspect-shape').textContent = table.shape === 1 ? 'Bàn Tròn' : (table.shape === 2 ? 'Bàn Vuông' : 'Bàn Chữ Nhật');
        document.getElementById('inspect-session').textContent = table.currentSessionCode || 'Chưa mở phiên';
        document.getElementById('inspect-total').textContent = table.currentSessionTotal ? table.currentSessionTotal.toLocaleString('vi-VN') + ' ₫' : '0 ₫';

        const badge = document.getElementById('inspect-status-badge');
        badge.textContent = statusNames[table.status] || 'Trống';
        badge.style.backgroundColor = statusColors[table.status] || '#198754';
        badge.className = 'badge px-3 py-2 fs-6 text-white';

        document.getElementById('delete-table-id').value = table.id;
    }

    async function saveTablePosition(tableId, x, y, width, height, rotation) {
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
        if (!selectedTable) return;
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
        document.getElementById('edit-table-id').value = selectedTable.id;
        document.getElementById('edit-table-name').value = selectedTable.name;
        document.getElementById('edit-table-capacity').value = selectedTable.capacity;
        document.getElementById('edit-table-shape').value = selectedTable.shape;
        new bootstrap.Modal(document.getElementById('editTableModal')).show();
    };

    // Auto-select first table if available
    if (tableData.length > 0) {
        selectTable(tableData[0]);
    }
});
