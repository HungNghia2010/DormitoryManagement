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
        $('#confirmDelete').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDelete').click(function () {
        var id = $(this).data('id');
        // Gửi yêu cầu xóa bản ghi có id là id thông qua Ajax hoặc form post
        // Sau khi xóa thành công, bạn có thể thực hiện chuyển hướng hoặc làm gì đó khác
        // ở đây là một ví dụ:
        window.location.href = '/Homes/Delete/' + id;
    });

    $('.btn.btn-danger.btn-sm').click(function () {
        var id = $(this).data('id');
        $('#confirmDeleteRoomType').data('id', id); // Lưu id của bản ghi cần xóa vào nút xác nhận xóa
    });

    $('#confirmDeleteRoomType').click(function () {
        var id = $(this).data('id');
        console.log(id);
        // Gửi yêu cầu xóa bản ghi có id là id thông qua Ajax hoặc form post
        // Sau khi xóa thành công, bạn có thể thực hiện chuyển hướng hoặc làm gì đó khác
        // ở đây là một ví dụ:
        window.location.href = '/Room/Delete/' + id;
    });

    $('.building-item').click(function () {
        var id = $(this).data('building-id');
        window.location.href = '/Admin/Homes/Floor?buildingId=' + id;
    });

});
