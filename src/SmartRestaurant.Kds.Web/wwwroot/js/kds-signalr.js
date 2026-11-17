/**
 * SmartRestaurant - KDS Realtime SignalR Connection
 */
(() => {
    const stationId = window.kdsCurrentStationId || '';

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/kitchen')
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    connection.on('OrderFired', (payloadJson) => {
        console.log('[SignalR KDS] Nhận thông báo OrderFired:', payloadJson);
        kdsApp.playChime();
        // Brief toast alert and refresh cards
        const toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        toast.fire({
            icon: 'info',
            title: '🔥 Có phiếu gọi món mới vừa gửi bếp!'
        });
        setTimeout(() => window.location.reload(), 1500);
    });

    connection.on('KitchenTicketUpdated', (payloadJson) => {
        console.log('[SignalR KDS] Nhận thông báo KitchenTicketUpdated:', payloadJson);
        setTimeout(() => window.location.reload(), 1000);
    });

    connection.on('OrderItemStatusChanged', (payloadJson) => {
        console.log('[SignalR KDS] Nhận thông báo OrderItemStatusChanged:', payloadJson);
    });

    connection.on('Item86Toggled', (payloadJson) => {
        console.log('[SignalR KDS] Nhận thông báo Item86Toggled:', payloadJson);
    });

    async function startConnection() {
        try {
            await connection.start();
            console.log('[SignalR KDS] Đã kết nối thành công tới Kitchen Hub.');
            if (stationId) {
                await connection.invoke('JoinStationGroup', stationId);
                console.log(`[SignalR KDS] Đã tham gia nhóm trạm bếp: station_${stationId}`);
            }
        } catch (err) {
            console.error('[SignalR KDS] Lỗi kết nối SignalR:', err);
            setTimeout(startConnection, 5000);
        }
    }

    startConnection();
})();
