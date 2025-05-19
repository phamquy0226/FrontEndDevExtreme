function initNotePopup() {
    console.log("initNotePopup called");

    $("#btnAddNotePop").off("click").on("click", addNotePop);
    $("#noteContentPop").dxTextArea("instance").option("value", "");
}

function refreshNoteList() {
    const container = $("#popupNoteContent");
    container.html("<div class='text-muted'>Đang tải...</div>");

    if (popupNote._workItemId) {
        $.get(notePartialUrl, { workItemId: popupNote._workItemId }, function (html) {
            container.html(html);
            initNotePopup(); // Phải gọi lại sau khi load partial
        }).fail(function () {
            container.html("<div class='text-danger'>Không thể tải ghi chú.</div>");
        });
    } else {
        container.html("<div class='text-warning'>Không có công việc được chọn.</div>");
    }
}

function addNotePop() {
    const $textArea = $("#noteContentPop").dxTextArea("instance");
    const content = $textArea.option("value")?.trim();

    if (!content) {
        DevExpress.ui.notify("Vui lòng nhập nội dung ghi chú", "warning", 2000);
        return;
    }

    $textArea.option("disabled", true);

    $.ajax({
        url: addNoteUrl,
        method: "POST",
        data: {
            workItemId: popupNote._workItemId,
            content: content
        },
        success: function (res) {
            if (res.success) {
                DevExpress.ui.notify("Đã thêm ghi chú", "success", 2000);
                $textArea.option("value", "");
                refreshNoteList(); // gọi lại để load ghi chú mới
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


$(document).on("click", ".btn-delete-note", function () {
    const noteId = $(this).data("note-id");
    const workItemId = $(this).data("work-item-id");

    if (confirm("Bạn có chắc muốn xoá ghi chú này?")) {
        $.post("/Note/DeleteNote", { noteId, workItemId }, function (res) {
            if (res.success) {
                DevExpress.ui.notify("Đã xoá ghi chú", "success", 2000);
                refreshNoteList(); // Gọi lại để load ghi chú mới
            } else {
                DevExpress.ui.notify(res.message || "Xoá ghi chú thất bại", "error", 2000);
            }
        }).fail(() => {
            DevExpress.ui.notify("Lỗi khi gọi API xoá", "error", 2000);
        });
    }
});
