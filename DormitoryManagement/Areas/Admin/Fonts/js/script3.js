$(document).ready(function () {
    

    $('.btn.btn-danger.btn-sm.btnstudentfee').click(function () {
        var id = $(this).data('id');
        var name = $(this).attr('name');
        $('.modal-body').text("Bạn có chắc chắn muốn xóa hóa đơn số " + name + " này không ?");
        $('#confirmDeleteFeeStudent').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });


    $('#confirmDeleteFeeStudent').click(function () {
        var id = $(this).data('id');
        window.location.href = '/TuitionFee/DeleteStudentFee/' + id;
    });

    $('.btnstudentfee').click(function () {
        var id = $(this).data('id');
        $('#payWithVNPay').data('id', id);
        $('#payWithMomo').data('id', id);
    });

    $('#payWithVNPay').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Payment/PayModal/' + id;
    });

    $('#payWithMomo').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Payment/PayMomo/' + id;
    });


    $('.btn.btn-danger.btn-sm.btndevice').click(function () {
        var id = $(this).data('id');
        $('#confirmDeleteDevice').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });


    $('#confirmDeleteDevice').click(function () {
        var id = $(this).data('id');
        window.location.href = '/DeviceReport/DeleteDevice/' + id;
    });


    $('.btn.btn-danger.btn-sm.btnBlog').click(function () {
        var id = $(this).data('id');
        $('#confirmDeleteBlog').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });


    $('#confirmDeleteBlog').click(function () {
        var id = $(this).data('id');
        window.location.href = '/BlogAdmin/DeleteBlog/' + id;
    });

});