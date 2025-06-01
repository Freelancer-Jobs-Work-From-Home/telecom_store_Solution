// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Thêm sản phẩm vào giỏ hàng
    $(".add-to-cart").click(function (e) {
        e.preventDefault();
        let productId = $(this).data("id");

        $.ajax({
            url: "/Cart/AddToCart",
            type: "POST",
            data: { id: productId },
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            success: function (response) {
                if (response.success) {
                    $("#cart-count").text(response.cartCount);
                    $(".mini_cart").html(response.cartHTML);

                    // Cập nhật modal
                    $("#cartModal .modal-title").text("Thêm vào giỏ hàng");
                    $("#cartModal .modal-body").html("Sản phẩm đã được thêm vào giỏ hàng thành công! 🛒");

                    // Hiển thị modal
                    $("#cartModal").modal("show");

                    // Cập nhật tổng tiền
                    updateTotalPrice();
                } else {
                    // Nếu lỗi (ví dụ: hết hàng, vượt số lượng tồn kho)
                    $("#cartModal .modal-title").text("Không thể thêm sản phẩm");
                    $("#cartModal .modal-body").html(response.message || "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.");

                    $("#cartModal").modal("show");
                }
            },
            error: function (xhr) {
                handleAjaxError(xhr, "Bạn cần phải đăng nhập để thực hiện chức năng này");
            }
        });
    });


    // Xóa sản phẩm khỏi giỏ hàng
    $(document).on("click", ".remove-from-cart", function (e) {
        e.preventDefault();
        let productId = $(this).data("id");

        $.ajax({
            url: "/Cart/RemoveFromCart",
            type: "POST",
            data: { id: productId },
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            success: function (response) {
                $("#cart-count").text(response.cartCount);
                $(".mini_cart").html(response.cartHTML);

                // Cập nhật modal
                $("#cartModal .modal-title").text("Xóa khỏi giỏ hàng");
                $("#cartModal .modal-body").html("Sản phẩm đã được xóa khỏi giỏ hàng! 🗑️");
                $("#cartModal").modal("show");

                // Cập nhật tổng tiền
                updateTotalPrice();
            },
            error: function (xhr) {
                handleAjaxError(xhr, "Bạn cần phải đăng nhập để thực hiện chức năng này");
            }
        });
    });

    // Cập nhật số lượng sản phẩm trong giỏ hàng
    $(document).on("change", ".update-quantity", function () {
        let productId = $(this).data("id");
        let newQuantity = $(this).val();
        let maxQuantity = parseInt($(this).data("max"));

        if (isNaN(newQuantity) || newQuantity < 1) {
            alert("Số lượng phải lớn hơn 0!");
            $(this).val(1);
            return;
        }

        if (newQuantity > maxQuantity) {
            alert("Số lượng vượt quá hàng trong kho! (Chỉ còn " + maxQuantity + ")");
            $(this).val(maxQuantity); // Reset lại số tối đa
            return;
        }

        $.ajax({
            url: "/Cart/UpdateQuantity",
            type: "POST",
            data: { id: productId, quantity: newQuantity },
            success: function (response) {
                location.reload(); // Reload lại trang để cập nhật giá tổng
            },
            error: function () {
                alert("Lỗi khi cập nhật số lượng!");
            }
        });
    });

    // Áp dụng mã giảm giá
    $("#applyDiscount").click(function () {
        let discountCode = $("#discountInput").val().trim(); // Xóa khoảng trắng
        $("#discountMessage").removeClass("text-danger text-success").text(""); // Reset thông báo

        if (!discountCode) {
            updateTotalPrice(); 
            return;
        }

        $.ajax({
            url: "/Cart/ApplyDiscount",
            type: "GET",
            data: { code: discountCode },
            success: function (response) {
                if (response.success) {
                    $("#discountMessage").text("Mã giảm giá hợp lệ!").addClass("text-success");
                    $("#totalPrice").text(response.newTotal + " VND");
                    $("#discountCode").val(discountCode);
                } else {
                    $("#discountMessage").text("Mã giảm giá không hợp lệ!").addClass("text-danger");
                }
            },
            error: function () {
                alert("Lỗi khi áp dụng mã giảm giá!");
            }
        });
    });

    // Cập nhật tổng tiền (không bắt buộc nhập mã giảm giá)
    function updateTotalPrice() {
        let discountCode = $("#discountCode").val() || ""; // Nếu không có mã giảm giá, gửi chuỗi rỗng

        $.ajax({
            url: "/Cart/UpdateTotal",
            type: "GET",
            data: { discountCode: discountCode },
            success: function (response) {
                $("#totalPrice").text(response.newTotal + " VND");
            },
            error: function () {
                alert("Lỗi khi cập nhật tổng tiền!");
            }
        });
    }
    // Xóa mã giảm giá
    $("#removeDiscount").click(function () {
        $.ajax({
            url: "/Cart/RemoveDiscount",
            type: "POST",
            success: function (response) {
                if (response.success) {
                    // Cập nhật lại giá trị tổng tiền từ session
                    $("#totalPrice").text(response.newTotal + " VND");
                    $("#discountMessage").text("Mã giảm giá đã được xóa.").removeClass("text-success").addClass("text-danger");
                }
            },
            error: function () {
                alert("Lỗi khi xóa mã giảm giá!");
            }
        });
    });

    function handleAjaxError(xhr, defaultMsg) {
        if (xhr.status === 401 || xhr.status === 403) {
            alert("Bạn cần đăng nhập để thực hiện hành động này.");
            window.location.href = "/Auth/Login"; // Đổi lại link login nếu khác
        } else {
            alert(defaultMsg);
        }
    }

});






