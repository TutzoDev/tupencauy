document.addEventListener('DOMContentLoaded', function () {
    // Hacer una solicitud al backend para obtener los datos del sitio seleccionado
    fetch('/api/sitios/selectedSite') // Actualiza esta URL según la ruta de tu API
        .then(response => response.json())
        .then(data => {
            var siteTitle = data.title;
            var primaryColor = data.primaryColor;
            var secondaryColor = data.secondaryColor;
            var fontColor = data.fontColor;

            // Actualizar el título del sitio
            if (siteTitle) {
                document.getElementById('site-title').textContent = siteTitle;
            }

            // Aplicar los colores dinámicos
            if (primaryColor && secondaryColor && fontColor) {
                var dynamicStyles = `
                    .sidebar { background-color: ${primaryColor} !important; }
                    .navbar { background-color: ${secondaryColor} !important; }
                    body { color: ${fontColor}; }
                `;
                document.getElementById('dynamic-styles').innerHTML = dynamicStyles;
            }
        })
        .catch(error => console.error('Error fetching site data:', error));
});
