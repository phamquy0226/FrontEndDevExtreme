
    var selectedDepartments = [];
    var selectedUsers = [];

    // Thêm phòng ban vào danh sách (Bootstrap 5)
    function addDepartment() {
        var selected = $("#SelectedDepartmentID").val();
        var text = $("#SelectedDepartmentID option:selected").text();

        if (selected && !selectedDepartments.includes(selected)) {
            selectedDepartments.push(selected);

            $("#department-list").append(`
                <li data-id="${selected}" class="list-group-item d-flex justify-content-between align-items-center list-group-item-action">
                    <div>
                        <i class="bi bi-building-fill me-2 text-primary"></i>
                        ${text}
                    </div>
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeDepartment('${selected}')">
                        <i class="bi bi-trash"></i>
                    </button>
                    <input type="hidden" name="DepartmentIDs" value="${selected}" />
                </li>
            `);

            // Animate and reset select
            $("#department-list li:last-child").hide().fadeIn(300);
            $("#SelectedDepartmentID").val("");
        }
    }

    // Xóa phòng ban với animation fade out
    function removeDepartment(id) {
        selectedDepartments = selectedDepartments.filter(x => x !== id);
        var $item = $(`#department-list li[data-id='${id}']`);
        $item.fadeOut(300, function () {
            $(this).remove();
        });
    }

    // Thêm người dùng vào danh sách (Bootstrap 5)
    function addUser() {
        var selected = $("#SelectedUserID").val();
        var text = $("#SelectedUserID option:selected").text();

        if (selected && !selectedUsers.includes(selected)) {
            selectedUsers.push(selected);

            $("#user-list").append(`
                <li data-id="${selected}" class="list-group-item d-flex justify-content-between align-items-center list-group-item-action">
                    <div>
                        <i class="bi bi-person-fill me-2 text-primary"></i>
                        ${text}
                    </div>
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeUser('${selected}')">
                        <i class="bi bi-trash"></i>
                    </button>
                    <input type="hidden" name="UserIDs" value="${selected}" />
                </li>
            `);

            // Animate and reset select
            $("#user-list li:last-child").hide().fadeIn(300);
            $("#SelectedUserID").val("");
        }
    }

    // Xóa người dùng với animation fade out
    function removeUser(id) {
        selectedUsers = selectedUsers.filter(x => x !== id);
        var $item = $(`#user-list li[data-id='${id}']`);
        $item.fadeOut(300, function () {
            $(this).remove();
        });
    }

    function updateStatusByProgress() {
        var progress = parseInt($("input[name='Progress']").val());

        // Update progress bar
        $(".progress-bar").css("width", (isNaN(progress) ? 0 : progress) + "%");

        // Update color based on progress
        if (isNaN(progress) || progress <= 0) {
            $(".progress-bar").removeClass("bg-warning bg-success").addClass("bg-secondary");
            $("input[name='Status'][value='0']").prop("checked", true); // Chưa thực hiện
        } else if (progress >= 100) {
            $(".progress-bar").removeClass("bg-warning bg-secondary").addClass("bg-success");
            $("input[name='Status'][value='2']").prop("checked", true); // Đã xong
        } else {
            $(".progress-bar").removeClass("bg-success bg-secondary").addClass("bg-warning");
            $("input[name='Status'][value='1']").prop("checked", true); // Đang thực hiện
        }
    }

    function updateProgressByStatus() {
        var status = $("input[name='Status']:checked").val();
        var $progress = $("input[name='Progress']");

        if (status === "0") {
            $progress.val(0).prop("readonly", false);
            $(".progress-bar").removeClass("bg-warning bg-success").addClass("bg-secondary");
        } else if (status === "1") {
            if (parseInt($progress.val()) === 0 || parseInt($progress.val()) === 100) {
                $progress.val(50);
            }
            $progress.prop("readonly", false);
            $(".progress-bar").removeClass("bg-success bg-secondary").addClass("bg-warning");
        } else if (status === "2") {
            $progress.val(100).prop("readonly", true);
            $(".progress-bar").removeClass("bg-warning bg-secondary").addClass("bg-success");
        }

        // Update progress bar
        $(".progress-bar").css("width", $progress.val() + "%");
    }

    function displayValidationErrors(errors) {
        $(".field-validation-error").text(""); // Clear cũ
        for (const key in errors) {
            const msg = errors[key].join(", ");
            const $el = $(`[data-valmsg-for="${key}"]`);
            if ($el.length) {
                $el.text(msg);
            }
        }
    }

    function initWorkItemCreate() {
        selectedDepartments = [];
        selectedUsers = [];
        $("#department-list").empty();
        $("#user-list").empty();

        // Set ngày mặc định là ngày hôm nay nếu chưa có giá trị
        var today = new Date().toISOString().split('T')[0]; // yyyy-MM-dd

        var $startDate = $("input[name='StartDate']");
        var $endDate = $("input[name='EndDate']");

        if (!$startDate.val()) {
            $startDate.val(today);
        }
        if (!$endDate.val()) {
            $endDate.val(today);
        }

        $("input[name='Progress']").off('input').on('input', function() {
            updateStatusByProgress();
            // Update progress bar in real-time
            var progress = parseInt($(this).val()) || 0;
            $(".progress-bar").css("width", progress + "%").attr("aria-valuenow", progress);
        });

        $("input[name='Status']").off('change').on('change', updateProgressByStatus);

        updateProgressByStatus();
        updateStatusByProgress();

        // Initialize tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    function submitCreateForm($form) {
        if ($form.data("submitting")) return;
        $form.data("submitting", true);

        // Show loading indicator
        var $submitBtn = $form.find('button[type="submit"]');
        var originalBtnHtml = $submitBtn.html();
        $submitBtn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang xử lý...');
        $submitBtn.prop('disabled', true);

        $.ajax({
            url: $form.attr("action") || "/WorkItem/Create",
            method: "POST",
            data: $form.serialize(),
            success: function (response) {
                if (response.success) {
                    DevExpress.ui.notify({
                        message: "Tạo công việc thành công",
                        type: "success",
                        displayTime: 2000,
                        position: { at: "top center", my: "top center" }
                    });
                    if (response.redirectUrl) {
                        setTimeout(() => window.location.href = response.redirectUrl, 1000);
                    }
                } else {
                    DevExpress.ui.notify({
                        message: response.message || "Tạo công việc thất bại",
                        type: "error",
                        displayTime: 3000,
                        position: { at: "top center", my: "top center" }
                    });
                    if (response.errors) {
                        displayValidationErrors(response.errors);
                    }
                }
            },
            error: function () {
                DevExpress.ui.notify({
                    message: "Lỗi kết nối đến server",
                    type: "error",
                    displayTime: 3000,
                    position: { at: "top center", my: "top center" }
                });
            },
            complete: function () {
                $form.data("submitting", false);
                $submitBtn.html(originalBtnHtml);
                $submitBtn.prop('disabled', false);
            }
        });
    }

    $(document).ready(function () {
        // Enable Bootstrap validation styles
        'use strict';
        window.addEventListener('load', function() {
            var forms = document.getElementsByClassName('needs-validation');
            var validation = Array.prototype.filter.call(forms, function(form) {
                form.addEventListener('submit', function(event) {
                    if (form.checkValidity() === false) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                    form.classList.add('was-validated');
                }, false);
            });
        }, false);

        // Submit cho popup form (nếu dùng popup)
        $("#formCreateWorkItemPop").off("submit").on("submit", function (e) {
            e.preventDefault();
            submitCreateForm($(this));
        });

        initWorkItemCreate();
    });
function closeWorkItemPopup() {
    $("#popup").dxPopup("hide");
}
