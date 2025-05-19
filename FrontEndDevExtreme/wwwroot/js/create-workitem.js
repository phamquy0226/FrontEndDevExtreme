var selectedDepartments = [];
var selectedUsers = [];

// Thêm phòng ban vào danh sách (đã chỉnh nút xóa Bootstrap 5)
function addDepartment() {
    var selected = $("#SelectedDepartmentID").val();
    var text = $("#SelectedDepartmentID option:selected").text();

    if (selected && !selectedDepartments.includes(selected)) {
        selectedDepartments.push(selected);

        $("#department-list").append(`
            <li data-id="${selected}" class="list-group-item d-flex justify-content-between align-items-center">
                ${text}
                <button type="button" class="btn-close" aria-label="Close" onclick="removeDepartment('${selected}')"></button>
                <input type="hidden" name="DepartmentIDs" value="${selected}" />
            </li>
        `);
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

// Thêm người dùng vào danh sách (đã chỉnh nút xóa Bootstrap 5)
function addUser() {
    var selected = $("#SelectedUserID").val();
    var text = $("#SelectedUserID option:selected").text();

    if (selected && !selectedUsers.includes(selected)) {
        selectedUsers.push(selected);

        $("#user-list").append(`
            <li data-id="${selected}" class="list-group-item d-flex justify-content-between align-items-center">
                ${text}
                <button type="button" class="btn-close" aria-label="Close" onclick="removeUser('${selected}')"></button>
                <input type="hidden" name="UserIDs" value="${selected}" />
            </li>
        `);
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
    if (isNaN(progress) || progress <= 0) {
        $("input[name='Status'][value='0']").prop("checked", true); // Chưa thực hiện
    } else if (progress >= 100) {
        $("input[name='Status'][value='2']").prop("checked", true); // Đã xong
    } else {
        $("input[name='Status'][value='1']").prop("checked", true); // Đang thực hiện
    }
}

function updateProgressByStatus() {
    var status = $("input[name='Status']:checked").val();
    var $progress = $("input[name='Progress']");

    if (status === "0") {
        $progress.val(0).prop("readonly", false);
    } else if (status === "1") {
        $progress.prop("readonly", false);
    } else if (status === "2") {
        $progress.val(100).prop("readonly", true);
    }
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

    $("input[name='Progress']").off('input').on('input', updateStatusByProgress);
    $("input[name='Status']").off('change').on('change', updateProgressByStatus);

    updateProgressByStatus();
    updateStatusByProgress();
}

function submitCreateForm($form) {
    if ($form.data("submitting")) return;
    $form.data("submitting", true);

    $.ajax({
        url: $form.attr("action") || "/WorkItem/Create",
        method: "POST",
        data: $form.serialize(),
        success: function (response) {
            if (response.success) {
                DevExpress.ui.notify("Tạo công việc thành công", "success", 2000);
                if (response.redirectUrl) {
                    setTimeout(() => window.location.href = response.redirectUrl, 1000);
                }
            } else {
                DevExpress.ui.notify(response.message || "Tạo công việc thất bại", "error", 3000);
                if (response.errors) {
                    displayValidationErrors(response.errors);
                }
            }
        },
        error: function () {
            DevExpress.ui.notify("Lỗi kết nối đến server", "error", 3000);
        },
        complete: function () {
            $form.data("submitting", false);
        }
    });
}

$(document).ready(function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })

    // Submit cho popup form (nếu dùng popup)
    $("#formCreateWorkItemPop").off("submit").on("submit", function (e) {
        e.preventDefault();
        submitCreateForm($(this));
    });

    initWorkItemCreate();
});