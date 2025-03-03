export function enableResizable (elementId)  {
    let element = document.getElementById(elementId);
    let isResizing = false;

    element.style.position = "relative";
    element.style.userSelect = "none";

    // Create the resizer handle
    let resizer = document.createElement("div");
    resizer.style.width = "10px";
    resizer.style.height = "100%";
    resizer.style.background = "gray";
    resizer.style.position = "absolute";
    resizer.style.right = "0";
    resizer.style.top = "0";
    resizer.style.cursor = "ew-resize"; // Horizontal resize cursor

    element.appendChild(resizer);

    resizer.addEventListener("mousedown", (event) => {
        isResizing = true;
        event.preventDefault();
    });

    document.addEventListener("mousemove", (event) => {
        if (!isResizing) return;

        let newWidth = event.clientX - element.offsetLeft;
        if (newWidth > 100) { // Optional: Set a minimum width
            element.style.width = `${newWidth}px`;
        }
    });

    document.addEventListener("mouseup", () => {
        isResizing = false;
    });
};