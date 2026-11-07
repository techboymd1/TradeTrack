document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("saleForm");

    if (!form) return;

    const linesContainer = document.getElementById("saleLines");
    const template = document.getElementById("saleLineTemplate");
    const addButton = document.getElementById("addSaleLine");
    const discountInput = form.querySelector("[data-discount]");

    const subtotalDisplay = document.getElementById("saleSubtotal");
    const discountDisplay = document.getElementById("saleDiscount");
    const totalDisplay = document.getElementById("saleGrandTotal");

    const formatter = new Intl.NumberFormat("en-NG", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    function money(value) {
        return `₦${formatter.format(value)}`;
    }

    function getRows() {
        return [...linesContainer.querySelectorAll("[data-sale-line]")];
    }

    function calculateTotals() {
        let subtotal = 0;

        getRows().forEach(row => {
            const product = row.querySelector("[data-product-select]");
            const quantityInput = row.querySelector("[data-quantity]");
            const lineTotal = row.querySelector("[data-line-total]");

            const selectedOption = product.selectedOptions[0];

            const price = Number(selectedOption?.dataset.price || 0);
            const quantity = Number(quantityInput.value || 0);

            const amount =
                product.value !== "0" && quantity > 0
                    ? price * quantity
                    : 0;

            subtotal += amount;

            lineTotal.textContent = money(amount);
        });

        subtotal = Math.round((subtotal + Number.EPSILON) * 100) / 100;

        const discount = Math.max(
            0,
            Number(discountInput.value || 0)
        );

        const total = Math.max(0, subtotal - discount);

        subtotalDisplay.textContent = money(subtotal);
        discountDisplay.textContent = money(discount);
        totalDisplay.textContent = money(total);
    }

    function renumberRows() {
        getRows().forEach((row, index) => {
            row.querySelector("[data-product-select]").name =
                `Lines[${index}].ProductId`;

            row.querySelector("[data-quantity]").name =
                `Lines[${index}].Quantity`;
        });
    }

    function addLine() {
        if (getRows().length >= 50) return;

        const index = getRows().length;
        const newRow = template.content.cloneNode(true);

        newRow.querySelectorAll("[name]").forEach(element => {
            element.name = element.name.replaceAll(
                "__index__",
                String(index)
            );
        });

        linesContainer.appendChild(newRow);
        calculateTotals();
    }

    addButton.addEventListener("click", addLine);

    linesContainer.addEventListener("change", calculateTotals);
    linesContainer.addEventListener("input", calculateTotals);

    linesContainer.addEventListener("click", event => {
        const removeButton = event.target.closest("[data-remove-line]");

        if (!removeButton) return;

        const rows = getRows();

        if (rows.length === 1) {
            rows[0].querySelector("[data-product-select]").value = "0";
            rows[0].querySelector("[data-quantity]").value = "1";
        } else {
            removeButton.closest("[data-sale-line]").remove();
        }

        renumberRows();
        calculateTotals();
    });

    discountInput.addEventListener("input", calculateTotals);

    form.addEventListener("submit", event => {
        for (const row of getRows()) {
            const product = row.querySelector("[data-product-select]");
            const quantity = Number(
                row.querySelector("[data-quantity]").value
            );

            if (
                product.value === "0" ||
                !Number.isInteger(quantity) ||
                quantity < 1
            ) {
                event.preventDefault();
                alert("Select a product and enter a whole-number quantity.");
                return;
            }

            const available = Number(
                product.selectedOptions[0]?.dataset.stock || 0
            );

            if (quantity > available) {
                event.preventDefault();
                alert(`Only ${available} units are available.`);
                return;
            }
        }

        renumberRows();
    });

    calculateTotals();
});