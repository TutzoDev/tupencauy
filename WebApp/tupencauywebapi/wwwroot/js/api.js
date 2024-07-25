// Función para verificar que la sesión es válida
function verifySession() {
    const token = localStorage.getItem('token');
    const tenantId = localStorage.getItem('tenantId');
    const currentPage = window.location.pathname.split('/').pop();

    // Lista de páginas que no requieren autenticación
    const unauthenticatedPages = ['login.html', 'register.html', 'sitios.html'];

    if (!tenantId) {
        if (!unauthenticatedPages.includes(currentPage)) {
            window.location.href = 'sitios.html';
        }
    }
}

// Función para cerrar sesión y redirigir a sitios.html
function logoutAndRedirect() {
    // Eliminar el token JWT y el tenant ID de localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('tenantId');
    localStorage.removeItem('idUser');
    localStorage.removeItem('siteInfo');
    // Redirigir al usuario a sitios.html
    window.location.href = 'sitios.html';
}

// Función para cargar la información del sitio y aplicar estilos dinámicos
async function loadSiteInfo() {
    const tenantId = localStorage.getItem('tenantId');
    const token = localStorage.getItem('token');
    let siteInfo = localStorage.getItem('siteInfo');

    if (!siteInfo) {
        try {
            const response = await fetch(`http://localhost:5234/api/Sitios/${tenantId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token
                }
            });

            if (!response.ok) {
                throw new Error('Error al obtener la información del sitio');
            }

            const sitio = await response.json();

            if (sitio) {
                siteInfo = {
                    siteName: sitio.nombre,
                    primaryColor: sitio.colorPrincipal,
                    secondaryColor: sitio.colorSecundario,
                    fontColor: sitio.colorTipografia
                };

                localStorage.setItem('siteInfo', JSON.stringify(siteInfo));
            }
        } catch (error) {
            console.error('Error al obtener la información del sitio:', error);
            return;
        }
    } else {
        siteInfo = JSON.parse(siteInfo);
    }

    if (siteInfo) {
        const dynamicStyles = `
            .sidebar { background-color: ${siteInfo.primaryColor} !important; }
            .navbar { background-color: ${siteInfo.secondaryColor} !important; }
            body { color: ${siteInfo.fontColor}; }
        `;
        document.getElementById('dynamic-styles').innerHTML = dynamicStyles;

        const siteNameElement = document.getElementById('siteName');
        if (siteNameElement) {
            siteNameElement.textContent = siteInfo.siteName;
        } else {
            console.error('Elemento con id siteName no encontrado.');
        }
    }
}

async function loadUserInfo() {
    const idUser = localStorage.getItem('idUser');
    const token = localStorage.getItem('token');

    if (!idUser || !token) {
        window.location.href = 'login.html';
        return;
    }

    try {
        const response = await fetch(`http://localhost:5234/api/Users/${idUser}/GetInfoUser`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch user info');
        }

        const data = await response.json();
        document.getElementById('userName').textContent = data.userName;
        document.getElementById('saldoDisponible').textContent = data.saldo.toFixed(2);

        // Asignar una imagen de perfil aleatoria al usuario
        const profileImages = [
            '../images/profiles/profile1.png',
            '../images/profiles/profile2.png',
            '../images/profiles/profile3.png',
            '../images/profiles/profile4.png',
            '../images/profiles/profile5.png'
        ];
        const randomIndex = Math.floor(Math.random() * profileImages.length);
        document.getElementById('userProfileImage').src = profileImages[randomIndex];

    } catch (error) {
        console.error('Error loading user info:', error);
    }
}

// Adjuntar eventos a los formularios
document.addEventListener('DOMContentLoaded', function () {
    const logoutButton = document.getElementById('logoutButton');
    if (logoutButton) {
        logoutButton.addEventListener('click', logoutAndRedirect);
    }

    // Verificar la sesión en las páginas protegidas
    verifySession();
    loadSiteInfo();
    loadUserInfo();
});
