-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: MYSQL8010.site4now.net
-- Tiempo de generación: 18-06-2025 a las 10:13:11
-- Versión del servidor: 8.0.36
-- Versión de PHP: 8.3.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `db_whats1`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mensajes`
--

CREATE TABLE `mensajes` (
  `Mensaje_id` int NOT NULL,
  `usuario_id` varchar(50) DEFAULT NULL,
  `Origen` varchar(150) DEFAULT NULL,
  `Remitente` varchar(150) DEFAULT NULL,
  `Fecha_inserta` datetime DEFAULT NULL,
  `Destinatarios` varchar(150) DEFAULT NULL,
  `Mensaje` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
  `Respuesta` varchar(250) DEFAULT NULL,
  `Respuesta_subid` varchar(50) DEFAULT NULL,
  `Respuesta_code` varchar(50) DEFAULT NULL,
  `Respuesta_message` varchar(50) DEFAULT NULL,
  `Fecha_desde` datetime DEFAULT NULL,
  `Fecha_hasta` datetime DEFAULT NULL,
  `PaisDestino` varchar(5) DEFAULT NULL,
  `Archivo_local` varchar(255) DEFAULT NULL,
  `Archivo_url` varchar(255) DEFAULT NULL,
  `Fecha_envio` datetime DEFAULT NULL,
  `Es_enviado` tinyint(1) DEFAULT NULL,
  `Intentos` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

--
-- Volcado de datos para la tabla `mensajes`
--

INSERT INTO `mensajes` (`Mensaje_id`, `usuario_id`, `Origen`, `Remitente`, `Fecha_inserta`, `Destinatarios`, `Mensaje`, `Respuesta`, `Respuesta_subid`, `Respuesta_code`, `Respuesta_message`, `Fecha_desde`, `Fecha_hasta`, `PaisDestino`, `Archivo_local`, `Archivo_url`, `Fecha_envio`, `Es_enviado`, `Intentos`) 
VALUES
(144, 'tangente', '::1', NULL, '2025-06-18 16:53:30', '1122334455', 'Gracias por registrarte 🙏, Marcelo.\nPresenta este código en el evento :\n*44B020B287*.\n🔥 MENSAJES ETERNOS 🔥', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2025-06-18 16:53:41', 1, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mgk_historia_acceso`
--

CREATE TABLE `mgk_historia_acceso` (
  `historia_id` int UNSIGNED NOT NULL,
  `usuario_id` int UNSIGNED NOT NULL,
  `fecha_inicio` datetime NOT NULL,
  `fecha_salida` datetime DEFAULT NULL,
  `direccion_id` varchar(15) NOT NULL,
  `latitud` varchar(18) DEFAULT NULL,
  `longitud` varchar(18) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario_servicio`
--

CREATE TABLE `usuario_servicio` (
  `usuario_id` varchar(20) NOT NULL,
  `url` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Volcado de datos para la tabla `usuario_servicio`
--

INSERT INTO `usuario_servicio` (`usuario_id`, `url`) VALUES
('otro', 'http://127.0.0.1:8085/'),
('tangente', 'http://127.0.0.1:8086/');

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_mensaje_servicio_url`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_mensaje_servicio_url` (
`_url_service` varchar(500)
,`Archivo_local` varchar(255)
,`Archivo_url` varchar(255)
,`Destinatarios` varchar(150)
,`Es_enviado` tinyint(1)
,`Fecha_desde` datetime
,`Fecha_envio` datetime
,`Fecha_hasta` datetime
,`Fecha_inserta` datetime
,`intentos` int
,`Mensaje` varchar(500)
,`Mensaje_id` int
,`Origen` varchar(150)
,`PaisDestino` varchar(5)
,`Remitente` varchar(150)
,`Respuesta` varchar(250)
,`Respuesta_code` varchar(50)
,`Respuesta_message` varchar(50)
,`Respuesta_subid` varchar(50)
,`usuario_id` varchar(50)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_grupou`
--

CREATE TABLE `_grupou` (
  `Grupou_code` varchar(5) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Es_activo` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_grupou_menu`
--

CREATE TABLE `_grupou_menu` (
  `Grupou_code` varchar(5) NOT NULL,
  `Menu_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_grupou_modulo`
--

CREATE TABLE `_grupou_modulo` (
  `Grupou_code` varchar(5) NOT NULL,
  `Modulo_cod` varchar(20) NOT NULL,
  `Permisos` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_l_acceso`
--

CREATE TABLE `_l_acceso` (
  `Acceso_id` int NOT NULL,
  `Usuario_id` varchar(20) NOT NULL,
  `Fecha_inicio` datetime NOT NULL,
  `Fecha_actualiza` datetime NOT NULL,
  `Fecha_fin` datetime DEFAULT NULL,
  `Origen` varchar(100) DEFAULT NULL,
  `Auxx` varchar(500) DEFAULT NULL,
  `Did` varchar(500) DEFAULT NULL,
  `OsName` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Volcado de datos para la tabla `_l_acceso`


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_l_actualiza`
--

CREATE TABLE `_l_actualiza` (
  `Actualizacion_id` int NOT NULL,
  `Nombre_tabla` varchar(50) NOT NULL,
  `Fecha_actualiza` datetime NOT NULL,
  `Comentarios` varchar(100) DEFAULT NULL,
  `Emisor_id` int NOT NULL,
  `Es_cambios` bit(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_l_historia_tabla`
--

CREATE TABLE `_l_historia_tabla` (
  `Historia_tabla_id` int NOT NULL,
  `Nombre` varchar(20) NOT NULL,
  `Llave` varchar(20) NOT NULL,
  `Operacion` char(1) DEFAULT NULL,
  `Acceso_id` int NOT NULL,
  `Usuario` varchar(20) NOT NULL,
  `Fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_menu`
--

CREATE TABLE `_menu` (
  `Menu_id` int NOT NULL,
  `Titulo` varchar(50) NOT NULL,
  `Descripcion` varchar(50) DEFAULT NULL,
  `Imagen` varchar(100) DEFAULT NULL,
  `Ventana` varchar(100) DEFAULT NULL,
  `Enlace` varchar(250) DEFAULT NULL,
  `Es_activo` tinyint(1) NOT NULL,
  `I18n` varchar(50) NOT NULL,
  `Orden` int DEFAULT NULL,
  `Grupo` int DEFAULT NULL,
  `Menu_id_padre` int DEFAULT NULL,
  `Auxx` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_modulo`
--

CREATE TABLE `_modulo` (
  `Modulo_cod` varchar(20) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Es_activo` bit(1) DEFAULT NULL,
  `Permisos` int NOT NULL,
  `Ruta` varchar(50) NOT NULL,
  `RutaApi` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_parametro`
--

CREATE TABLE `_parametro` (
  `Parametro_clave` varchar(50) NOT NULL,
  `Valor` varchar(200) DEFAULT NULL,
  `Descripcion` varchar(80) NOT NULL,
  `Tipo` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_session`
--

CREATE TABLE `_session` (
  `Session_id` int NOT NULL,
  `Usuario_id` varchar(50) NOT NULL,
  `Front` varchar(50) DEFAULT NULL,
  `Acceso_id` int DEFAULT NULL,
  `Session_json` mediumtext,
  `Fecha_inicio` datetime DEFAULT NULL,
  `Fecha_actualiza` datetime DEFAULT NULL,
  `Origen` varchar(100) DEFAULT NULL,
  `Auxx` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_usuario`
--

CREATE TABLE `_usuario` (
  `Usuario_id` varchar(20) NOT NULL,
  `Password` varchar(40) NOT NULL,
  `Password_tmp` varchar(40) NOT NULL,
  `Nombre` varchar(60) NOT NULL,
  `Email` varchar(80) NOT NULL,
  `Telefono` varchar(80) NOT NULL,
  `Es_activo` bit(1) DEFAULT NULL,
  `Externo_id` varchar(100) DEFAULT NULL,
  `Externo_id2` varchar(100) DEFAULT NULL,
  `Fecha_ultimo_ingreso` datetime DEFAULT NULL,
  `Intentos_fallidos` int DEFAULT NULL,
  `Fecha_ultimo_intento` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Volcado de datos para la tabla `_usuario`
--

INSERT INTO `_usuario` (`Usuario_id`, `Password`, `Password_tmp`, `Nombre`, `Email`, `Telefono`, `Es_activo`, `Externo_id`, `Externo_id2`, `Fecha_ultimo_ingreso`, `Intentos_fallidos`, `Fecha_ultimo_intento`) VALUES
('otro', 'be84f894c545f542c15b1b4d1a38b22a', '', '', 'demo.correow@gmail.com', '', b'1', NULL, NULL, '2025-06-13 12:29:54', 0, NULL),
('tangente', 'be84f894c545f542c15b1b4d1a38b22a', '', '', 'demo.correow@gmail.com', '', b'1', NULL, NULL, '2025-06-13 12:29:54', 0, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `_usuario_grupou`
--

CREATE TABLE `_usuario_grupou` (
  `Usuario_id` varchar(20) NOT NULL,
  `Grupou_code` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `_v_usuario`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `_v_usuario` (
`Email` varchar(80)
,`Es_activo` bit(1)
,`Externo_id` varchar(100)
,`Externo_id2` varchar(100)
,`Nombre` varchar(60)
,`Password` varchar(40)
,`Password_tmp` varchar(40)
,`Telefono` varchar(80)
,`Usuario_id` varchar(20)
);

-- --------------------------------------------------------

--
-- Estructura para la vista `v_mensaje_servicio_url`
--
DROP TABLE IF EXISTS `v_mensaje_servicio_url`;

CREATE ALGORITHM=UNDEFINED DEFINER=`a4ef68_whats1`@`%` SQL SECURITY DEFINER VIEW `v_mensaje_servicio_url`  AS SELECT `m`.`Intentos` AS `intentos`, `m`.`Mensaje_id` AS `Mensaje_id`, `m`.`usuario_id` AS `usuario_id`, `m`.`Origen` AS `Origen`, `m`.`Remitente` AS `Remitente`, `m`.`Fecha_inserta` AS `Fecha_inserta`, `m`.`Destinatarios` AS `Destinatarios`, `m`.`Mensaje` AS `Mensaje`, `m`.`Respuesta` AS `Respuesta`, `m`.`Respuesta_subid` AS `Respuesta_subid`, `m`.`Respuesta_code` AS `Respuesta_code`, `m`.`Respuesta_message` AS `Respuesta_message`, `m`.`Fecha_desde` AS `Fecha_desde`, `m`.`Fecha_hasta` AS `Fecha_hasta`, `m`.`PaisDestino` AS `PaisDestino`, `m`.`Archivo_local` AS `Archivo_local`, `m`.`Archivo_url` AS `Archivo_url`, `m`.`Fecha_envio` AS `Fecha_envio`, `m`.`Es_enviado` AS `Es_enviado`, `us`.`url` AS `_url_service` FROM ((`mensajes` `m` join `_usuario` `u` on((`u`.`Usuario_id` = `m`.`usuario_id`))) join `usuario_servicio` `us` on((`us`.`usuario_id` = `u`.`Usuario_id`))) ;

-- --------------------------------------------------------

--
-- Estructura para la vista `_v_usuario`
--
DROP TABLE IF EXISTS `_v_usuario`;

CREATE ALGORITHM=UNDEFINED DEFINER=`a4ef68_whats1`@`%` SQL SECURITY DEFINER VIEW `_v_usuario`  AS SELECT `_usuario`.`Usuario_id` AS `Usuario_id`, `_usuario`.`Password` AS `Password`, `_usuario`.`Password_tmp` AS `Password_tmp`, `_usuario`.`Nombre` AS `Nombre`, `_usuario`.`Email` AS `Email`, `_usuario`.`Telefono` AS `Telefono`, `_usuario`.`Es_activo` AS `Es_activo`, `_usuario`.`Externo_id` AS `Externo_id`, `_usuario`.`Externo_id2` AS `Externo_id2` FROM `_usuario` ;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `mensajes`
--
ALTER TABLE `mensajes`
  ADD PRIMARY KEY (`Mensaje_id`);

--
-- Indices de la tabla `mgk_historia_acceso`
--
ALTER TABLE `mgk_historia_acceso`
  ADD PRIMARY KEY (`historia_id`),
  ADD KEY `usuario_id` (`usuario_id`);

--
-- Indices de la tabla `usuario_servicio`
--
ALTER TABLE `usuario_servicio`
  ADD PRIMARY KEY (`usuario_id`);

--
-- Indices de la tabla `_grupou`
--
ALTER TABLE `_grupou`
  ADD PRIMARY KEY (`Grupou_code`);

--
-- Indices de la tabla `_grupou_menu`
--
ALTER TABLE `_grupou_menu`
  ADD PRIMARY KEY (`Grupou_code`,`Menu_id`);

--
-- Indices de la tabla `_grupou_modulo`
--
ALTER TABLE `_grupou_modulo`
  ADD PRIMARY KEY (`Grupou_code`,`Modulo_cod`);

--
-- Indices de la tabla `_l_acceso`
--
ALTER TABLE `_l_acceso`
  ADD PRIMARY KEY (`Acceso_id`);

--
-- Indices de la tabla `_l_actualiza`
--
ALTER TABLE `_l_actualiza`
  ADD PRIMARY KEY (`Actualizacion_id`);

--
-- Indices de la tabla `_l_historia_tabla`
--
ALTER TABLE `_l_historia_tabla`
  ADD PRIMARY KEY (`Historia_tabla_id`);

--
-- Indices de la tabla `_menu`
--
ALTER TABLE `_menu`
  ADD PRIMARY KEY (`Menu_id`);

--
-- Indices de la tabla `_modulo`
--
ALTER TABLE `_modulo`
  ADD PRIMARY KEY (`Modulo_cod`);

--
-- Indices de la tabla `_parametro`
--
ALTER TABLE `_parametro`
  ADD PRIMARY KEY (`Parametro_clave`);

--
-- Indices de la tabla `_session`
--
ALTER TABLE `_session`
  ADD PRIMARY KEY (`Session_id`);

--
-- Indices de la tabla `_usuario`
--
ALTER TABLE `_usuario`
  ADD PRIMARY KEY (`Usuario_id`);

--
-- Indices de la tabla `_usuario_grupou`
--
ALTER TABLE `_usuario_grupou`
  ADD PRIMARY KEY (`Usuario_id`,`Grupou_code`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `mensajes`
--
ALTER TABLE `mensajes`
  MODIFY `Mensaje_id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=145;

--
-- AUTO_INCREMENT de la tabla `mgk_historia_acceso`
--
ALTER TABLE `mgk_historia_acceso`
  MODIFY `historia_id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `_l_acceso`
--
ALTER TABLE `_l_acceso`
  MODIFY `Acceso_id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `_session`
--
ALTER TABLE `_session`
  MODIFY `Session_id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
