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

INSERT INTO `versenyzok` (`id`, `nev`, `lengetes1`, `ido1`, `lengetes2`, `ido2`, `lengetes3`, `ido3`, `legjobb_pont`, `legjobb_ido`, `pilla_hely`) VALUES
(1, 'Nagy Ádám', 15, 6.25, 18, 6.1, 16, 6.15, 18, 6.1, 1),
(2, 'Kovács Bence', 14, 7.01, 15, 6.88, 14, 6.95, 15, 6.88, 2),
(3, 'Tóth Csaba', 19, 5.5, 20, 5.42, 18, 5.6, 20, 5.42, 3),
(4, 'Szabó Dóra', 10, 8.12, 11, 7.99, 10, 8.05, 11, 7.99, 4),
(5, 'Varga Ede', 17, 5.95, 16, 6.02, 17, 5.9, 17, 5.9, 5),
(6, 'Kiss Fanni', 13, 7.5, 14, 7.45, 12, 7.6, 14, 7.45, 6),
(7, 'Horváth Gábor', 20, 5.3, 21, 5.25, 20, 5.35, 21, 5.25, 7),
(8, 'Lakatos Hédi', 15, 6.7, 15, 6.65, 16, 6.55, 16, 6.55, 8),
(9, 'Molnár István', 11, 7.8, 12, 7.75, 13, 7.7, 13, 7.7, 9),
(10, 'Juhász Júlia', 16, 6.35, 17, 6.3, 16, 6.4, 17, 6.3, 10);

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
