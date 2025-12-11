-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Dec 11. 15:49
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
-- Tábla szerkezet ehhez a táblához `attempt`
--

CREATE TABLE `attempt` (
  `id` int(11) NOT NULL,
  `competitor_id` int(11) NOT NULL,
  `round` tinyint(4) NOT NULL CHECK (`round` between 1 and 3),
  `score` tinyint(4) NOT NULL CHECK (`score` between 0 and 10),
  `time_ms` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `attempt`
--

INSERT INTO `attempt` (`id`, `competitor_id`, `round`, `score`, `time_ms`) VALUES
(1, 1, 1, 9, 1620),
(2, 1, 2, 8, 5000),
(3, 1, 3, 10, 1050),
(4, 2, 1, 10, 1100),
(5, 2, 2, 6, 4500),
(6, 2, 3, 7, 3000),
(7, 3, 1, 8, 2200),
(8, 3, 2, 8, 2300),
(9, 3, 3, 9, 980);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `competitor`
--

CREATE TABLE `competitor` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `competitor`
--

INSERT INTO `competitor` (`id`, `name`) VALUES
(1, 'Kovács János'),
(2, 'Nagy Béla'),
(3, 'Szabó Erika');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `attempt`
--
ALTER TABLE `attempt`
  ADD PRIMARY KEY (`id`),
  ADD KEY `competitor_id` (`competitor_id`);

--
-- A tábla indexei `competitor`
--
ALTER TABLE `competitor`
  ADD PRIMARY KEY (`id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `attempt`
--
ALTER TABLE `attempt`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT a táblához `competitor`
--
ALTER TABLE `competitor`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `attempt`
--
ALTER TABLE `attempt`
  ADD CONSTRAINT `attempt_ibfk_1` FOREIGN KEY (`competitor_id`) REFERENCES `competitor` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
