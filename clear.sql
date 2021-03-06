-- Maciej WoŸniak
-- Projekt eParafia
-- Inzynieria Danych, zima 2014/15, TCS UJ
-- clear.sql

DROP FUNCTION delete_bierzmowania() CASCADE;

DROP FUNCTION delete_komunie() CASCADE;

DROP FUNCTION delete_sluby() CASCADE;

DROP FUNCTION check_chrzest_update() CASCADE;

DROP FUNCTION check_chrzest_update() CASCADE;

DROP FUNCTION check_komunia_update() CASCADE;

DROP FUNCTION check_bierzmowanie_update() CASCADE;

DROP FUNCTION check_slub_update() CASCADE;

DROP FUNCTION check_pesel(CHAR(11), CHAR(1)) CASCADE; 

DROP FUNCTION check_ok(INTEGER) CASCADE; 

DROP FUNCTION check_dane() CASCADE; 

DROP FUNCTION check_chrzest() CASCADE; 

DROP FUNCTION check_komunia() CASCADE; 

DROP FUNCTION check_bierzmowanie() CASCADE; 

DROP FUNCTION check_slub() CASCADE; 

DROP FUNCTION insert_chrzty(CHAR(11), VARCHAR(50), VARCHAR(30), DATE, CHAR(1), CHAR(11), CHAR(11), DATE, VARCHAR(60), VARCHAR(80), VARCHAR(80)) CASCADE; 

DROP FUNCTION insert_komunie(DATE, CHAR(11)) CASCADE; 

DROP FUNCTION insert_bierzmowania(DATE, CHAR(11), VARCHAR(60), VARCHAR(80)) CASCADE;

DROP FUNCTION insert_sluby(DATE, CHAR(11), VARCHAR(80), CHAR(11), VARCHAR(60), VARCHAR(80), VARCHAR(80)) CASCADE; 

DROP TABLE IF EXISTS parafianie CASCADE;

DROP TABLE IF EXISTS sakramenty CASCADE;

DROP TABLE IF EXISTS chrzty CASCADE;

DROP TABLE IF EXISTS pierwsze_komunie CASCADE;

DROP TABLE IF EXISTS bierzmowania CASCADE;

DROP TABLE IF EXISTS sluby CASCADE;

DROP TABLE IF EXISTS pogrzeby CASCADE;