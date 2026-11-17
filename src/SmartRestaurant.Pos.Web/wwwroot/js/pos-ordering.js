/**
 * SmartRestaurant POS Ordering & Client Cart Script
 */
(function () {
    let draftCart = [];
    const tableId = window.posTableId;
    const storageKey = `pos_draft_${tableId}`;

    // Load initial draft from LocalStorage if exists
    try {
        const saved = localStorage.getItem(storageKey);
        if (saved) {
            draftCart = JSON.parse(saved);
        }
    } catch (e) {
        draftCart = [];
    }

    // Initialize UI
    renderDraftCart();

    // 1. Filter Dishes by Category
    window.selectCategory = function (categoryId, btn) {
        document.querySelectorAll('.cat-pill').forEach(b => b.classList.remove('active'));
        if (btn) btn.classList.add('active');

        const cards = document.querySelectorAll('.dish-card');
        cards.forEach(card => {
            const itemCat = card.getAttribute('data-category');
            if (categoryId === 'all' || itemCat === categoryId) {
                card.style.display = 'flex';
            } else {
                card.style.display = 'none';
            }
        });
    };

    // 2. Fast Search Filter (<100ms)
    window.filterDishes = function () {
        const term = (document.getElementById('dish-search-input').value || '').trim().toLowerCase();
        const cards = document.querySelectorAll('.dish-card');

        cards.forEach(card => {
            const name = (card.getAttribute('data-name') || '').toLowerCase();
            const code = (card.getAttribute('data-code') || '').toLowerCase();

            if (term === '' || name.includes(term) || code.includes(term)) {
                card.style.display = 'flex';
            } else {
                card.style.display = 'none';
            }
        });
    };

    window.clearSearch = function () {
        const input = document.getElementById('dish-search-input');
        if (input) {
            input.value = '';
            filterDishes();
        }
    };

    // 3. Dish Click Action
    window.onDishClicked = function (dishId) {
        const item = (window.posCatalog.Items || []).find(i => i.Id === dishId);
        if (!item) return;

        const hasVariants = item.Variants && item.Variants.length > 0;
        const hasModifiers = item.ModifierGroups && item.ModifierGroups.length > 0;

        if (hasVariants || hasModifiers) {
            openDishCustomizer(item);
        } else {
            addSimpleDishToCart(item);
        }
    };

    function addSimpleDishToCart(item) {
        const existing = draftCart.find(d => d.menuItemId === item.Id && !d.menuItemVariantId && (!d.modifiers || d.modifiers.length === 0));
        if (existing) {
            existing.quantity += 1;
        } else {
            draftCart.push({
                menuItemId: item.Id,
                menuItemVariantId: null,
                itemName: item.Name,
                unitPrice: item.BasePrice,
                quantity: 1,
                kitchenNote: null,
                modifiers: []
            });
        }

        saveCartAndRender();
    }

    // 4. Dish Customizer Modal
    function openDishCustomizer(item) {
        document.getElementById('customizer-dish-id').value = item.Id;
        document.getElementById('customizer-base-price').value = item.BasePrice;
        document.getElementById('customizer-dish-name').textContent = item.Name;
        document.getElementById('customizer-note').value = '';

        // Variants
        const vContainer = document.getElementById('variants-container');
        const vList = document.getElementById('variants-list');
        vList.innerHTML = '';

        if (item.Variants && item.Variants.length > 0) {
            vContainer.classList.remove('d-none');
            // Base option
            vList.innerHTML += `
                <div class="form-check form-check-inline">
                    <input class="btn-check" type="radio" name="variant-radio" id="v-base" value="" data-price="${item.BasePrice}" checked>
                    <label class="btn btn-outline-warning btn-sm px-3" for="v-base">Tiêu chuẩn (${formatMoney(item.BasePrice)})</label>
                </div>
            `;
            item.Variants.forEach(v => {
                const price = item.BasePrice + v.PriceDifference;
                vList.innerHTML += `
                    <div class="form-check form-check-inline">
                        <input class="btn-check" type="radio" name="variant-radio" id="v-${v.Id}" value="${v.Id}" data-price="${price}" data-name="${v.VariantName}">
                        <label class="btn btn-outline-warning btn-sm px-3" for="v-${v.Id}">${v.VariantName} (${formatMoney(price)})</label>
                    </div>
                `;
            });
        } else {
            vContainer.classList.add('d-none');
        }

        // Modifiers
        const mContainer = document.getElementById('modifiers-container');
        const mList = document.getElementById('modifiers-groups-list');
        mList.innerHTML = '';

        if (item.ModifierGroups && item.ModifierGroups.length > 0) {
            mContainer.classList.remove('d-none');
            item.ModifierGroups.forEach(g => {
                let groupHtml = `<div class="mb-2"><small class="text-white fw-bold">${g.GroupName}:</small><div class="d-flex flex-wrap gap-2 mt-1">`;
                g.Options.forEach(o => {
                    const priceAdj = o.PriceAdjustment > 0 ? ` (+${formatMoney(o.PriceAdjustment)})` : '';
                    groupHtml += `
                        <div class="form-check form-check-inline">
                            <input class="form-check-input mod-option-checkbox" type="checkbox" id="mod-${o.Id}" value="${o.Id}" data-name="${o.OptionName}" data-price="${o.PriceAdjustment}">
                            <label class="form-check-label text-light small" for="mod-${o.Id}">${o.OptionName}${priceAdj}</label>
                        </div>
                    `;
                });
                groupHtml += `</div></div>`;
                mList.innerHTML += groupHtml;
            });
        } else {
            mContainer.classList.add('d-none');
        }

        const modal = new bootstrap.Modal(document.getElementById('modalDishCustomizer'));
        modal.show();
    }

    window.appendNote = function (text) {
        const noteArea = document.getElementById('customizer-note');
        if (noteArea.value) {
            noteArea.value += ', ' + text;
        } else {
            noteArea.value = text;
        }
    };

    window.confirmAddCustomizedDish = function () {
        const dishId = document.getElementById('customizer-dish-id').value;
        const item = (window.posCatalog.Items || []).find(i => i.Id === dishId);
        if (!item) return;

        // Selected variant
        const selectedVariantRadio = document.querySelector('input[name="variant-radio"]:checked');
        let variantId = null;
        let variantName = '';
        let unitPrice = item.BasePrice;

        if (selectedVariantRadio && selectedVariantRadio.value) {
            variantId = selectedVariantRadio.value;
            variantName = selectedVariantRadio.getAttribute('data-name') || '';
            unitPrice = parseFloat(selectedVariantRadio.getAttribute('data-price')) || item.BasePrice;
        }

        // Selected modifiers
        const selectedModifiers = [];
        document.querySelectorAll('.mod-option-checkbox:checked').forEach(cb => {
            selectedModifiers.push({
                modifierOptionId: cb.value,
                optionName: cb.getAttribute('data-name'),
                priceAdjustment: parseFloat(cb.getAttribute('data-price')) || 0
            });
        });

        const kitchenNote = (document.getElementById('customizer-note').value || '').trim() || null;
        const displayName = variantName ? `${item.Name} (${variantName})` : item.Name;

        draftCart.push({
            menuItemId: item.Id,
            menuItemVariantId: variantId,
            itemName: displayName,
            unitPrice: unitPrice,
            quantity: 1,
            kitchenNote: kitchenNote,
            modifiers: selectedModifiers
        });

        saveCartAndRender();

        const modalEl = document.getElementById('modalDishCustomizer');
        const modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) modal.hide();
    };

    // 5. Render Cart
    function renderDraftCart() {
        const container = document.getElementById('draft-items-list');
        const emptyNotice = document.getElementById('empty-draft-notice');
        const countBadge = document.getElementById('draft-count-badge');
        const btnCount = document.getElementById('draft-btn-count');

        if (!container) return;

        if (draftCart.length === 0) {
            container.innerHTML = '';
            if (emptyNotice) emptyNotice.style.display = 'block';
            if (countBadge) countBadge.textContent = '0';
            if (btnCount) btnCount.textContent = '0';
            recalculateSummary();
            return;
        }

        if (emptyNotice) emptyNotice.style.display = 'none';

        let html = '';
        draftCart.forEach((item, index) => {
            let modsText = '';
            if (item.modifiers && item.modifiers.length > 0) {
                modsText = `<div class="small text-info">+ ${item.modifiers.map(m => m.optionName).join(', ')}</div>`;
            }
            let noteText = '';
            if (item.kitchenNote) {
                noteText = `<div class="small text-warning fst-italic"><i class="bi bi-chat-dots me-1"></i>${item.kitchenNote}</div>`;
            }

            const itemLineTotal = (item.unitPrice + (item.modifiers ? item.modifiers.reduce((s, m) => s + m.priceAdjustment, 0) : 0)) * item.quantity;

            html += `
                <div class="cart-item-row">
                    <div style="flex: 1;">
                        <div class="fw-bold text-white">${item.itemName}</div>
                        ${modsText}
                        ${noteText}
                        <div class="small text-secondary">${formatMoney(item.unitPrice)} &times; ${item.quantity} = <span class="text-info fw-bold">${formatMoney(itemLineTotal)}</span></div>
                    </div>
                    <div class="d-flex align-items-center gap-2">
                        <button class="btn btn-outline-secondary btn-sm px-2 py-1" onclick="posCart.adjustQty(${index}, -1)">
                            <i class="bi bi-dash"></i>
                        </button>
                        <span class="fw-bold fs-6 text-white" style="min-width: 24px; text-align: center;">${item.quantity}</span>
                        <button class="btn btn-outline-secondary btn-sm px-2 py-1" onclick="posCart.adjustQty(${index}, 1)">
                            <i class="bi bi-plus"></i>
                        </button>
                        <button class="btn btn-outline-danger btn-sm px-2 py-1 ms-1" onclick="posCart.removeItem(${index})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            `;
        });

        container.innerHTML = html;
        const totalItemsCount = draftCart.reduce((s, i) => s + i.quantity, 0);
        if (countBadge) countBadge.textContent = totalItemsCount;
        if (btnCount) btnCount.textContent = totalItemsCount;

        recalculateSummary();
    }

    function saveCartAndRender() {
        try {
            localStorage.setItem(storageKey, JSON.stringify(draftCart));
        } catch (e) { }
        renderDraftCart();
    }

    function recalculateSummary() {
        const subtotalEl = document.getElementById('summary-subtotal');
        const totalEl = document.getElementById('summary-total');

        const existingSubtotal = window.posSession ? window.posSession.Subtotal : 0;
        const surcharges = window.posSession ? window.posSession.Surcharges : 0;

        let draftSubtotal = 0;
        draftCart.forEach(item => {
            const modSum = item.modifiers ? item.modifiers.reduce((s, m) => s + m.priceAdjustment, 0) : 0;
            draftSubtotal += (item.unitPrice + modSum) * item.quantity;
        });

        const combinedSubtotal = existingSubtotal + draftSubtotal;
        const combinedTotal = combinedSubtotal + surcharges;

        if (subtotalEl) subtotalEl.textContent = formatMoney(combinedSubtotal);
        if (totalEl) totalEl.textContent = formatMoney(combinedTotal);
    }

    // 6. Action: BÁO BẾP (FireOrder with Idempotency UUID)
    window.fireOrder = async function () {
        if (draftCart.length === 0) {
            Swal.fire('Giỏ hàng trống', 'Vui lòng chọn món trước khi bấm Báo bếp.', 'warning');
            return;
        }

        const staffId = document.getElementById('active-staff-select').value;
        const idempotencyKey = crypto.randomUUID();

        // Show loading
        Swal.fire({
            title: 'Đang gửi vé xuống bếp...',
            text: 'Vui lòng chờ xác nhận từ trạm chế biến',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        try {
            const response = await fetch('/Order/Fire', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    tableId: tableId,
                    orderId: window.posSession.ActiveOrderId,
                    idempotencyKey: idempotencyKey,
                    staffId: staffId,
                    note: null,
                    items: draftCart
                })
            });

            const res = await response.json();
            if (res.success) {
                // Clear local cart
                draftCart = [];
                localStorage.removeItem(storageKey);
                renderDraftCart();

                Swal.fire({
                    icon: 'success',
                    title: 'BÁO BẾP THÀNH CÔNG!',
                    text: res.message,
                    timer: 1500,
                    showConfirmButton: false
                }).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire('Lỗi Báo Bếp', res.message || 'Không thể gửi vé xuống bếp.', 'error');
            }
        } catch (e) {
            Swal.fire('Lỗi kết nối', 'Không thể kết nối với máy chủ.', 'error');
        }
    };

    // 7. Request Bill
    window.requestBill = async function () {
        const result = await Swal.fire({
            title: `Yêu cầu tạm tính bàn ${window.posSession.TableCode}?`,
            text: 'Chuyển trạng thái bàn sang Tạm Tính (Màu tím) và thông báo tới thu ngân.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Xác Nhận Tạm Tính',
            cancelButtonText: 'Đóng'
        });

        if (result.isConfirmed) {
            const staffId = document.getElementById('active-staff-select').value;
            const resp = await fetch('/Floor/RequestBill', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ tableId: tableId, staffId: staffId })
            });

            const res = await resp.json();
            if (res.success) {
                Swal.fire('Thành công', res.message, 'success').then(() => location.reload());
            } else {
                Swal.fire('Lỗi', res.message, 'error');
            }
        }
    };

    // 8. Transfer Table Modal
    window.openTransferModal = async function () {
        try {
            const resp = await fetch('/Floor');
            // Fetch tables for target selection
            const modal = new bootstrap.Modal(document.getElementById('modalTransfer'));
            modal.show();
        } catch (e) { }
    };

    function formatMoney(amount) {
        return (amount || 0).toLocaleString('vi-VN') + ' ₫';
    }

    // Export Cart operations
    window.posCart = {
        adjustQty: function (index, delta) {
            if (!draftCart[index]) return;
            draftCart[index].quantity += delta;
            if (draftCart[index].quantity <= 0) {
                draftCart.splice(index, 1);
            }
            saveCartAndRender();
        },
        removeItem: function (index) {
            if (!draftCart[index]) return;
            draftCart.splice(index, 1);
            saveCartAndRender();
        }
    };

    // Resync implementation for SignalR
    window.posSync.syncActiveTable = async function (tId) {
        try {
            const resp = await fetch(`/api/pos/sync/${tId}`);
            const res = await resp.json();
            if (res.success && res.data && res.data.SessionDetail) {
                window.posSession = res.data.SessionDetail;
                recalculateSummary();
                console.log('Table state synchronized successfully.');
            }
        } catch (e) {
            console.error('Failed to sync table state:', e);
        }
    };
})();
