const formatter = new Intl.NumberFormat('en-US', {
	style: 'currency',
	currency: 'USD',
});

function addToCart(id) {
	$.ajax({
		type: "POST",
		url: '/addcart/' + id,
		// data: { productId: id },
		success: function (response) {
			if (response.status == 'OK') {
				console.log(response.data.cartHtml)
				$('.cart-empty').remove()
				$('#cart-table').remove()
				$('#cart-table-non-reload').html(response.data.cartHtml);

				updateSummaryTotals(response.data);
				showAlert('success', 'Product added to cart!', 'top');
				updateCartInfo();
			} else {
				showAlert('danger', 'Failed to add product to cart.', 'top');
			}
		},
		error: function () {
			showAlert('danger', 'An error occurred while adding the product to the cart.','top');
		}
	});
}

function updateQuantity(productId, newQuantity) {
	// Check valid value
	if (isNaN(newQuantity) || newQuantity < 1) {
		newQuantity = 1;
	}

	// Update input field with new quantity
	$('#quantity-' + productId).val(newQuantity);

	// Send AJAX request to update quantity
	$.ajax({
		type: "POST",
		url: '/updatecart',
		data: {
			productId: productId,
			quantity: newQuantity
		},
		success: function (response) {
			if (response.status === 'OK') {
				// Update the UI with new values
				const updatedData = response.data;

				// Update quantity and total price for this product row
				const rowId = `#row-${productId}`;
				$(rowId).find('td:nth-child(4)').text(formatter.format(updatedData.priceCol));
				$(rowId).find('td:nth-child(5)').text(formatter.format(updatedData.discountCol));
				$(rowId).find('td:nth-child(6)').text(formatter.format(updatedData.taxCol));
				$(rowId).find('td:nth-child(7)').text(formatter.format(updatedData.totalCol));
				// Optionally update summary totals
				updateSummaryTotals(response.data);

				updateCartInfo()
				console.log("Updated quantity successful.");
			} else {
				showAlert('danger', 'Update quantity failed.', 'top');
			}
		},
		error: function (error) {
			console.error('Error updating quantity:', error);
			showAlert('danger', 'Error updating quantity.', 'top');
		}
	});
}

// Function to update summary totals
function updateSummaryTotals(data) {
	// Update SubTotal, DiscountTotal, TaxTotal, GrandTotal
	$('#price-total').text(formatter.format(data.priceTotal));
	$('#discount-total').text(formatter.format(data.discountTotal));
	$('#tax-total').text(formatter.format(data.taxTotal));
	$('#grand-total').text(formatter.format(data.grandTotal));

	const grandTotalText = `TOTAL (${formatter.format(data.priceTotal)} - ${formatter.format(data.discountTotal)} + ${formatter.format(data.taxTotal)}) =`;
	$('#total-text').html(`<strong>${grandTotalText}</strong>`);
}

function updateCartInfo() {
	$.ajax({
		type: "GET",
		url: '/cartinfo',
		success: function (data) {
			console.log(data)
			$('#cart-count-header').text(data.cartCount);
			$('#cart-count-sidebar').text(data.cartCount);
			$('#cart-count-cart').text(data.cartCount);

			$('#grand-total-header').text(formatter.format(data.grandTotal))
			$('#grand-total-sidebar').text(formatter.format(data.grandTotal))
		},
		error: function () {
			console.error('Error updating cart info.');
		}
	});
}

// Handle button click events for update
$(document).on('click', 'button.update-qty-btn', function () {
	var productId = $(this).attr('product-id');
	// Determine if it's plus or minus button
	var change = $(this).find('i').hasClass('icon-plus') ? 1 : -1; 

	var quantityInput = $('#quantity-' + productId);
	var currentQuantity = parseInt(quantityInput.val());

	// Calculate new quantity
	var newQuantity = currentQuantity + change;
	updateQuantity(productId, newQuantity);
});

// Handle input changes for manual quantity update
$(document).on('input', 'input[id^="quantity-"]', function () {
	// quantity-{id} => ['quantity', '{id}']
	var productId = this.id.split('-')[1];
	var newQuantity = parseInt($(this).val());

	updateQuantity(productId, newQuantity);
});

$(document).on('click', 'button.remove-row-btn', function () {
	var id = $(this).attr('product-id');
	$.ajax({
		type: "POST",
		url: '/removecart/' + id,
		// data: { productId: id },
		success: function (response) {
			if (response.status == 'OK') {

				$(`#row-${id}`).remove();
				// ^= use for dynamic id
				console.log($('tbody tr[id^="row-"]').length); 
				if ($('tbody tr[id^="row-"]').length === 0) {
					console.log(2)
					$('#cart-table').hide();
					showAlert('info', 'Empty. No products have been added to the cart.', 'bottom')
				}
				console.log(response.data)
				updateSummaryTotals(response.data);
				showAlert('success', 'Product removed from cart!', 'top');
				updateCartInfo();
			} else {
				showAlert('danger', 'Failed to remove product from cart.', 'top');
			}
		},
		error: function () {
			showAlert('danger', 'An error occurred while removing the product from the cart.', 'top');
		}
	});
});




