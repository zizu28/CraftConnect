export function init(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    // Handle pasting
    container.addEventListener('paste', (e) => {
        e.preventDefault();
        const paste = (e.clipboardData || window.clipboardData).getData('text');
        if (paste && paste.length === 6 && /^\d+$/.test(paste)) {
            const inputs = container.querySelectorAll('.code-input');
            inputs.forEach((input, index) => {
                input.value = paste[index];
                // Manually update the Blazor component's value
                const event = new Event('input', { bubbles: true });
                input.dispatchEvent(event);
            });
            // Focus the last input
            inputs[inputs.length - 1].focus();
        }
    });
}

export function focusNext(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.focus();
        el.select();
    }
}

export function isInputValueEmpty(elementId) {
    const el = document.getElementById(elementId);
    return el ? el.value === '' : false;
}
