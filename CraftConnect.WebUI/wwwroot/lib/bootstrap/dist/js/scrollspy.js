let observer;
let dotNetHelper;

export function initScrollSpy(dotNetObj, contentSelector) {
    dotNetHelper = dotNetObj;

    // This configuration triggers when a section is in the top 30% of the viewport
    let options = {
        rootMargin: "-20% 0px -70% 0px",
        threshold: 0
    };

    observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // When a section enters the "active" area, tell .NET
                dotNetHelper.invokeMethodAsync('SetActiveSection', entry.target.id);
            }
        });
    }, options);

    // Find all <section> elements with an [id] inside the content area
    const sections = document.querySelectorAll(contentSelector);
    sections.forEach(section => {
        if (section.id) {
            observer.observe(section);
        }
    });
}

// Clean up the observer when the Blazor component is disposed
export function cleanupScrollSpy() {
    if (observer) {
        observer.disconnect();
    }
    // .NET reference is disposed of on the C# side
}
