$(document).ready(function () {
    $('#fromYear').on('keydown', function (e) {
        // Chỉ cho phép nhập số và phím backspace/delete
        if (!((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105) || e.keyCode == 8 || e.keyCode == 46)) {
            e.preventDefault();
        }

        // Giới hạn chiều dài của giá trị nhập vào là 4 ký tự
        if ($(this).val().length >= 4 && e.keyCode !== 8 && e.keyCode !== 46) {
            e.preventDefault();
        }
    });

    $('#toYear').on('keydown', function (e) {
        // Chỉ cho phép nhập số và phím backspace/delete
        if (!((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105) || e.keyCode == 8 || e.keyCode == 46)) {
            e.preventDefault();
        }

        // Giới hạn chiều dài của giá trị nhập vào là 4 ký tự
        if ($(this).val().length >= 4 && e.keyCode !== 8 && e.keyCode !== 46) {
            e.preventDefault();
        }
    });

    $("button.btn-primary").click(function () {
        var fromMonth = $("#fromMonth").val();
        var toMonth = $("#toMonth").val();
        var fromYear = $("#fromYear").val();
        var toYear = $("#toYear").val();

        // Lặp qua từng hàng trong bảng
        $("table#dataTable tbody tr").each(function () {
            var monthYear = $(this).find("td:nth-child(3)").text(); // Lấy giá trị tháng/năm từ cột thứ 3 (thứ tự là 0-based)
            var year = monthYear.split("/")[1];
            var month = monthYear.split("/")[0];
            // Kiểm tra xem tháng/năm của hàng có nằm trong khoảng từ tháng/năm đến tháng/năm không
            if ((year >= fromYear && year <= toYear) && (month >= fromMonth && month <= toMonth)) {
                $(this).show(); // Hiển thị hàng nếu điều kiện lọc được áp dụng
            } else {
                $(this).hide(); // Ẩn hàng nếu không phù hợp với điều kiện lọc
            }
        });
    });

    // Xử lý sự kiện khi click vào nút Reset
    $("#resetButton").click(function () {
        $('#fromYear').val('');
        $('#toYear').val('');
        $('#fromMonth').val('--Chọn tháng--');
        $('#toMonth').val('--Chọn tháng--');
        $("table#dataTable tbody tr").show(); // Hiển thị lại tất cả các hàng
    });

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

});