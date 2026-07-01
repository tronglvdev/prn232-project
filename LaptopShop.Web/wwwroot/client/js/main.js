(function ($) {
  "use strict";

  // 1. Spinner xử lý lúc load trang
  var spinner = function () {
    setTimeout(function () {
      if ($("#spinner").length > 0) {
        $("#spinner").removeClass("show");
      }
    }, 1);
  };
  spinner(0);

  // 2. Fixed Navbar khi cuộn trang
  $(window).scroll(function () {
    if ($(window).width() < 992) {
      if ($(this).scrollTop() > 55) {
        $(".fixed-top").addClass("shadow");
      } else {
        $(".fixed-top").removeClass("shadow");
      }
    } else {
      if ($(this).scrollTop() > 55) {
        $(".fixed-top").addClass("shadow").css("top", 0);
      } else {
        $(".fixed-top").removeClass("shadow").css("top", 0);
      }
    }
  });

  // 3. Nút Back to top
  $(window).scroll(function () {
    if ($(this).scrollTop() > 300) {
      $(".back-to-top").fadeIn("slow");
    } else {
      $(".back-to-top").fadeOut("slow");
    }
  });
  $(".back-to-top").click(function () {
    $("html, body").animate({ scrollTop: 0 }, 1500, "easeInOutExpo");
    return false;
  });

  // 4. Xử lý tăng giảm số lượng trong giỏ hàng (Trang Cart)
  $(".quantity button").on("click", function () {
    const button = $(this);
    const input = button.parent().parent().find("input");
    const oldValue = parseFloat(input.val());
    const id = input.attr("data-cart-detail-id");
    const price = parseFloat(input.attr("data-cart-detail-price"));

    let newVal;
    if (button.hasClass("btn-plus")) {
      newVal = oldValue + 1;
    } else {
      newVal = oldValue > 1 ? oldValue - 1 : 1;
    }

    input.val(newVal);
    $(`#quantity-${id}`).val(newVal);

    const priceElement = $(`p[data-cart-detail-id='${id}']`);
    if (priceElement.length) {
      const newRowPrice = price * newVal;
      priceElement.text(formatCurrency(newRowPrice) + " đ");
    }
    updateCartTotal();
  });

  $('input[name="cartDetailIds"]').on("change", function () {
    updateCartTotal();
  });

  // 5. Cập nhật tổng tiền giỏ hàng
  function updateCartTotal() {
    let total = 0;
    const totalPriceElements = $(`p[data-cart-total-price]`);

    $('input[name="cartDetailIds"]:checked').each(function () {
      const row = $(this).closest("tr");
      const inputQuantity = row.find(".quantity input");
      const quantity = parseFloat(inputQuantity.val());
      const price = parseFloat(inputQuantity.attr("data-cart-detail-price"));
      total += price * quantity;
    });

    totalPriceElements.each(function () {
      $(this).text(formatCurrency(total) + " đ");
      $(this).attr("data-cart-total-price", total);
    });
  }

  // Kiểm tra trước khi Checkout
  $('form[action="/confirm-checkout"]').on("submit", function (e) {
    const checkedCount = $('input[name="cartDetailIds"]:checked').length;
    if (checkedCount === 0) {
      alert("Vui lòng chọn ít nhất một sản phẩm để tiếp tục thanh toán!");
      e.preventDefault();
      return false;
    }
  });

  // Định dạng tiền tệ VNĐ
  function formatCurrency(value) {
    const formatter = new Intl.NumberFormat("vi-VN", {
      style: "decimal",
      minimumFractionDigits: 0,
    });
    return formatter.format(value).replace(/\./g, ",");
  }

  // 6. XỬ LÝ LỌC REALTIME
  function handleFilter() {
    let factoryArr = [];
    let targetArr = [];
    let priceArr = [];

    $("#factoryFilter .form-check-input:checked").each(function () {
      factoryArr.push($(this).val());
    });

    $("#targetFilter .form-check-input:checked").each(function () {
      targetArr.push($(this).val());
    });

    $("#priceFilter .form-check-input:checked").each(function () {
      priceArr.push($(this).val());
    });

    let sortValue = $('input[name="radio-sort"]:checked').val();

    const currentUrl = new URL(window.location.href);
    const searchParams = currentUrl.searchParams;

    searchParams.set("page", "1");
    searchParams.set("sort", sortValue);

    if (factoryArr.length > 0)
      searchParams.set("factory", factoryArr.join(","));
    else searchParams.delete("factory");

    if (targetArr.length > 0) searchParams.set("target", targetArr.join(","));
    else searchParams.delete("target");

    if (priceArr.length > 0) searchParams.set("price", priceArr.join(","));
    else searchParams.delete("price");

    window.location.href = currentUrl.toString();
  }

  // Kiểm tra trạng thái đăng nhập
  function isLogin() {
    const navElement = $("#navbarCollapse");
    const childLogin = navElement.find("a.a-login");
    if (childLogin.length > 0) {
      return false;
    }
    return true;
  }

  $(document).ready(function () {
    updateCartTotal();

    // Active Navbar link
    const navElement = $("#navbarCollapse");
    const currentPath = window.location.pathname;
    navElement.find("a.nav-link").each(function () {
      const link = $(this);
      if (link.attr("href") === currentPath) {
        link.addClass("active");
      } else {
        link.removeClass("active");
      }
    });

    // Lắng nghe sự kiện change REALTIME cho bộ lọc
    $("#factoryFilter, #targetFilter, #priceFilter").on(
      "change",
      ".form-check-input",
      function () {
        handleFilter();
      },
    );

    $('input[name="radio-sort"]').on("change", function () {
      handleFilter();
    });

    $("#btnFilter").click(function (event) {
      event.preventDefault();
      handleFilter();
    });

    // Tự động tích lại checkbox dựa trên URL khi load trang
    const params = new URLSearchParams(window.location.search);
    const syncCheckboxes = (paramName, containerId) => {
      if (params.has(paramName)) {
        const values = params.get(paramName).split(",");
        values.forEach((val) => {
          $(`${containerId} .form-check-input[value="${val}"]`).prop(
            "checked",
            true,
          );
        });
      }
    };

    syncCheckboxes("factory", "#factoryFilter");
    syncCheckboxes("target", "#targetFilter");
    syncCheckboxes("price", "#priceFilter");

    if (params.has("sort")) {
      const sort = params.get("sort");
      $(`input[type="radio"][name="radio-sort"][value="${sort}"]`).prop(
        "checked",
        true,
      );
    }

    // Preview Ảnh Avatar khi chọn file
    $("#avatarInput").change(function (e) {
      const file = e.target.files[0];
      if (file) {
        const fileType = file["type"];
        const validImageTypes = [
          "image/gif",
          "image/jpeg",
          "image/png",
          "image/webp",
        ];
        if ($.inArray(fileType, validImageTypes) < 0) {
          alert("Vui lòng chọn định dạng ảnh (jpg, png, webp, gif)");
          return;
        }

        const reader = new FileReader();
        reader.onload = function (event) {
          $("#avatarPreview").attr("src", event.target.result);
        };
        reader.readAsDataURL(file);
      }
    });

    // Ẩn/Hiện mật khẩu
    $(document).on("click", ".toggle-password", function () {
      $(this).toggleClass("bi-eye bi-eye-slash");
      let input = $(this).closest(".input-group").find("input");
      if (input.attr("type") == "password") {
        input.attr("type", "text");
      } else {
        input.attr("type", "password");
      }
    });

    // Kiểm tra đổi mật khẩu hợp lệ
    $("#changePasswordForm").on("submit", function (e) {
      let newPass = $("#newPassword").val();
      let confirmPass = $("#confirmPassword").val();

      if (newPass.length < 6) {
        alert("Mật khẩu mới phải có ít nhất 6 ký tự!");
        e.preventDefault();
        return;
      }

      if (newPass !== confirmPass) {
        $("#passwordError").show();
        e.preventDefault();
      } else {
        $("#passwordError").hide();
      }
    });

    // Hàm gọi AJAX dùng chung cho cả trang chủ và trang chi tiết
    function sendAddToCartAjax(productId, quantity) {
      if (!isLogin()) {
        $.toast({
          heading: "Lỗi thao tác",
          text: "Bạn cần đăng nhập tài khoản",
          position: "top-right",
          icon: "error",
        });
        return;
      }

      const token = $("meta[name='_csrf']").attr("content");
      const header = $("meta[name='_csrf_header']").attr("content");

      $.ajax({
        url: `${window.location.origin}/api/add-product-to-cart`,
        type: "POST",
        data: JSON.stringify({ quantity: quantity, productId: productId }),
        contentType: "application/json",
        success: function (response) {
          const sum = +response;
          // Cập nhật số badge giỏ hàng trên header
          $("#sumCart").text(sum);

          $.toast({
            heading: "Giỏ hàng",
            text: "Thêm sản phẩm vào giỏ hàng thành công",
            position: "top-right",
          });
        },
        error: function (response) {
          alert("Có lỗi xảy ra");
          console.log("error: ", response);
        },
      });
    }

    // Nút Add to cart ở TRANG CHỦ
    $(document).on("click", ".btnAddToCartHomepage", function (event) {
      event.preventDefault();
      const productId = $(this).attr("data-product-id");
      const quantity = 1;
      sendAddToCartAjax(productId, quantity);
    });

    // Nút Add to cart ở TRANG CHI TIẾT
    $(document).on("click", ".btnAddToCartDetail", function (event) {
      event.preventDefault();
      const productId = $(this).attr("data-product-id");

      const quantity = parseInt($("#detailQuantity").val()) || 1;

      sendAddToCartAjax(productId, quantity);
    });

    // --- Video Modal ---
    var $videoSrc;
    $(".btn-play").click(function () {
      $videoSrc = $(this).data("src");
    });
    $("#videoModal").on("shown.bs.modal", function (e) {
      $("#video").attr(
        "src",
        $videoSrc + "?autoplay=1&amp;modestbranding=1&amp;showinfo=0",
      );
    });
    $("#videoModal").on("hide.bs.modal", function (e) {
      $("#video").attr("src", $videoSrc);
    });

    // Carousels công cụ quảng cáo
    $(".testimonial-carousel").owlCarousel({
      autoplay: true,
      smartSpeed: 2000,
      dots: true,
      loop: true,
      margin: 25,
      nav: true,
      navText: [
        '<i class="bi bi-arrow-left"></i>',
        '<i class="bi bi-arrow-right"></i>',
      ],
      responsive: { 0: { items: 1 }, 992: { items: 2 } },
    });

    $(".vegetable-carousel").owlCarousel({
      autoplay: true,
      smartSpeed: 1500,
      dots: true,
      loop: true,
      margin: 25,
      nav: true,
      navText: [
        '<i class="bi bi-arrow-left"></i>',
        '<i class="bi bi-arrow-right"></i>',
      ],
      responsive: {
        0: { items: 1 },
        768: { items: 2 },
        992: { items: 3 },
        1200: { items: 4 },
      },
    });
  });
})(jQuery);
