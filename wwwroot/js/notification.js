/*window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(300, function () {
        $(this).remove();
    });
}, 5000);*/

$(".alert").hide().slideDown(300).delay(5000).slideUp(300, function () {
    $(this).remove();
});

function showAlert(status, message, position = 'top') {
    var alertHtml = `
        <div class="alert alert-block alert-${status} fade in">
            <button type="button" class="close" data-dismiss="alert">×</button>
            <strong>${status === "success" ? "Success!" : status === "error" ? "Error!" : "Information!"}</strong> ${message}
        </div>
    `;

    var $alert = $(alertHtml);
    
    if (position === 'top') {
        // prepend is used to add a new notification at the top
        $("#alert-container").prepend($alert);
    } else if (position === 'bottom') {
        $("#cart-table").after($alert);
    }

    $alert.hide().slideDown(300).delay(5000).slideUp(300, function () {
        $(this).remove();
    });
}


