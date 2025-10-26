window.scrollToBottom = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Auto-grows a textarea
function autoGrow(element) {
    element.style.height = "auto"; // Reset height
    element.style.height = (element.scrollHeight) + "px"; // Set to scroll height
}

// Attach event listener for textarea
window.autoGrowTextarea = (elementRef) => {
    var element = document.querySelector('textarea');
    if (element) {
        autoGrow(element);
    }
};

// Resets textarea height (call after sending message)
window.resetTextareaHeight = () => {
    var element = document.querySelector('textarea');
    if (element) {
        element.style.height = "auto";
    }
};