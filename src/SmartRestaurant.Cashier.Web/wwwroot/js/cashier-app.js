/**
 * SmartRestaurant - Cashier Terminal Logic
 */
const cashierApp = (() => {
    function init() {
        initClock();
    }

    function initClock() {
        const clockEl = document.getElementById('cashier-clock');
        function update() {
            if (clockEl) {
                const now = new Date();
                clockEl.textContent = now.toLocaleTimeString('vi-VN', { hour12: false });
            }
        }
        update();
        setInterval(update, 1000);
    }

    // Cash Denomination Helpers
    function setCashGiven(amount) {
        const input = document.getElementById('cash-given-input');
        if (input) {
            input.value = amount;
            calculateCashChange(amount);
        }
    }

    function addCash(extra) {
        const input = document.getElementById('cash-given-input');
        if (input) {
            const current = parseFloat(input.value) || 0;
            input.value = current + extra;
            const total = parseFloat(document.querySelector('.card-footer .fs-3')?.textContent?.replace(/[^0-9]/g, '')) || 0;
            calculateCashChange(total);
        }
    }

    function calculateCashChange(totalAmount) {
        const input = document.getElementById('cash-given-input');
        const display = document.getElementById('cash-change-display');
        if (!input || !display) return;

        const given = parseFloat(input.value) || 0;
        const change = Math.max(0, given - totalAmount);
        display.textContent = `${change.toLocaleString('vi-VN')} ₫`;
    }

    // Settlement Actions
    async function settleBill(invoiceId, paymentMethod, cashierId, totalAmount, ref = null) {
        const givenInput = document.getElementById('cash-given-input');
        let givenAmount = totalAmount;
        let changeAmount = 0;

        if (paymentMethod === 1 && givenInput) {
            givenAmount = parseFloat(givenInput.value) || totalAmount;
            if (givenAmount < totalAmount) {
                Swal.fire('Cảnh báo', 'Tiền khách đưa chưa đủ để thanh toán hóa đơn!', 'warning');
                return;
            }
            changeAmount = Math.max(0, givenAmount - totalAmount);
        }

        try {
            const res = await fetch('/api/payment/settle', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    invoiceId: invoiceId,
                    paymentMethod: paymentMethod,
                    amount: totalAmount,
                    changeAmount: changeAmount,
                    transactionRef: ref || (paymentMethod === 1 ? 'CASH' : 'POS'),
                    cashierStaffId: cashierId
                })
            });

            const data = await res.json();
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Quyết Toán Thành Công!',
                    text: `Hóa đơn đã thanh toán ${totalAmount.toLocaleString('vi-VN')} ₫. Bàn đã được chuyển về trạng thái sẵn sàng đón khách mới!`,
                    showCancelButton: true,
                    confirmButtonText: '🖨️ In Hóa Đơn',
                    cancelButtonText: 'Về Danh Sách Bàn'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.open(`/Cashier/Receipt/${invoiceId}`, '_blank');
                    }
                    window.location.href = '/Cashier';
                });
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể thanh toán hóa đơn', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    async function settleCardBill(invoiceId, cashierId, totalAmount) {
        const refInput = document.getElementById('card-ref-input');
        const ref = refInput?.value || 'POS-CARD';
        await settleBill(invoiceId, 2, cashierId, totalAmount, ref);
    }

    // VietQR Simulated Bank Webhook
    async function simulateVietQrWebhook(orderCode, amount) {
        try {
            const randomRef = `FT${Date.now()}`;
            const res = await fetch('/api/payment/vietqr-webhook', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    transactionRef: randomRef,
                    amount: amount,
                    orderCode: orderCode,
                    bankCode: 'MB',
                    payerInfo: 'NGUYEN VAN TESTER'
                })
            });

            const data = await res.json();
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Webhook Ngân Hàng Thành Công!',
                    text: `Ngân hàng đã gửi thông báo giao dịch ${amount.toLocaleString('vi-VN')} ₫ (Mã chuẩn chi: ${randomRef}). Hóa đơn đã tự động quyết toán!`,
                    timer: 2500,
                    showConfirmButton: false
                }).then(() => {
                    window.location.href = '/Cashier';
                });
            } else {
                Swal.fire('Lỗi Webhook', data.message, 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    // Split Bill Calculations
    function calculateEqualSplit(totalAmount) {
        const parts = parseInt(document.getElementById('equal-parts-input')?.value) || 2;
        const each = Math.round(totalAmount / parts);
        const display = document.getElementById('equal-part-amount');
        if (display) {
            display.textContent = `${each.toLocaleString('vi-VN')} ₫`;
        }
    }

    async function executeEqualSplit(orderId, cashierId) {
        const parts = parseInt(document.getElementById('equal-parts-input')?.value) || 2;
        try {
            const res = await fetch('/api/payment/split-bill', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    orderId: orderId,
                    splitMode: 'equal',
                    equalParts: parts,
                    cashierStaffId: cashierId
                })
            });
            const data = await res.json();
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: `Đã tách thành ${parts} hóa đơn con!`,
                    text: 'Mỗi người có thể thanh toán riêng bằng tiền mặt hoặc chuyển khoản QR.',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    window.location.href = '/Cashier';
                });
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể tách hóa đơn', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    async function executeItemSplit(orderId, cashierId) {
        const checkboxes = document.querySelectorAll('.split-item-checkbox');
        const group1 = [];
        const group2 = [];

        checkboxes.forEach(cb => {
            const itemId = cb.getAttribute('data-item-id');
            const qty = parseInt(cb.getAttribute('data-qty')) || 1;
            if (cb.checked) {
                group1.push({ orderItemId: itemId, quantity: qty });
            } else {
                group2.push({ orderItemId: itemId, quantity: qty });
            }
        });

        if (group1.length === 0 || group2.length === 0) {
            Swal.fire('Cảnh báo', 'Vui lòng phân bổ món ăn vào cả 2 hóa đơn (không để hóa đơn nào bị trống)!', 'warning');
            return;
        }

        try {
            const res = await fetch('/api/payment/split-bill', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    orderId: orderId,
                    splitMode: 'by_items',
                    allocations: [group1, group2],
                    cashierStaffId: cashierId
                })
            });
            const data = await res.json();
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Đã tách thành 2 hóa đơn theo món!',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    window.location.href = '/Cashier';
                });
            } else {
                Swal.fire('Lỗi', data.message || 'Không thể tách hóa đơn', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    // Shift Operations
    async function openShift() {
        const floatInput = document.getElementById('opening-float-input');
        const notesInput = document.getElementById('open-notes-input');
        const amount = parseFloat(floatInput?.value) || 0;

        try {
            const res = await fetch('/api/payment/open-shift', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    staffId: '00000000-0000-0000-0000-000000000000',
                    openingCashFloat: amount,
                    notes: notesInput?.value || ''
                })
            });
            const data = await res.json();
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Mở Ca Thành Công!',
                    timer: 1500,
                    showConfirmButton: false
                }).then(() => window.location.reload());
            } else {
                Swal.fire('Lỗi', data.message, 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    async function closeShiftBlind(shiftId) {
        const cashInput = document.getElementById('actual-cash-input');
        const notesInput = document.getElementById('shift-notes-input');
        const actualCash = parseFloat(cashInput?.value);

        if (isNaN(actualCash) || actualCash < 0) {
            Swal.fire('Cảnh báo', 'Vui lòng nhập số tiền mặt thực tế kiểm đếm trong két!', 'warning');
            return;
        }

        try {
            const res = await fetch('/api/payment/blind-shift-close', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    shiftId: shiftId,
                    actualCashCount: actualCash,
                    notes: notesInput?.value || '',
                    cashierStaffId: '00000000-0000-0000-0000-000000000000'
                })
            });

            const data = await res.json();
            if (data.success) {
                const info = data.data;
                const statusHtml = info.hasDiscrepancy
                    ? `<div class="text-danger fw-bold fs-5">Phát hiện chênh lệch: ${info.varianceAmount > 0 ? '+' : ''}${info.varianceAmount.toLocaleString('vi-VN')} ₫</div>`
                    : `<div class="text-success fw-bold fs-5">Khớp 100% không chênh lệch!</div>`;

                Swal.fire({
                    icon: info.hasDiscrepancy ? 'warning' : 'success',
                    title: 'Đã Đóng Ca Thu Ngân!',
                    html: `
                        <div class="text-start">
                            <p>Tiền quỹ đầu ca: <strong>${info.openingFloat.toLocaleString('vi-VN')} ₫</strong></p>
                            <p>Doanh thu tiền mặt hệ thống: <strong>${info.systemCashTotal.toLocaleString('vi-VN')} ₫</strong></p>
                            <p>Tiền kỳ vọng sổ sách: <strong>${info.expectedAmount.toLocaleString('vi-VN')} ₫</strong></p>
                            <p>Tiền thực tế bạn đếm: <strong>${info.actualCashCount.toLocaleString('vi-VN')} ₫</strong></p>
                            <hr />
                            ${statusHtml}
                        </div>
                    `,
                    confirmButtonText: 'Hoàn Tất'
                }).then(() => window.location.reload());
            } else {
                Swal.fire('Lỗi', data.message, 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', e.message, 'error');
        }
    }

    return {
        init,
        setCashGiven,
        addCash,
        calculateCashChange,
        settleBill,
        settleCardBill,
        simulateVietQrWebhook,
        calculateEqualSplit,
        executeEqualSplit,
        executeItemSplit,
        openShift,
        closeShiftBlind
    };
})();

document.addEventListener('DOMContentLoaded', () => {
    cashierApp.init();
});
