// Función para manejar el login
async function handleLogin(event) {
    event.preventDefault();

    const EmailUsername = document.getElementById('username').value;
    const Password = document.getElementById('password').value;
    const tenantId = document.getElementById('tenantId').value;

    try {
        const response = await fetch('http://localhost:5234/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ EmailUsername, Password, tenantId })
        });

        if (!response.ok) {
            throw new Error('Login fallido');
        }

        const data = await response.json();

        // Verificar si el tenantId del usuario coincide con el tenantId actual
        if (data.tenantId !== tenantId) {
            throw new Error('Este usuario no pertenece a este sitio');
        }

        console.log(data);
        localStorage.setItem('token', data.token);
        localStorage.setItem('tenantId', tenantId);
        localStorage.setItem('idUser', data.idUser); // Almacenar Id del usuario

        // Redirigir basado en la respuesta de la API
        window.location.href = "pencas-disponibles.html";
    } catch (error) {
        alert(error.message);
    }
}

// Función para manejar el registro
async function handleRegister(event) {
    event.preventDefault();

    const nombre = document.getElementById('nombre').value;
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const tenantId = document.getElementById('tenantId').value;

    if (password !== confirmPassword) {
        alert('Las contraseñas no coinciden');
        return;
    }

    try {
        const response = await fetch('http://localhost:5234/api/Auth/registrarse', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ nombre, username, email, password, tenantId })
        });

        if (!response.ok) {
            throw new Error('Registro fallido');
        }

        const data = await response.json();
        console.log(data);

        localStorage.setItem('tenantId', tenantId);

        // Redirigir basado en la respuesta de la API
        window.location.href = "sitios.html";
    } catch (error) {
        alert(error.message);
    }
}

async function handleGoogleLogin(event) {
    event.preventDefault();

    const tenantId = document.getElementById('tenantId').value;

    try {
        const response = await fetch(`http://localhost:5234/api/Auth/google-login?tenantId=${tenantId}`, {
            method: 'POST', 
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error('Login fallido');
        }

        const data = await response.json();

        // Verificar si el tenantId del usuario coincide con el tenantId actual
        //if (data.tenantId !== tenantId) {
        //    throw new Error('Este usuario no pertenece a este sitio');
        //}

        console.log(data);
        localStorage.setItem('token', data.token);
        localStorage.setItem('tenantId', tenantId);
        localStorage.setItem('idUser', data.idUser); // Almacenar Id del usuario

        // Redirigir basado en la respuesta de la API
        window.location.href = "pencas-disponibles.html";
    } catch (error) {
        alert(error.message);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
});
