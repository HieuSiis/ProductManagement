
$(function () {
    // Catch click events on tabs
    $('#myTab a').on('click', function (e) {
        e.preventDefault();

        // Remove the `btn-primary` class from all buttons
        $('#myTab a span').removeClass('btn-primary');

        // Add class `btn-primary` to the pressed button
        $(this).find('span').addClass('btn-primary');
    });
});

function goBack() {
    if (document.referrer) {
        window.location.href = document.referrer;
    } else {
        window.location.href = "/";
    }
}