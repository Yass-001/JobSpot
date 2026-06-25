
function truncateDescription(text, maxLength = 120) {
    return text && text.length > maxLength
        ? text.substring(0, maxLength) + "..."
        : (text || "");
}

// Expose globally if needed
window.JobPosting = { truncateDescription };

// Example usage: -> in index.html or any Razor view
// <script src="~/js/job-posting-display.js"></script>
// <script>
//     // Now you can use: window.JobPosting.truncateDescription(description, 120)
// </script>

function doSomething() {
    document.writeln("This is a test function to demonstrate the JavaScript file is loaded.");
}
