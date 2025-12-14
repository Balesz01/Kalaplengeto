-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Dec 11. 20:21
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `kalaplengeto`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `versenyzok`
--

CREATE TABLE `versenyzok` (
  `id` int(11) NOT NULL,
  `nev` varchar(100) NOT NULL,
  `lengetes1` tinyint(4) NOT NULL,
  `ido1` double NOT NULL,
  `lengetes2` tinyint(4) NOT NULL,
  `ido2` double NOT NULL,
  `lengetes3` tinyint(4) NOT NULL,
  `ido3` double NOT NULL,
  `legjobb_pont` int(11) NOT NULL,
  `legjobb_ido` double NOT NULL,
  `pilla_hely` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `versenyzok`
--

INSERT INTO `versenyzok` 
(`id`, `nev`, `lengetes1`, `ido1`, `lengetes2`, `ido2`, `lengetes3`, `ido3`, `legjobb_pont`, `legjobb_ido`, `pilla_hely`) 
VALUES
(1, 'Kiss Fanni', 10, 0.0, 9, 1.0, 8, 2.0, 10, 0.0, 1),
(2, 'Juhász Júlia', 9, 1.0, 8, 2.0, 8, 2.0, 9, 1.0, 2),
(3, 'Kovács Bence', 9, 1.0, 8, 2.0, 7, 3.0, 9, 1.0, 3),
(4, 'Szabó Dóra', 8, 2.0, 7, 3.0, 6, 4.0, 8, 2.0, 4),
(5, 'Nagy Ádám', 8, 2.0, 7, 3.0, 6, 4.0, 8, 2.0, 5),
(6, 'Lakatos Hédi', 8, 2.0, 7, 3.0, 6, 4.0, 8, 2.0, 6),
(7, 'Tóth Csaba', 7, 3.0, 6, 4.0, 5, 5.0, 7, 3.0, 7),
(8, 'Varga Ede', 7, 3.0, 6, 4.0, 5, 5.0, 7, 3.0, 8),
(9, 'Molnár István', 6, 4.0, 6, 4.0, 5, 5.0, 6, 4.0, 9),
(10, 'Horváth Gábor', 6, 4.0, 5, 5.0, 4, 6.0, 6, 4.0, 10);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `versenyzok`
--
ALTER TABLE `versenyzok`
  ADD PRIMARY KEY (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
