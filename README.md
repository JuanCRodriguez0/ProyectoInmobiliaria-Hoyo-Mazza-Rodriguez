# 🏢 Proyecto Inmobiliaria MVC

### *Creamos un sistema de gestión de alquileres temporarios de propiedades inmuebles que realiza una agencia inmobiliaria.*    

## 📅 Entrega
- Materia: Laboratorio de Software II
- Proyecto: Sistema de Gestión Inmobiliaria (ASP.NET Core MVC + MySQL)
- Entrega: Entrega 1 - ABM Propietarios e Inquilinos

## 👥 Integrantes
- *Hoyo, Jeremias - jeremiashoyo035@gmail.com - @Ego572 https://github.com/Ego572 - Discord: agony9999*  
- *Mazza, Agustin - agusmazza@gmail.com - @AgustinMazza https://github.com/AgustinMazza - Discord: cote8942*  
- *Rodríguez, Juan Cruz - juancruzrodriguez0@gmail.com - @JuanCRodriguez0 https://github.com/JuanCRodriguez0 - Discord: juancruzr*  



## 📊 Modelado de Datos

*A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación.*

### *Diagrama de Clases*  

![Diagrama de Clases](img.png)

## 💾 Pasos a seguir para levantar la BD

### 1. Requisitos Previos
* .NET SDK 8.0 o superior
* MySQL Server
* IDE recomendado: Visual Studio Code
* Interfaz recomendada: DBeaver

### 2. Base de Datos
## 🗄️ Configuración de la Base de Datos (DBeaver)

1. **Crear la Base de Datos:**
   * Abrir DBeaver y conectarse al servidor MySQL local.
   * Hacer clic derecho sobre la conexión en el panel izquierdo $\rightarrow$ **Create** $\rightarrow$ **Database**.
   * Nombrar la base de datos como `inmobiliaria_db` y presionar **OK**.

2. **Ejecutar el Script SQL:**
   * Abrir el archivo `init.sql` ubicado en la carpeta `/Scripts` del proyecto.
   * Copiar todo su contenido.
   * En DBeaver, con la base de datos `inmobiliaria_db` seleccionada, presionar `Ctrl + ALT + X` (o ir al menú **SQL Editor** $\rightarrow$ **New SQL Script**).
   * Pegar el contenido del script.
   * Ejecutar todo el script presionando el botón **Execute Script** (el ícono con la hoja y el rayo naranja) o `Alt + X`.

3. **Verificar Tablas:**
   * Hacer clic derecho sobre la base de datos `inmobiliaria_db` en DBeaver y seleccionar **Refresh** (`F5`).
   * Desplegar la sección **Tables** para confirmar la presencia de `propietarios` e `inquilinos`.