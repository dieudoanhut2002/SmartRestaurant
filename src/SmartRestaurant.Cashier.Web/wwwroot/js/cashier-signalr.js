/**
 * SmartRestaurant - Cashier Realtime SignalR
 */
(() => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/cashier')
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    connection.on('TableStatusChanged', (payloadJson) => {
        console.log('[SignalR Cashier] TableStatusChanged:', payloadJson);
        const data = JSON.parse(payloadJson);
        if (data.NewStatus === 3 || data.NewStatus === 'BillingRequested') {
            const toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 4000
            });
            toast.fire({
                icon: 'warning',
                title: `🔔 Bàn ${data.TableCode} vừa yêu cầu thanh toán!`
            });
            setTimeout(() => window.location.reload(), 1500);
        }
    });

    connection.on('InvoiceSettled', (payloadJson) => {
        console.log('[SignalR Cashier] InvoiceSettled:', payloadJson);
    });

    async function startConnection() {
        try {
            await connection.start();
            console.log('[SignalR Cashier] Đã kết nối thành công tới Cashier Hub.');
            await connection.invoke('JoinCashierGroup');
        } catch (err) {
            console.error('[SignalR Cashier] Lỗi kết nối SignalR:', err);
            setTimeout(startConnection, 5000);
        }
    }

    startConnection();
})();
