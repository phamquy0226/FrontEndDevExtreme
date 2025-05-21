
$(function () {
    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Create work item popup
    const popup = $("#popupCreateWorkItem").dxPopup({
        title: "Tạo công việc mới",
        width: function () {
            return window.innerWidth > 768 ? "50%" : "95%";
        },
        height: "100%",
        visible: false,
        showCloseButton: true,
        showTitle: true,
        dragEnabled: true,
        hideOnOutsideClick: true,
        position: {
            my: "top right",
            at: "top right",
            of: window
        },
        animation: {
            show: {
                type: "slide",
                duration: 300,
                from: { right: -window.innerWidth * 0.5 },
                to: { right: 0 }
            },
            hide: {
                type: "fade",
                duration: 300
            }
        },
        contentTemplate: function (contentElement) {
            contentElement.append(`
                <div class="d-flex justify-content-center align-items-center p-5" style="height:100%;">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Đang tải...</span>
                    </div>
                    <span class="ms-3">Đang tải...</span>
                </div>
            `);

            $.get(appUrls.createPopUp, function (html) {
                contentElement.html(html);
                if (typeof initWorkItemCreate === "function") {
                    initWorkItemCreate();
                }
            }).fail(function () {
                contentElement.html('<div class="alert alert-danger">Không thể tải form tạo công việc. Vui lòng thử lại.</div>');
            });
        }
    }).dxPopup("instance");

    $("#btnOpenCreatePopup, #mobileCreateBtn").on("click", function (e) {
        e.preventDefault();
        popup.show();
    });

    // Note popup
    let popupNote = $("#popupNote").dxPopup({
        title: "Ghi chú công việc",
        width: function () {
            return window.innerWidth > 768 ? "50%" : "95%";
        },
        height: "100%",
        visible: false,
        showCloseButton: true,
        showTitle: true,
        dragEnabled: true,
        hideOnOutsideClick: true,
        position: {
            my: "top right",
            at: "top right",
            of: window
        },
        animation: {
            show: {
                type: "slide",
                duration: 300,
                from: { right: -window.innerWidth * 0.5 },
                to: { right: 0 }
            },
            hide: {
                type: "fade",
                duration: 300
            }
        },
        contentTemplate: function (contentElement) {
            contentElement.append(`
                <div id="popupNoteContent">
                    <div class="d-flex justify-content-center align-items-center p-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Đang tải...</span>
                        </div>
                        <span class="ms-3">Đang tải ghi chú...</span>
                    </div>
                </div>
            `);
        },
        onShown: function () {
            const container = $("#popupNoteContent");

            if (popupNote._workItemId) {
                $.get(appUrls.notePartial, { workItemId: popupNote._workItemId }, function (html) {
                    container.html(html);
                    if (typeof initNotePopup === "function") {
                        initNotePopup();
                    }

                    $(".note-container").addClass("border rounded p-3 mb-3");
                    $(".note-form").addClass("bg-light p-3 rounded border");
                }).fail(function () {
                    container.html(`
                        <div class="alert alert-danger" role="alert">
                            <i class="bi bi-exclamation-triangle-fill me-2"></i>
                            Không thể tải ghi chú. Vui lòng thử lại sau.
                        </div>
                    `);
                });
            } else {
                container.html(`
                    <div class="alert alert-warning" role="alert">
                        <i class="bi bi-info-circle-fill me-2"></i>
                        Không có công việc nào được chọn.
                    </div>
                `);
            }
        },
        onHidden: function () {
            popupNote._workItemId = undefined;
            window.currentWorkItemId = undefined;
            $("#popupNoteContent").empty();
        }
    }).dxPopup("instance");

    // Open note popup when clicking on note button
    $(document).on("click", ".btn-open-note", function (e) {
        e.preventDefault();
        const workItemId = $(this).data("id");
        popupNote._workItemId = workItemId;
        window.currentWorkItemId = workItemId;
        popupNote.show(workItemId);
    });

    //// Expand/Collapse buttons
    //$("#btnExpandAll").on("click", function () {
    //    $(".task-card").addClass("expanded");
    //});

    //$("#btnCollapseAll").on("click", function () {
    //    $(".task-card").removeClass("expanded");
    //});

    // Responsive layout changes
    $(window).on('resize', function () {
        if (window.innerWidth < 768) {
            $("#workItemGrid").addClass("dx-datagrid-compact");
        } else {
            $("#workItemGrid").removeClass("dx-datagrid-compact");
        }
    }).trigger('resize');

    // Initialize filter collapse state from localStorage
    const filterState = localStorage.getItem('filterCollapseState');
    if (filterState === 'collapsed') {
        $("#filterCollapse").removeClass('show');
    }

    $("#filterCollapse").on('hidden.bs.collapse shown.bs.collapse', function () {
        const isCollapsed = !$(this).hasClass('show');
        localStorage.setItem('filterCollapseState', isCollapsed ? 'collapsed' : 'expanded');
    });

    // Enhance DataGrid appearance
    enhanceDataGrid();

    function enhanceDataGrid() {
        $(".dx-datagrid-headers").addClass("bg-light");
        $(".dx-datagrid-rowsview .dx-row-alt").addClass("bg-light-subtle");

        if (window.innerWidth < 768) {
            $(".dx-datagrid-table").addClass("table-responsive");
        }

        $(".dx-data-row").hover(
            function () { $(this).addClass("bg-light"); },
            function () { $(this).removeClass("bg-light"); }
        );
    }
});

