const API = 'https://localhost:7080/api';
const API_USER = `${API}/auth`;

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('btn_login').addEventListener('click', runLogin);
});

async function runLogin() {
        const login = document.getElementById('login_user').value;
        const password = document.getElementById('password_user').value;
        const html_result = document.getElementById('result_login');
        if (login === "" || password === "") {
            html_result.textContent = 'Заполните все поля';
            html_result.style.backgroundColor = 'red'
            return;
        }
        
    try {
        const response = await fetch(`${API_USER}/auth`, {
            method: 'POST',
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({login, password})
        });
        const text = await response.text();

        if (text != "") {
            html_result.textContent = text;
        } else {
            console.log(text);
            return;
        }

        if (!response.ok) {
            html_result.textContent = "Логин или пароль неверные";
            html_result.style.backgroundColor = "red";
        } else {
            const userData = { userLogin: login ,authStatus: true};
            html_result.textContent = "Вы вошли успешно";
            html_result.style.backgroundColor = "blue";
            
            if(userData.authStatus) {
                setTimeout(function () {window.location.href="index.html"}, 3000);
            }
            
        }
    } catch (error) {
        html_result.textContent = error.message;
        html_result.style.color = "red";
    }
}