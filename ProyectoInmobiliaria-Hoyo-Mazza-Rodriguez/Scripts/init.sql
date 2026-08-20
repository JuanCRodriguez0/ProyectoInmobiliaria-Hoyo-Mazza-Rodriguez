CREATE DATABASE IF NOT EXISTS inmobiliaria
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;
 
USE inmobiliaria;
 
-- 2) Tabla Propietarios
DROP TABLE IF EXISTS propietarios;
CREATE TABLE propietarios (
    idPropietario   INT AUTO_INCREMENT PRIMARY KEY,
    dni             VARCHAR(20)  NOT NULL,
    nombre          VARCHAR(100) NOT NULL,
    apellido        VARCHAR(100) NOT NULL,
    fechaNacimiento DATE         NOT NULL,
    direccion       VARCHAR(200) NOT NULL,
    telefono        VARCHAR(30)  NOT NULL,
    email           VARCHAR(150) NOT NULL,
    estado          BOOLEAN NOT NULL,
    UNIQUE KEY uq_propietarios_dni (dni)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
 
-- 3) Tabla Inquilinos
DROP TABLE IF EXISTS inquilinos;
CREATE TABLE inquilinos (
    idInquilino     INT AUTO_INCREMENT PRIMARY KEY,
    dni             VARCHAR(20)    NOT NULL,
    nombre          VARCHAR(100)   NOT NULL,
    apellido        VARCHAR(100)   NOT NULL,
    fechaNacimiento DATE           NOT NULL,
    telefono        VARCHAR(30)    NOT NULL,
    email           VARCHAR(150)   NOT NULL,
    garantes        VARCHAR(500)   NOT NULL,
    sueldo          DECIMAL(12,2)  NOT NULL,
    UNIQUE KEY uq_inquilinos_dni (dni)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
 
-- =========================================================
-- Datos de prueba 
-- =========================================================
INSERT INTO propietarios (dni, nombre, apellido, fechaNacimiento, direccion, telefono, email,estado) VALUES
('30111222', 'Juan',  'Perez',  '1985-04-12', 'Av. Siempre Viva 123', '3511234567', 'juan.perez@mail.com', 1),
('28555666', 'Maria', 'Gomez',  '1990-09-23', 'San Martin 456',       '3517654321', 'maria.gomez@mail.com', 1);
 
INSERT INTO inquilinos (dni, nombre, apellido, fechaNacimiento, telefono, email, garantes, sueldo) VALUES
('35777888', 'Carlos', 'Diaz',  '1995-01-10', '3519998888', 'carlos.diaz@mail.com', 'Perez Juan - 30111222', 450000.00),
('36999000', 'Ana',    'Lopez', '1998-06-30', '3512223344', 'ana.lopez@mail.com',   'Gomez Maria - 28555666', 380000.00);