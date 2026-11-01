document.addEventListener("DOMContentLoaded", () => {

    document.getElementById("RegisterClick").addEventListener("click", function () {
        textLogin = document.getElementById("LoginInput").value;
        textEmail = document.getElementById("EmailInput").value;
        textPassword = document.getElementById("PasswordInput").value;

        createUser(textLogin, textEmail, textPassword);
    });
});

async function createUser(textLogin, textEmail, textPassword)
{
    user =
        {
            login: textLogin,
            email: textEmail,
            password: textPassword
        };
    try
    {
        const response = await fetch("/api/user", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            console.log("Ошибка : ", response.status);
            return;
        }
        const result = await response.json();
        console.log("Пользователь создан");
        console.log(result);
        return result;
    }
    catch (error) {
        console.log("Ошибка : ", error);
    }
}