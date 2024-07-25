# tupenca.uy - Proyecto web y mobile en .NET 8 (TSI Grupo 4 - Tecnólogo en Informática)

Este proyecto consta de una plataforma con distintos sitios web, cada uno de estos contiene una serie de pencas en las que el usuario final puede apostar.
La plataforma tiene un superadministrador que gestiona los sitios mediante un backoffice, y cada sitio tiene su propio administrador que gestiona las pencas.
Se utilizaron los frameworks .NET Core y .NET MAUI, la app mobile se implementó únicamente para Android.

## Requisitos Previos
Antes de empezar, asegúrate de tener los siguientes requisitos:
- **.NET SDK**: Asegúrate de tener instalada la última versión del .NET SDK.
- **Visual Studio 2022**: Instalarlo con soporte para .NET MAUI.
- **Cuenta de Google**: Necesaria para funcionalidad de Login con Google.
- **Firebase**: Configurado con Firebase Cloud Messaging.

**Configurar Firebase:**
- Ingresa a **https://console.firebase.google.com/**
- Crea un nuevo proyecto o usa uno existente
- Configura Firebase Cloud Messaging (FCM) siguiendo la documentación de Firebase
- Descarga el archivo google-services.json (para Android)
- Coloca el archivo **google-services.json** en la carpeta **Platforms/Android**

**Configurar Google Developers:**
- Ingresa a https://console.cloud.google.com con tu cuenta de Google
- Despliega el menú de navegacion que se encuentra en la esquina superior izquierda y selecciona la opción "APIs and services"
- Crea un nuevo proyecto
- Ve a las credenciales de API de Google
- Crea una nueva credencial de OAuth 2.0 para aplicaciones
- Configura la pantalla de consentimiento OAuth
- Agrega los URI de redirección autorizados según las necesidades de tu proyecto (Valores predeterminados: https - https://localhost:7251/signin-google, http - http://localhost:5234/signin-google)

# INSTRUCCIONES PARA EJECUTAR EL PROYECTO WEB:
- Instalar Visual Studio
- Instalar SQLServer
- Instalar SQL Server Management Studio
- Descargar comprimido del proyecto o clonarlo en la ubicación deseada
- Abrir el archivo tupencauy.sln en Visual Studio
- Restaurar Paquetes NuGet: Abre una terminal en la ubicación de la solución "tupencauy.sln" y ejecuta el siguiente comando "dotnet restore"
    
- Configurar el archivo **appsettings.json** del proyecto "tupencauy":
    - Configurar el ConnectionString: "DefaultConnection":"Server={nombre_del_servidor};Database={nombre_de_la_base_de_datos};Trusted_Connection=True;TrustServerCertificate=True"
    - Configurar las "GoogleKeys": {"ClientId": "{tu_client_id}","ClientSecret": "{tu_client_secret}" }

- Configurar el archivo **appsettings.json** del proyecto "tupencauywebapi":
    - Configurar el Jwt: "Key": "{tu_key}"
    - Configurar las "GoogleKeys": {"ClientId": "{tu_client_id}","ClientSecret": "{tu_client_secret}" }

- Ejecutar el siguiente comando en la consola de administración de paquetes NuGet : (Package Manager - PM)
    -Update-Database

- Hacer click derecho en el siguiente botón : ![image](https://github.com/facundoa17IT/TSI-.NET/assets/80794153/43461065-191b-4467-855c-7a229ad4d6d3) 
    Ir a: Configurar proyectos de inicio > Proyectos de inicio múltiples > Aplicar > Aceptar

- En la siguiente sección: ![image](https://github.com/facundoa17IT/TSI-.NET/assets/80794153/75b6bda4-88f9-412e-9590-0dc5bfaa7c3e) clickear en la flecha y elegir la opción "https".
    Finalmente clickear el botón de play verde para ejecutar los proyectos MVC y web API.


# INSTRUCCIONES PARA EJECUTAR LA APP MÓVIL: (luego de haber finalizado exitosamente la ejecución del proyecto web)
- Restaurar Paquetes NuGet: Abre una terminal en la ubicación de la solución "tupencauy.sln" y ejecuta el siguiente comando "dotnet restore"

**Creando un emulador Android:**
- Ir a la siguiente sección : ![image](https://github.com/facundoa17IT/TSI-.NET/assets/80794153/8b20820d-d0ce-4722-915e-cdf9519ebb5d)
- Desplegar el menú y seleccionar la opción "Administrador de dispositivos Android"
- Crear un nuevo dispositivo Android e inicializarlo
- Ejecutar el proyecto seleccionando como opción el dispositivo recién creado

**Desde un celular físico Android:**
- Conecta tu dispositivo mediante USB a tu computadora
- Instala los drivers de tu dispositivo desde la web del fabricante si tu computadora no reconoce el celular automáticamente (omitir este paso si esto no ocurre)
- Ejecuta el proyecto seleccionando tu dispositivo físico como se muestra en la siguiente imagen de ejemplo: ![image](https://github.com/facundoa17IT/TSI-.NET/assets/80794153/2593159b-75e5-41b0-8320-db518748cd26)
