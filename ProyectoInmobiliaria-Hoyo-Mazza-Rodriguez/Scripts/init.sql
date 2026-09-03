-- 1) Crear BD
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

-- 4) Tabla Inmuebles
DROP TABLE IF EXISTS tipos_inmueble;
CREATE TABLE tipos_inmueble (
    idTipoInmueble  INT AUTO_INCREMENT PRIMARY KEY,
    descripcion     VARCHAR(100) NOT NULL,
    UNIQUE KEY uq_tipos_inmueble_descripcion (descripcion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5) Tabla Inmuebles
DROP TABLE IF EXISTS inmuebles;
CREATE TABLE inmuebles (
    idInmueble      INT AUTO_INCREMENT PRIMARY KEY,
    idPropietario   INT NOT NULL,
    idTipoInmueble  INT NOT NULL,
    direccion       VARCHAR(200)  NOT NULL,
    ambientes       INT           NOT NULL,
    superficie      DECIMAL(10,2) NOT NULL,
    precioPorDia    DECIMAL(12,2) NOT NULL,
    disponible      BOOLEAN       NOT NULL DEFAULT 1,
    portada         VARCHAR(300)  NULL,
    CONSTRAINT fk_inmuebles_propietario FOREIGN KEY (idPropietario) REFERENCES propietarios (idPropietario),
    CONSTRAINT fk_inmuebles_tipo FOREIGN KEY (idTipoInmueble) REFERENCES tipos_inmueble (idTipoInmueble)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6) Tabla Imágenes de Inmueble 
DROP TABLE IF EXISTS inmueble_imagenes;
CREATE TABLE inmueble_imagenes (
    idImagen        INT AUTO_INCREMENT PRIMARY KEY,
    idInmueble      INT NOT NULL,
    ruta            VARCHAR(300) NOT NULL,
    CONSTRAINT fk_imagenes_inmueble FOREIGN KEY (idInmueble) REFERENCES inmuebles (idInmueble) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7) Tabla Reservas
DROP TABLE IF EXISTS reservas;
CREATE TABLE reservas (
    idReserva       INT AUTO_INCREMENT PRIMARY KEY,
    idInquilino     INT NOT NULL,
    idInmueble      INT NOT NULL,
    montoPorDia     DECIMAL(12,2) NOT NULL,
    fechaDesde      DATE NOT NULL,
    fechaHasta      DATE NOT NULL,
    CONSTRAINT fk_reservas_inquilino FOREIGN KEY (idInquilino) REFERENCES inquilinos (idInquilino),
    CONSTRAINT fk_reservas_inmueble FOREIGN KEY (idInmueble) REFERENCES inmuebles (idInmueble)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Datos de prueba
INSERT INTO tipos_inmueble (descripcion) VALUES
('Casa'), ('Departamento'), ('PH'), ('Local Comercial');

INSERT INTO inmuebles (idPropietario, idTipoInmueble, direccion, ambientes, superficie, precioPorDia, disponible, portada) VALUES
(1, 1, 'Av. Colon 1234, Cordoba', 4, 120.50, 15000.00, 1, NULL),
(2, 2, 'Bv. San Juan 567, Cordoba', 2, 55.00, 9000.00, 1, NULL);

INSERT INTO reservas (idInquilino, idInmueble, montoPorDia, fechaDesde, fechaHasta) VALUES
(1, 1, 15000.00, '2026-01-05', '2026-01-15'),
(2, 2, 9000.00,  '2026-02-01', '2026-02-10');