// Enhanced Toast for Success and Error only
function showToast(type, message, options = {}) {
    const defaults = {
        duration: 3000,
        gravity: "top",
        position: "right",
        close: true,
        stopOnFocus: true,
        escapeMarkup: false
    };

    const config = { ...defaults, ...options };
    const styles = getToastStyles(type);

    Toastify({
        text: `
            <div class="toast-content">
                <div class="toast-icon">${getToastIcon(type)}</div>
                <div class="toast-message">
                    <div class="toast-title">${getToastTitle(type)}</div>
                    <div class="toast-text">${message}</div>
                </div>
            </div>
        `,
        duration: config.duration,
        gravity: config.gravity,
        position: config.position,
        close: config.close,
        stopOnFocus: config.stopOnFocus,
        style: {
            background: styles.gradient,
            borderRadius: "12px",
            boxShadow: styles.boxShadow,
            border: styles.border,
            padding: "16px 20px",
            minWidth: "320px",
            fontFamily: "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif"
        },
        onClick: config.onClick || null,
        callback: config.callback || null
    }).showToast();
}

// Get icon for success and error only
function getToastIcon(type) {
    const icons = {
        success: `<svg width="20" height="20" viewBox="0 0 20 20" fill="none">
                    <circle cx="10" cy="10" r="10" fill="#10B981"/>
                    <path d="M6 10l2 2 6-6" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>`,
        error: `<svg width="20" height="20" viewBox="0 0 20 20" fill="none">
                  <circle cx="10" cy="10" r="10" fill="#EF4444"/>
                  <path d="M8 8l4 4M12 8l-4 4" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>`
    };
    return icons[type];
}

// Get title for success and error only
function getToastTitle(type) {
    const titles = {
        success: "Thành công!",
        error: "Lỗi!"
    };
    return titles[type];
}

// Get styles for success and error only
function getToastStyles(type) {
    const styles = {
        success: {
            gradient: "linear-gradient(135deg, #10B981 0%, #059669 100%)",
            boxShadow: "0 10px 25px rgba(16, 185, 129, 0.3), 0 4px 10px rgba(0, 0, 0, 0.1)",
            border: "1px solid rgba(255, 255, 255, 0.2)"
        },
        error: {
            gradient: "linear-gradient(135deg, #EF4444 0%, #DC2626 100%)",
            boxShadow: "0 10px 25px rgba(239, 68, 68, 0.3), 0 4px 10px rgba(0, 0, 0, 0.1)",
            border: "1px solid rgba(255, 255, 255, 0.2)"
        }
    };
    return styles[type];
}

// Utility functions
function showSuccessToast(message, options = {}) {
    showToast('success', message, options);
}

function showErrorToast(message, options = {}) {
    showToast('error', message, options);
}



// Add styles to head
if (!document.querySelector('#toast-styles')) {
    const styleElement = document.createElement('div');
    styleElement.id = 'toast-styles';
    styleElement.innerHTML = toastStyles;
    document.head.appendChild(styleElement);
}

// Your updated handlers
const handlers = {
    success: function (response) {
        showSuccessToast('Tạo công việc thành công!');

        const popup = $("#popupCreateWorkItem").dxPopup("instance");
        popup.hide();
        $('#Filter').submit();
    },

    error: function (xhr) {
        // Get specific error message if available
        let errorMessage = 'Lỗi khi tạo công việc!';

        try {
            const response = JSON.parse(xhr.responseText);
            if (response.message) {
                errorMessage = response.message;
            }
        } catch (e) {
            // Use default message
        }

        showErrorToast(errorMessage, {
            duration: 4000 // Longer duration for errors
        });
    }
};

// Example usage:
/*
showSuccessToast('Tạo công việc thành công!');
showErrorToast('Lỗi khi tạo công việc!');

// With custom options
showSuccessToast('Lưu thành công!', {
    duration: 5000,
    onClick: () => console.log('Success clicked!')
});
*/