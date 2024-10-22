﻿# tupenca.uy - Proyecto web y mobile en .NET 8 (Realizado por : Santiago Tutzo, Juan Sebastián Ugas, Facundo Aparicio, Santiago Paván)

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

- Hacer click derecho en el siguiente botón : ![image](https://github.com/user-attachments/assets/c92b8122-ae01-409f-bed6-64ec6d506792)

    Ir a: Configurar proyectos de inicio > Proyectos de inicio múltiples > Aplicar > Aceptar

- En la siguiente sección: ![image](https://github.com/user-attachments/assets/b6599b2d-79ee-451c-beb2-2de23e13acc4) clickear el botón "Iniciar" como se muestra en la imagen para ejecutar los proyectos MVC y web API.


# INSTRUCCIONES PARA EJECUTAR LA APP MÓVIL DE MANERA INALÁMBRICA: (luego de haber finalizado exitosamente la ejecución del proyecto web)
- Restaurar Paquetes NuGet: Abre una terminal en la ubicación de la solución "tupencauy.sln" y ejecuta el siguiente comando "dotnet restore"

**Creando un emulador Android:**
- Ir a la siguiente sección : ![image](https://github.com/user-attachments/assets/004c746a-ada2-494b-8feb-87ec47e1f668)
- Desplegar el menú y seleccionar la opción "Administrador de dispositivos Android"
- Crear un nuevo dispositivo Android e inicializarlo
- Ejecutar el proyecto seleccionando como opción el dispositivo recién creado

**Desde un celular físico Android:**
- Conecta tu dispositivo mediante USB a tu computadora
- Instala los drivers de tu dispositivo desde la web del fabricante si tu computadora no reconoce el celular automáticamente (omitir este paso si esto no ocurre)
- Ve al archivo "NetworkConstants.cs" y cambia el primer url de la línea 7 por "http://{ip_de_tu_computadora}:{puerto_de_ejecucion_de_proyecto_webapi}"
- Ejecuta el proyecto seleccionando tu dispositivo físico como se muestra en la siguiente imagen de ejemplo: ![image](https://github.com/user-attachments/assets/37e00935-c839-4790-b4af-bc50ebe20af5)

