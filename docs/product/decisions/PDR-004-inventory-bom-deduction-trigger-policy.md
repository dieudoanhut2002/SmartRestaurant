# Product Decision Record: PDR-004 — Two-Phase Inventory Lock & Phased BOM Deduction Policy

- **Decision ID**: PDR-004
- **Title**: Two-Phase Inventory Lock (Khóa tồn kho 2 bước) and Phased BOM Rollout
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
In restaurant dining, delay occurs between order taking, kitchen prep, serving, and checkout. Deducting inventory only upon settlement causes overselling during peak hours for limited items (e.g., roasted duck, steaks, imported beer). Conversely, immediate deduction without reservation causes inventory drift when kitchen cannot fulfill or dishes are cancelled legitimately.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Phased Scope Boundary:
1. **v1.0 (MVP Scope)**:
   - Tracks **Direct Saleable / Countable Items**: Packaged drinks (bottled/canned beer, soft drinks, wine), prepared desserts, and discrete portion-based dishes.
   - Defer raw ingredient sub-gram recipe breakdown (BOM: gram of beef, ml of fish sauce) to **v1.1**.
   - Modular architecture must expose clean domain interfaces (`IInventoryDeductionEngine`) to allow plug-and-play activation of detailed BOM in v1.1 without structural refactoring.

### 2.2. Standard Inventory State Transitions & Formula:
$$\text{AvailableStock (Tồn khả dụng)} = \text{PhysicalStock (Tồn thực tế)} - \text{ReservedStock (Tồn tạm giữ)}$$

1. **Step 1 — Tạm Giữ (Reserve on Order Confirmation / Fire)**:
   - When an order line is confirmed or fired to kitchen/bar, the required quantity is allocated to `ReservedStock`.
   - When `AvailableStock <= 0`, system immediately triggers an out-of-stock notification and locks the item against new orders across all POS/waiter tablets.
2. **Step 2 — Tiêu Thụ Thực Tế (Consume on Prep / Serve)**:
   - When dish is marked as prepared or served (per station policy), reserved stock converts permanently to `ConsumedStock`:
     $$\text{PhysicalStock} \leftarrow \text{PhysicalStock} - \text{Quantity}$$
     $$\text{ReservedStock} \leftarrow \text{ReservedStock} - \text{Quantity}$$
3. **Step 3 — Hủy Món Hợp Lệ (Legitimate Cancellation)**:
   - When cancelled before preparation: System releases `ReservedStock` back to `AvailableStock`. No waste recorded.
4. **Step 4 — Hao Hụt / Hủy Bỏ (Waste & Spoilage)**:
   - When dish is cancelled after preparation has begun (spoiled, dropped, customer left): `ReservedStock` converts to `WasteStock` in the `WasteLog` table with reason code and manager authorization.

### 2.3. Concurrency & Idempotency Safeguards:
- **Preventing Negative Stock**: Concurrency control must ensure that parallel orders for the last remaining portions cannot cause negative available stock.
- **Idempotent Retry**: Network retry or re-submitting an order request must not execute duplicate stock reservations or double-deductions.

---

## 3. Consequences
- **Positive**: Eliminates overselling; clear audit trail of physical, reserved, consumed, and spoiled inventory; keeps MVP deployment fast and manageable.
- **Negative**: Deep raw ingredient COGS reporting deferred to v1.1.
