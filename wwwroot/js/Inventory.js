document.addEventListener("DOMContentLoaded", function () {

    const searchInput = document.getElementById("productSearch");
    const categoryFilter = document.getElementById("categoryFilter");
    const stockFilter = document.getElementById("stockFilter");
    const clearButton = document.getElementById("clearProductFilters");

    const resultCount = document.getElementById("productResultCount");
    const noResults = document.getElementById("noMatchingProducts");

    const productRows = Array.from(
        document.querySelectorAll("[data-product-row]")
    );

    if (
        !searchInput ||
        !categoryFilter ||
        !stockFilter ||
        !clearButton
    ) {
        return;
    }

    if (productRows.length === 0) {
        searchInput.disabled = true;
        categoryFilter.disabled = true;
        stockFilter.disabled = true;
        clearButton.disabled = true;

        return;
    }


    // Get the available categories from the product rows.
    const categories = [
        ...new Set(
            productRows
                .map(row => row.dataset.category.trim())
                .filter(category => category !== "")
        )
    ].sort((a, b) => a.localeCompare(b));


    // Add each category to the dropdown.
    categories.forEach(category => {

        const option = document.createElement("option");

        option.value = category;
        option.textContent = category;

        categoryFilter.appendChild(option);

    });


    // Search and filter the product rows.
    function filterProducts() {

        const searchTerm = searchInput.value.trim().toLowerCase();
        const selectedCategory = categoryFilter.value.toLowerCase();
        const selectedStock = stockFilter.value;

        let visibleProducts = 0;

        productRows.forEach(row => {

            const productText = row.textContent.toLowerCase();

            const productCategory = (
                row.dataset.category || ""
            ).toLowerCase();

            const productStock = row.dataset.stockStatus;

            const matchesSearch = productText.includes(searchTerm);

            const matchesCategory =
                selectedCategory === "all" ||
                productCategory === selectedCategory;

            const matchesStock =
                selectedStock === "all" ||
                productStock === selectedStock;

            const isVisible =
                matchesSearch &&
                matchesCategory &&
                matchesStock;

            row.hidden = !isVisible;

            if (isVisible) {
                visibleProducts++;
            }

        });

        resultCount.textContent =
            `${visibleProducts} of ${productRows.length} products`;

        noResults.hidden = visibleProducts !== 0;
    }


    // Restore the original filter settings.
    function clearFilters() {

        searchInput.value = "";
        categoryFilter.value = "all";
        stockFilter.value = "all";

        filterProducts();
    }


    searchInput.addEventListener("input", filterProducts);

    categoryFilter.addEventListener("change", filterProducts);

    stockFilter.addEventListener("change", filterProducts);

    clearButton.addEventListener("click", clearFilters);

});