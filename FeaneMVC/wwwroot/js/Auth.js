const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');

signUpButton.addEventListener('click', () =>
    container.classList.add('right-panel-active'));

signInButton.addEventListener('click', () =>
    container.classList.remove('right-panel-active'));


document.getElementById("contactForm").addEventListener("submit", function (event) {
    let el = document.getElementById('contactForm');
    let name = el.username.value;
    let email = el.email.value;
    let password = el.password.value;
    let credential = el.credential.value

    let errorMessageName = errorMessagePassword = errorMessageEmail = errorMessageCredential = ""

    if (credential.length === 0) {
        errorMessageCredential += "Пожалуйста заполните поле логина.<br>";
    }
    if (name.length === 0) {
        errorMessageName += "Пожалуйста заполните поле имени.<br>";
    }
    if (email.length === 0) {
        errorMessageEmail += "Пожалуйста заполните поле email!<br>";
    }
    if (password.length === 0) {
        errorMessagePassword += "Пожалуйста введите пароль.<br>";
    }

    if (name.length < 4) {
        errorMessageName += "Некорректная длина имени (минимум 4 символа).<br>";
    }

    if (name.length > 20) {
        errorMessageName += "Некорректная длина имени (максимум 20 символов).<br>";
    }

    if (password.length < 8) {
        errorMessagePassword += "Слишком короткий пароль (минимум 8 символов).<br>";
    } else {

        if (!/[A-Z]/.test(password)) {
            errorMessagePassword += "Пароль должен содержать хотя бы одну заглавную букву.<br>";
        }

        if (!/[0-9]/.test(password)) {
            errorMessagePassword += "Пароль должен содержать хотя бы одну цифру.<br>";
        }
    }
    if (errorMessageCredential !== "") {
        document.getElementById("errorCredential").innerHTML = errorMessageName;
        event.preventDefault(); // Prevent form submission
    }
    if (errorMessageName !== "") {
        document.getElementById("errorName").innerHTML = errorMessageName;
        event.preventDefault(); // Prevent form submission
    }
    if (errorMessageEmail !== "") {
        document.getElementById("errorEmail").innerHTML = errorMessageEmail;
        event.preventDefault(); // Prevent form submission
    }
    if (errorMessagePassword !== "") {
        document.getElementById("errorPassword").innerHTML = errorMessagePassword;
        event.preventDefault(); // Prevent form submission
    }

});
