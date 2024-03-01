$(document).ready(function () {
    $("#NewPassword").on("input", function () {
        updatePasswordStrength();
    });

    $("#passwordStrengthText").on("mouseenter", function () {
        showPasswordRequirements();
    });

    $("#passwordStrengthText").on("mouseleave", function () {
        hidePasswordRequirements();
    });

    function showPasswordRequirements() {
        // Hiển thị yêu cầu mật khẩu khi người dùng di chuột vào phần text
        $("#passwordRequirements").css("display", "block");
    }

    function hidePasswordRequirements() {
        // Hiển thị yêu cầu mật khẩu khi người dùng di chuột vào phần text
        $("#passwordRequirements").css("display", "none");
    }

    function updatePasswordStrength() {
        var password = $("#NewPassword").val();
        var strength = calculatePasswordStrength(password);

        // Cập nhật thanh mật khẩu mạnh/yếu và mô tả
        $("#passwordStrengthBar").css("width", strength + "%");
        $("#passwordStrengthText").text(getStrengthText(strength));

        // Cập nhật màu sắc của thanh progress
        updateProgressBarColor(strength);
    }

    function calculatePasswordStrength(password) {
        var lowercaseRegex = /[a-z]/;
        var uppercaseRegex = /[A-Z]/;
        var digitRegex = /\d/;
        var specialCharRegex = /[!#\$%\^&\*\(\)_\+\-=\[\]\{\};:'",<>\./?\\|`~]/;

        // Đếm số loại ký tự xuất hiện trong mật khẩu
        var charTypesCount = 0;
        if (lowercaseRegex.test(password)) charTypesCount++;
        if (uppercaseRegex.test(password)) charTypesCount++;
        if (digitRegex.test(password)) charTypesCount++;
        if (specialCharRegex.test(password)) charTypesCount++;

        // Kiểm tra độ dài mật khẩu
        var lengthStrength = (password.length >= 8 && password.length <= 15) ? 50 : 0;

        // Tính toán mức độ mạnh dựa trên số loại ký tự và chiều dài mật khẩu
        var strength = Math.min(password.length * charTypesCount, 100) + lengthStrength;
        return strength;
    }

    function getStrengthText(strength) {
        // Trả về mô tả dựa trên mức độ mạnh của mật khẩu
        if (strength >= 80) {
            return "Strong";
        } else if (strength >= 40) {
            return "Moderate";
        } else {
            return "Weak";
        }
    }

    function updateProgressBarColor(strength) {
        var progressBar = $("#passwordStrengthBar");

        if (strength < 40) {
            progressBar.removeClass("bg-warning bg-success").addClass("bg-danger");
        } else if (strength < 80) {
            progressBar.removeClass("bg-danger bg-success").addClass("bg-warning");
        } else {
            progressBar.removeClass("bg-danger bg-warning").addClass("bg-success");
        }
    }

    $('.delete-btn').click(function () {
        var id = $(this).data('id');
        var name = $(this).attr('name');
        $('.modal-body').text("Bạn có chắc chắn muốn xóa "+name + " này không ?");
        $('#confirmDelete').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDelete').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Homes/Delete/' + id;
    });

    $('.btn.btn-danger.btn-sm').click(function () {
        var id = $(this).data('id');
        var name = $(this).attr('name');
        $('.modal-body').text("Bạn có chắc chắn muốn xóa " + name + " này không ?");
        $('#confirmDeleteRoomType').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });


    $('#confirmDeleteRoomType').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Room/Delete/' + id;
    });

    $('.btn.btn-danger.btn-sm').click(function () {
        var id = $(this).data('id');
        var name = $(this).attr('name');
        $('.modal-body').text("Bạn có chắc chắn muốn xóa " + name + " này không ?");
        $('#confirmDeleteStudent').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDeleteStudent').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Student/Delete/' + id;
    });

    $('.btn.btn-danger.btn-sm.btnFloor').click(function () {
        var id = $(this).data('id');
        var name = $(this).attr('name');
        $('.modal-body').text("Bạn có chắc chắn muốn xóa phòng "+ name +" này không ?");
        $('#confirmDeleteRoom').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDeleteRoom').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Homes/DeleteRoom/' + id;
    });

    $('.building-item').click(function () {
        var id = $(this).data('building-id');
        window.location.href = '/Admin/Homes/Floor?buildingId=' + id;
    });


    var maxCapacity = parseInt($("#NumberOfBeds").val());
    var length = $('.student-input').length;
    var s = maxCapacity - length;
    console.log(s)
            $.ajax({
                url: '/Admin/Homes/GetStudents', // Địa chỉ URL của phương thức GetStudents
                method: 'GET', // Phương thức GET để gửi yêu cầu
                success: function (response) {

                    const data = response.map(item => ({
                        id: `${item.id}`,
                        name: `${item.name}`,
                        text: `${item.id}|${item.name}`
                    }));
                    // Khởi tạo Select2 với dữ liệu nhận được
                    if (s > 0) {
                        $(".select2").select2({
                            placeholder: "Tìm kiếm sinh viên",
                            maximumSelectionLength: s,
                            multiple: true,
                            data: data, // Dữ liệu nhận được từ máy chủ
                            templateResult: function (data) {
                                // Sử dụng template để hiển thị tên và mã số sinh viên
                                return $('<span>' + data.id + " - " + data.name + '</span>');
                            },
                            templateSelection: function (data) {
                                // Sử dụng template để hiển thị tên và mã số sinh viên khi lựa chọn được chọn
                                return $('<span>' + data.id + " - " + data.name + '</span>');
                            },

                        });
                    } else {
                        $(".select2").hide();
                        $(".select2").prop('disabled', true);
                    }

                },
                error: function (xhr, status, error) {
                    alert(error); // Xử lý lỗi nếu có
                }
            });

    $('.delete-student').click(function () {
        // Loại bỏ phần tử cha của nút xóa sinh viên (div có class là col-md-2)
        var id = $(this).data('id');
        var name = $(this).attr('name');
        var room = $("#RoomName").val();
        $('.modal-body').text("Bạn có chắc chắn muốn xóa sinh viên " + name + " ra khỏi phòng " + room + " này không ?");
        $('#confirmDeleteRoomStudent').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDeleteRoomStudent').click(function () {
        var id = $(this).data('id');
        window.location.href = '/Homes/DeleteRoomStudent/' + id;
    });

    $('.logout').click(function () {
        // Loại bỏ phần tử cha của nút xóa sinh viên (div có class là col-md-2)
        $('.modal-body').text("Select Logout below if you are ready to end your current session.");
    });


});


