/**
 * SmartRestaurant POS SignalR Client & State Sync
 */
(function () {
    const badge = document.getElementById('signalr-badge');
    const badgeText = document.getElementById('signalr-text');

    function setStatus(status, text) {
        if (!badge || !badgeText) return;
        badge.className = 'badge rounded-pill px-3 py-2 d-flex align-items-center gap-2 ' +
            (status === 'connected' ? 'bg-success' : status === 'reconnecting' ? 'bg-warning text-dark' : 'bg-danger');
        badgeText.textContent = text;
    }

    if (typeof signalR === 'undefined') {
        console.warn('SignalR library is not loaded.');
        setStatus('error', 'Chế độ Ngoại tuyến');
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/table')
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.onreconnecting(error => {
        console.warn('SignalR Reconnecting...', error);
        setStatus('reconnecting', 'Mất mạng - Đang kết nối lại...');
    });

    connection.onreconnected(connectionId => {
        console.log('SignalR Reconnected:', connectionId);
        setStatus('connected', 'Trực Tuyến');

        // State Reconnection Handshake: Trigger client resync if viewing a table
        if (window.posTableId && window.posSync && typeof window.posSync.syncActiveTable === 'function') {
            window.posSync.syncActiveTable(window.posTableId);
        } else if (window.location.pathname.startsWith('/Floor') || window.location.pathname === '/') {
            window.location.reload();
        }
    });

    connection.onclose(error => {
        console.error('SignalR Closed:', error);
        setStatus('error', 'Mất kết nối máy chủ');
    });

    connection.on('TableStatusChanged', payloadJson => {
        try {
            const data = typeof payloadJson === 'string' ? JSON.parse(payloadJson) : payloadJson;
            console.log('Realtime TableStatusChanged:', data);

            // Update Floor view card if present
            const card = document.getElementById(`table-card-${data.TableId}`);
            if (card) {
                card.className = `pos-table-card state-${data.Status}`;
                const badgeEl = card.querySelector('.status-badge');
                if (badgeEl) {
                    badgeEl.textContent = getStatusText(data.Status);
                }
            }
        } catch (e) {
            console.error('Error handling TableStatusChanged:', e);
        }
    });

    function getStatusText(status) {
        switch (status) {
            case 1: case 'Available': return 'Trống';
            case 2: case 'Occupied': return 'Có Khách';
            case 3: case 'BillingRequested': return 'Yêu Cầu Tạm Tính';
            case 4: case 'CleaningNeeded': return 'Cần Dọn Dẹp';
            default: return 'Khác';
        }
    }

    async function start() {
        try {
            await connection.start();
            console.log('SignalR Connected to TableHub');
            setStatus('connected', 'Trực Tuyến');
        } catch (err) {
            console.error('SignalR Connection Error: ', err);
            setStatus('error', 'Mất kết nối');
            setTimeout(start, 5000);
        }
    }

    start();

    // Export sync helper
    window.posSync = {
        connection: connection,
        manualResync: function () {
            if (window.posTableId && typeof window.posSync.syncActiveTable === 'function') {
                window.posSync.syncActiveTable(window.posTableId);
            } else {
                window.location.reload();
            }
        }
    };
})();
