const themeToggle = document.getElementById("themeToggle");

const saveTheme = localStorage.getItem("theme");

if (saveTheme === "light") {
    document.body.classList.add("light-theme");
}

if(themeToggle){
    themeToggle.addEventListener("click", () => {
        document.body.classList.toggle("light-theme");
        
        if (document.body.classList.contains("light-theme")) {
            localStorage.setItem("theme", "light");
        }
        else {
            localStorage.setItem("theme", "dark");
        }
    })
}