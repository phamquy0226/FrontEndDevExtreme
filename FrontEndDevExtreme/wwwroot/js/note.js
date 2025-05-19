function addNote() {
    const $textArea = $("#noteContent").dxTextArea("instance");
    const content = $textArea.option("value")?.trim();

    if (!content) {
        DevExpress.ui.notify("Vui lòng nhập nội dung ghi chú", "warning", 2000);
        return;
    }

    $textArea.option("disabled", true);

    $.ajax({
        url: '@Url.Action("AddNote", "WorkItem")',
        method: "POST",
        data: {
            workItemId: @workItemId,
            content: content
        },
        success: function (res) {
            if (res.success) {
                DevExpress.ui.notify("Đã thêm ghi chú", "success", 2000);
                window.location.reload();
            } else {
                DevExpress.ui.notify(res.message || "Thêm ghi chú thất bại", "error", 2000);
            }
        },
        error: function () {
            DevExpress.ui.notify("Lỗi khi gọi API", "error", 2000);
        },
        complete: function () {
            $textArea.option("disabled", false);
        }
    });
}

function initNotePopup() {
    // Ví dụ: gán sự kiện click cho nút "Thêm ghi chú"
    $("#btnAddNote").off("click").on("click", function () {
        addNote();
    });

    // Nếu cần thiết, bạn cũng có thể reset textarea, hoặc làm gì đó khi mở popup
    $("#noteContent").val("");
}
