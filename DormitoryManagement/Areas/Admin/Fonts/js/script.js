
    $(document).ready(function () {
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

        $('.building-item').click(function () {
            var id = $(this).data('building-id');
            window.location.href = '/Admin/Homes/Floor?buildingId=' + id;
        });
    });

