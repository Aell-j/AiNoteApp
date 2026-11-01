const API = 'https://localhost:7080/api';
const API_USER = `${API}/auth`;

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('btn_register').addEventListener('click', runRegister);
});

async function runRegister() {
    const login = document.getElementById('login_register').value;
    const password = document.getElementById('password_register').value;
    const email = document.getElementById('email_register').value;
    
    const html_result = document.getElementById('result_register');
    if (login === "" || password === "" || email === "") {
        html_result.textContent = 'Заполните все поля';
        html_result.style.color = 'red'
        return;
    }

    try {
        const response = await fetch(`${API_USER}/register`, {
            method: 'POST',
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({email, login, password})
        });
        const text = await response.text();

        if (text != "") {
            html_result.textContent = text;
        } else {
            console.log(text);
            return;
        }

        if (!response.ok) {
            html_result.textContent = "Такой пользователь уже существует";
            html_result.style.color = "red";
        } else {
            const userData = { userLogin: login ,authStatus: true};
            html_result.textContent = "Вы успешно зарегистрировались";
            html_result.style.color = "green";

            if(userData.authStatus) {
                setTimeout(function () {window.location.href="index.html"}, 3000);
            }

        }
    } catch (error) {
        html_result.textContent = error.message;
        html_result.style.color = "red";
    }
}