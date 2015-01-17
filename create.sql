-- Maciej WoŸniak
-- Projekt eParafia
-- Inzynieria Danych, zima 2014/15, TCS UJ

CREATE OR REPLACE FUNCTION check_ok(id INTEGER) RETURNS BOOLEAN AS $$
	DECLARE
		b1 BOOLEAN;
		b2 BOOLEAN;
		b3 BOOLEAN;
		b4 BOOLEAN;
	BEGIN
		SELECT INTO b1 EXISTS (SELECT * FROM chrzty c WHERE c.id_sakramentu = id);
		SELECT INTO b2 EXISTS (SELECT * FROM pierwsze_komunie p WHERE p.id_sakramentu = id);
		SELECT INTO b3 EXISTS (SELECT * FROM bierzmowania b WHERE b.id_sakramentu = id);
		SELECT INTO b4 EXISTS (SELECT * FROM sluby s WHERE s.id_sakramentu = id);
		IF b1 = FALSE AND b2 = FALSE AND b3 = FALSE AND b4 = FALSE THEN
			RETURN FALSE;
		END IF;
		RETURN TRUE;
	END
$$ LANGUAGE plpgsql;

DROP TABLE IF EXISTS parafianie CASCADE;
CREATE TABLE parafianie (
	pesel CHAR(11) PRIMARY KEY,
	nazwisko CHARACTER VARYING(50) NOT NULL,
	imie CHARACTER VARYING(30) NOT NULL,
	data_urodzenia DATE NOT NULL,
	plec CHAR(1) NOT NULL,
	ojciec CHAR(11) REFERENCES parafianie(pesel) ON DELETE SET NULL,
	matka CHAR(11) REFERENCES parafianie(pesel) ON DELETE SET NULL,
	CHECK (plec IN ('K', 'M'))
);

DROP TABLE IF EXISTS sakramenty CASCADE;
CREATE TABLE sakramenty (
	id serial PRIMARY KEY,
	data_udzielenia DATE NOT NULL,
	powiazany BOOLEAN DEFAULT FALSE,
	CHECK ((powiazany = FALSE) OR (powiazany = TRUE AND check_ok(id) = TRUE))
);

DROP TABLE IF EXISTS chrzty CASCADE;
CREATE TABLE chrzty (
	id_sakramentu serial PRIMARY KEY REFERENCES sakramenty(id) ON DELETE CASCADE,
	osoba CHAR(11) UNIQUE NOT NULL REFERENCES parafianie(pesel) ON DELETE CASCADE,
	imie_chrztu CHARACTER VARYING(60) NOT NULL,
	ojciec_chrzestny CHARACTER VARYING(80) NOT NULL,
	matka_chrzestna CHARACTER VARYING(80) NOT NULL,
	CHECK (ojciec_chrzestny != matka_chrzestna)
);

DROP TABLE IF EXISTS pierwsze_komunie CASCADE;
CREATE TABLE pierwsze_komunie (
	id_sakramentu serial PRIMARY KEY REFERENCES sakramenty(id) ON DELETE CASCADE,
	osoba CHAR(11) UNIQUE NOT NULL REFERENCES parafianie(pesel) ON DELETE CASCADE
);

DROP TABLE IF EXISTS bierzmowania CASCADE;
CREATE TABLE bierzmowania (
	id_sakramentu serial PRIMARY KEY REFERENCES sakramenty(id) ON DELETE CASCADE,
	osoba CHAR(11) UNIQUE NOT NULL REFERENCES parafianie(pesel) ON DELETE CASCADE,
	imie_bierzmowania CHARACTER VARYING(30) NOT NULL,
	swiadek CHARACTER VARYING(80) NOT NULL
);

DROP TABLE IF EXISTS sluby CASCADE;
CREATE TABLE sluby (
	id_sakramentu serial PRIMARY KEY REFERENCES sakramenty(id) ON DELETE CASCADE,
	maz CHAR(11) REFERENCES parafianie(pesel) ON DELETE CASCADE,
	dane_meza CHARACTER VARYING(80),
	zona CHAR(11) REFERENCES parafianie(pesel) ON DELETE CASCADE,
	dane_zony CHARACTER VARYING(80),
	swiadek1 CHARACTER VARYING(80) NOT NULL,
	swiadek2 CHARACTER VARYING(80) NOT NULL,
	uniewazniony BOOLEAN NOT NULL DEFAULT FALSE,
	CHECK (swiadek1 != swiadek2),
	CHECK ((dane_zony IS NOT NULL AND zona IS NULL) OR (dane_zony IS NULL AND zona IS NOT NULL)),
	CHECK ((dane_meza IS NOT NULL AND maz IS NULL) OR (dane_meza IS NULL AND maz IS NOT NULL)),
	CHECK (maz IS NOT NULL OR zona IS NOT NULL)
);

DROP TABLE IF EXISTS pogrzeby CASCADE;
CREATE TABLE pogrzeby (
	osoba CHAR(11) PRIMARY KEY REFERENCES parafianie(pesel) ON DELETE CASCADE,
	data_smierci DATE,
	data_pogrzebu DATE NOT NULL,
	CHECK (data_smierci IS NULL OR data_smierci <= data_pogrzebu)
);

CREATE OR REPLACE FUNCTION check_pesel(pesel CHAR(11), plec CHAR(1)) RETURNS BOOLEAN AS $$
      DECLARE c INT;
      DECLARE a CHAR ARRAY[11];
      DECLARE w INT;
      DECLARE ch CHAR;
      BEGIN
              IF pesel IS NULL  THEN RETURN FALSE; END IF;
              SELECT INTO a * FROM (SELECT regexp_split_to_array(pesel,'')) as arr ;
              IF plec = 'K' THEN
              	ch := a[10];
              	IF ch = '0' or ch = '2' or ch = '4' or ch = '6' or ch = '8' THEN
              		c:=0;
              	ELSE RETURN FALSE;
              	END IF;
              END IF;
              IF plec = 'M' THEN
              	ch := a[10];
              	IF ch = '1' or ch = '3' or ch = '5' or ch = '7' or ch = '9' THEN
              		c:=0;
              	ELSE RETURN FALSE;
              	END IF;
              END IF;
              FOR i IN 1..10 LOOP
                      ch := a[i];
                      IF ch='0' or ch='1' or ch='2' or ch='3' or ch='4' or ch='5' or ch='6' or ch='7' or ch='8' or ch='9' THEN
                              c := 0;
                      ELSE RETURN FALSE;
                      END IF;
              END LOOP;
              w := a[1]::INT + 3*(a[2]::INT) + 7*(a[3]::int) + 9*(a[4]::int) + (a[5]::int) + 3*(a[6]::int)+7*(a[7]::int)+9*(a[8]::int) + a[9]::int + 3*(a[10]::int);
              w := w % 10;
              w := 10 - w;
              w := w % 10;
              IF w = a[11]::int THEN
                      RETURN TRUE;
              END IF;
              RETURN FALSE;
              END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION check_dane() RETURNS trigger AS $check_dane$
	DECLARE r RECORD;
    BEGIN
    	IF NEW.ojciec IS NOT NULL THEN
		    SELECT INTO r * FROM parafianie WHERE pesel = NEW.ojciec LIMIT 1;
	        IF r.data_urodzenia >= NEW.data_urodzenia THEN
	        	RAISE EXCEPTION 'Zla data urodzenia!';
	        END IF;
	        IF r.plec != 'M' THEN 
	        	RAISE EXCEPTION 'Zla plec ojca!';
	        END IF;
		END IF;
		IF NEW.matka IS NOT NULL THEN
			SELECT INTO r * FROM parafianie WHERE pesel = NEW.matka LIMIT 1;
	        IF r.data_urodzenia >= NEW.data_urodzenia THEN
	        	RAISE EXCEPTION 'Zla data urodzenia!';
	        END IF;
	        IF r.plec != 'K' THEN 
	        	RAISE EXCEPTION 'Zla plec matki!';
		    END IF;
		END IF;
		IF check_pesel(NEW.pesel, NEW.plec) = FALSE THEN
			RAISE EXCEPTION 'Niepoprawny pesel!';
		END IF;
	    RETURN NEW;
    END;
$check_dane$ LANGUAGE plpgsql;

CREATE TRIGGER check_dane BEFORE INSERT OR UPDATE ON parafianie FOR EACH ROW
    EXECUTE PROCEDURE check_dane();

CREATE OR REPLACE FUNCTION check_chrzest() RETURNS trigger AS $check_chrzest$
	DECLARE r RECORD;
    BEGIN
	    SELECT INTO r * FROM parafianie WHERE pesel = NEW.osoba LIMIT 1;
	    IF r.data_urodzenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	    	DELETE FROM parafianie WHERE pesel = NEW.osoba;
	        RAISE EXCEPTION 'Zla data chrztu!';
	    END IF;
	    RETURN NEW;
    END;
$check_chrzest$ LANGUAGE plpgsql;

CREATE TRIGGER check_chrzest BEFORE INSERT ON chrzty FOR EACH ROW
    EXECUTE PROCEDURE check_chrzest();  

CREATE OR REPLACE FUNCTION check_chrzest_update() RETURNS trigger AS $check_chrzest_update$
	DECLARE r RECORD;
    BEGIN
	    SELECT INTO r * FROM parafianie WHERE pesel = NEW.osoba LIMIT 1;
	    IF r.data_urodzenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	        RAISE EXCEPTION 'Zla data chrztu!';
	    END IF;
	    RETURN NEW;
    END;
$check_chrzest_update$ LANGUAGE plpgsql;

CREATE TRIGGER check_chrzest_update BEFORE UPDATE ON chrzty FOR EACH ROW
    EXECUTE PROCEDURE check_chrzest_update(); 

CREATE OR REPLACE FUNCTION check_komunia() RETURNS trigger AS $check_komunia$
	DECLARE r RECORD;
    BEGIN
	    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.osoba)) = FALSE THEN 
	    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
	    END IF;

	    SELECT INTO r * FROM chrzty c join sakramenty s on s.id = c.id_sakramentu WHERE c.osoba = NEW.osoba LIMIT 1;
        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
        	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
        	RAISE EXCEPTION 'Zla data komunii!';
        END IF;

	    RETURN NEW;
    END;
$check_komunia$ LANGUAGE plpgsql;

CREATE TRIGGER check_komunia BEFORE INSERT ON pierwsze_komunie FOR EACH ROW
    EXECUTE PROCEDURE check_komunia();

CREATE OR REPLACE FUNCTION check_komunia_update() RETURNS trigger AS $check_komunia_update$
	DECLARE r RECORD;
    BEGIN
	    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.osoba)) = FALSE THEN 
	    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
	    END IF;

	    SELECT INTO r * FROM chrzty c join sakramenty s on s.id = c.id_sakramentu WHERE c.osoba = NEW.osoba LIMIT 1;
        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
        	RAISE EXCEPTION 'Zla data komunii!';
        END IF;

	    RETURN NEW;
    END;
$check_komunia_update$ LANGUAGE plpgsql;

CREATE TRIGGER check_komunia_update BEFORE UPDATE ON pierwsze_komunie FOR EACH ROW
    EXECUTE PROCEDURE check_komunia_update();

CREATE OR REPLACE FUNCTION check_bierzmowanie() RETURNS trigger AS $check_bierzmowanie$
	DECLARE r RECORD;
    BEGIN
	    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.osoba)) = FALSE THEN 
	    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
	    END IF;
		IF (SELECT EXISTS (SELECT 1 from pierwsze_komunie where osoba = NEW.osoba)) = FALSE THEN 
			DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze pierwszej Komunii!';
	    END IF;
	    SELECT INTO r * FROM pierwsze_komunie p join sakramenty s on s.id = p.id_sakramentu WHERE p.osoba = NEW.osoba LIMIT 1;
        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
        	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
        	RAISE EXCEPTION 'Zla data bierzmowania!';
        END IF;

	    RETURN NEW;
    END;
$check_bierzmowanie$ LANGUAGE plpgsql;

CREATE TRIGGER check_bierzmowanie BEFORE INSERT ON bierzmowania FOR EACH ROW
    EXECUTE PROCEDURE check_bierzmowanie();

CREATE OR REPLACE FUNCTION check_bierzmowanie_update() RETURNS trigger AS $check_bierzmowanie_update$
	DECLARE r RECORD;
    BEGIN
	    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.osoba)) = FALSE THEN 
	    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
	    END IF;
		IF (SELECT EXISTS (SELECT 1 from pierwsze_komunie where osoba = NEW.osoba)) = FALSE THEN 
	    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze pierwszej Komunii!';
	    END IF;
	    SELECT INTO r * FROM pierwsze_komunie p join sakramenty s on s.id = p.id_sakramentu WHERE p.osoba = NEW.osoba LIMIT 1;
        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
        	RAISE EXCEPTION 'Zla data bierzmowania!';
        END IF;

	    RETURN NEW;
    END;
$check_bierzmowanie_update$ LANGUAGE plpgsql;

CREATE TRIGGER check_bierzmowanie_update BEFORE UPDATE ON bierzmowania FOR EACH ROW
    EXECUTE PROCEDURE check_bierzmowanie_update();

CREATE OR REPLACE FUNCTION check_slub() RETURNS trigger AS $check_slub$
	DECLARE r RECORD;
	DECLARE rec RECORD;
    BEGIN
    	IF NEW.maz IS NOT NULL THEN
		    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.maz)) = FALSE THEN 
		    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
		    END IF;
			IF (SELECT EXISTS (SELECT 1 from pierwsze_komunie where osoba = NEW.maz)) = FALSE THEN 
				DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze pierwszej Komunii!';
		    END IF;
		    IF (SELECT EXISTS (SELECT 1 from bierzmowania where osoba = NEW.maz)) = FALSE THEN 
		    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze bierzmowania!';
		    END IF;
		    SELECT INTO r * FROM bierzmowania b join sakramenty s on s.id = b.id_sakramentu WHERE b.osoba = NEW.maz LIMIT 1;
	        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	        	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	        	RAISE EXCEPTION 'Zla data slubu!';
	        END IF;
		    IF (SELECT EXISTS (SELECT 1 from sluby where maz = NEW.maz AND uniewazniony = FALSE)) = TRUE THEN
		    	FOR rec IN SELECT zona FROM sluby WHERE maz = NEW.maz AND uniewazniony = FALSE LOOP
		    		IF (SELECT EXISTS (SELECT * FROM pogrzeby WHERE osoba = zona)) = FALSE THEN
		    			DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    			RAISE EXCEPTION 'Mezczyzna jest juz w innym zwiazku malzenskim!';
		    		END IF;
		    	END LOOP;
		    END IF;
		    IF (SELECT plec FROM parafianie WHERE pesel = NEW.maz) != 'M' THEN
		    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Zla plec mezczyzny!';
		    END IF;
		END IF;

		IF NEW.zona IS NOT NULL THEN
		    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.zona)) = FALSE THEN 
		    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
		    END IF;
			IF (SELECT EXISTS (SELECT 1 from pierwsze_komunie where osoba = NEW.zona)) = FALSE THEN 
				DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze pierwszej Komunii!';
		    END IF;
		    IF (SELECT EXISTS (SELECT 1 from bierzmowania where osoba = NEW.zona)) = FALSE THEN 
		    	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze bierzmowania!';
		    END IF;
		    SELECT INTO r * FROM bierzmowania b join sakramenty s on s.id = b.id_sakramentu WHERE b.osoba = NEW.zona LIMIT 1;
	        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	        	DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
	        	RAISE EXCEPTION 'Zla data slubu!';
	        END IF;
		    IF (SELECT EXISTS (SELECT 1 from sluby where zona = NEW.zona AND uniewazniony = FALSE)) = TRUE THEN
		    	FOR rec IN SELECT maz FROM sluby WHERE zona = NEW.zona AND uniewazniony = FALSE LOOP
		    		IF (SELECT EXISTS (SELECT * FROM pogrzeby WHERE osoba = maz)) = FALSE THEN
		    			DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    			RAISE EXCEPTION 'Kobieta jest juz w innym zwiazku malzenskim!';
		    		END IF;
		    	END LOOP;
		    END IF;

		    IF (SELECT plec FROM parafianie WHERE pesel = NEW.zona) != 'K' THEN
		   		DELETE FROM sakramenty WHERE id = NEW.id_sakramentu;
		    	RAISE EXCEPTION 'Zla plec kobiety!';
		    END IF;
		END IF;
	    RETURN NEW;
    END;
$check_slub$ LANGUAGE plpgsql;

CREATE TRIGGER check_slub BEFORE INSERT ON sluby FOR EACH ROW
    EXECUTE PROCEDURE check_slub();

CREATE OR REPLACE FUNCTION check_slub_update() RETURNS trigger AS $check_slub_update$
	DECLARE r RECORD;
	DECLARE rec RECORD;
    BEGIN
    	IF NEW.maz IS NOT NULL THEN
		    SELECT INTO r * FROM bierzmowania b join sakramenty s on s.id = b.id_sakramentu WHERE b.osoba = NEW.maz LIMIT 1;
	        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	        	RAISE EXCEPTION 'Zla data slubu!';
	        END IF;
		    IF (SELECT EXISTS (SELECT 1 from sluby where maz = NEW.maz AND uniewazniony = FALSE)) = TRUE THEN
		    	FOR rec IN SELECT zona FROM sluby WHERE maz = NEW.maz AND uniewazniony = FALSE LOOP
		    		IF (SELECT EXISTS (SELECT * FROM pogrzeby WHERE osoba = zona)) = FALSE THEN
		    			RAISE EXCEPTION 'Mezczyzna jest juz w innym zwiazku malzenskim!';
		    		END IF;
		    	END LOOP;
		    END IF;
		    IF (SELECT plec FROM parafianie WHERE pesel = NEW.maz) != 'M' THEN
		    	RAISE EXCEPTION 'Zla plec mezczyzny!';
		    END IF;
		END IF;

		IF NEW.zona IS NOT NULL THEN
		    IF (SELECT EXISTS (SELECT 1 from chrzty where osoba = NEW.zona)) = FALSE THEN 
		    	RAISE EXCEPTION 'Osoba nie jest jeszcze ochrzczona!';
		    END IF;
			IF (SELECT EXISTS (SELECT 1 from pierwsze_komunie where osoba = NEW.zona)) = FALSE THEN 
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze pierwszej Komunii!';
		    END IF;
		    IF (SELECT EXISTS (SELECT 1 from bierzmowania where osoba = NEW.zona)) = FALSE THEN 
		    	RAISE EXCEPTION 'Osoba nie przyjela jeszcze bierzmowania!';
		    END IF;
		    SELECT INTO r * FROM bierzmowania b join sakramenty s on s.id = b.id_sakramentu WHERE b.osoba = NEW.zona LIMIT 1;
	        IF r.data_udzielenia > (SELECT data_udzielenia FROM sakramenty where id = NEW.id_sakramentu) THEN
	        	RAISE EXCEPTION 'Zla data slubu!';
	        END IF;
		    IF (SELECT EXISTS (SELECT 1 from sluby where zona = NEW.zona AND uniewazniony = FALSE)) = TRUE THEN
		    	FOR rec IN SELECT maz FROM sluby WHERE zona = NEW.zona AND uniewazniony = FALSE LOOP
		    		IF (SELECT EXISTS (SELECT * FROM pogrzeby WHERE osoba = maz)) = FALSE THEN
		    			RAISE EXCEPTION 'Kobieta jest juz w innym zwiazku malzenskim!';
		    		END IF;
		    	END LOOP;
		    END IF;

		    IF (SELECT plec FROM parafianie WHERE pesel = NEW.zona) != 'K' THEN
		    	RAISE EXCEPTION 'Zla plec kobiety!';
		    END IF;
		END IF;
	    RETURN NEW;
    END;
$check_slub_update$ LANGUAGE plpgsql;

CREATE TRIGGER check_slub_update BEFORE UPDATE ON sluby FOR EACH ROW
    EXECUTE PROCEDURE check_slub_update();

CREATE OR REPLACE FUNCTION insert_chrzty(osoba CHAR(11), nazwisko CHARACTER VARYING(50), imie CHARACTER VARYING(30), 
data_urodzenia DATE, plec CHAR(1), ojciec CHAR(11), matka CHAR(11), data_udzielenia DATE, imie_chrztu CHARACTER VARYING(60), 
ojciec_chrzestny CHARACTER VARYING(80), matka_chrzestna CHARACTER VARYING(80)) 
RETURNS INT AS $$
    DECLARE
    	ident INT;
    BEGIN
    	INSERT INTO parafianie values (osoba, nazwisko, imie, data_urodzenia, plec, ojciec, matka);
	    INSERT INTO sakramenty values(DEFAULT, data_udzielenia) RETURNING id INTO ident;
	    INSERT INTO chrzty values(ident, osoba, imie_chrztu, ojciec_chrzestny, matka_chrzestna);
	    UPDATE sakramenty SET powiazany = TRUE WHERE id = ident;
	    RETURN ident;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_komunie(data_udzielenia DATE, osoba CHAR(11)) RETURNS INT AS $$
    DECLARE
    	ident INT;
    BEGIN
	    INSERT INTO sakramenty values(DEFAULT, data_udzielenia) RETURNING id INTO ident;
	    INSERT INTO pierwsze_komunie values(ident, osoba);
	    UPDATE sakramenty SET powiazany = TRUE WHERE id = ident;
	    RETURN ident;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_bierzmowania(data_udzielenia DATE, osoba CHAR(11), imie CHARACTER VARYING(60), swiadek CHARACTER VARYING(80)) 
RETURNS INT AS $$
    DECLARE
    	ident INT;
    BEGIN
	    INSERT INTO sakramenty values(DEFAULT, data_udzielenia) RETURNING id INTO ident;
        INSERT INTO bierzmowania values(ident, osoba, imie, swiadek);
	    UPDATE sakramenty s SET powiazany = TRUE WHERE id = ident;
	    RETURN ident;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_sluby(data_udzielenia DATE, osoba1 CHAR(11), dane1 CHARACTER VARYING(80), osoba2 CHAR(11), 
dane2 CHARACTER VARYING(60), swiadek1 CHARACTER VARYING(80), swiadek2 CHARACTER VARYING(80)) RETURNS INT AS $$
    DECLARE
    	ident INT;
    BEGIN
	    INSERT INTO sakramenty values(DEFAULT, data_udzielenia) RETURNING id INTO ident;
	    INSERT INTO sluby values(ident, osoba1, dane1, osoba2, dane2, swiadek1, swiadek2, FALSE);
	    UPDATE sakramenty SET powiazany = TRUE WHERE id = ident;
	    RETURN NULL;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_komunie() RETURNS TRIGGER AS $delete_komunie$
	BEGIN
		DELETE FROM sakramenty where id = OLD.id_sakramentu;
		RETURN OLD;
	END;
$delete_komunie$ LANGUAGE plpgsql;

CREATE TRIGGER delete_komunie AFTER DELETE ON pierwsze_komunie FOR EACH ROW
    EXECUTE PROCEDURE delete_komunie();

CREATE OR REPLACE FUNCTION delete_bierzmowania() RETURNS TRIGGER AS $delete_bierzmowania$
	BEGIN
		DELETE FROM sakramenty where id = OLD.id_sakramentu;
		RETURN OLD;
	END;
$delete_bierzmowania$ LANGUAGE plpgsql;

CREATE TRIGGER delete_bierzmowania AFTER DELETE ON bierzmowania FOR EACH ROW
    EXECUTE PROCEDURE delete_bierzmowania();

CREATE OR REPLACE FUNCTION delete_sluby() RETURNS TRIGGER AS $delete_sluby$
	BEGIN
		DELETE FROM sakramenty where id = OLD.id_sakramentu;
		RETURN OLD;
	END;
$delete_sluby$ LANGUAGE plpgsql;

CREATE TRIGGER delete_sluby AFTER DELETE ON sluby FOR EACH ROW
    EXECUTE PROCEDURE delete_sluby();

DROP VIEW IF EXISTS all_parafianie;
CREATE VIEW all_parafianie as
	SELECT p.imie||' '||p.nazwisko||' data urodzenia: '||p.data_urodzenia, p.pesel as osoba,
	(SELECT 'Ojciec: '||(CASE WHEN p.ojciec IS NULL THEN 'Nieznany' ELSE (SELECT par.imie||' '||par.nazwisko FROM parafianie par WHERE par.pesel = p.ojciec)||' (PESEL: '||p.ojciec||')' END)) as ojciec,
	(SELECT 'Matka: '||(CASE WHEN p.ojciec IS NULL THEN 'Nieznana' ELSE (SELECT par.imie||' '||par.nazwisko FROM parafianie par WHERE par.pesel = p.matka)||' (PESEL: '||p.matka||')' END)) as matka,
	(SELECT s.data_udzielenia FROM sakramenty s JOIN chrzty c ON c.id_sakramentu = s.id WHERE c.osoba = p.pesel) as chrzest,
	(SELECT s.data_udzielenia FROM sakramenty s JOIN pierwsze_komunie pk ON pk.id_sakramentu = s.id WHERE pk.osoba = p.pesel) as pierwsza_komunia,
	(SELECT s.data_udzielenia FROM sakramenty s JOIN bierzmowania b ON b.id_sakramentu = s.id WHERE b.osoba = p.pesel) as bierzmowanie,
	(SELECT s.data_udzielenia FROM sakramenty s JOIN sluby sl ON sl.id_sakramentu = s.id WHERE sl.maz = p.pesel or sl.zona = p.pesel) as data_slubu,
	(SELECT CASE WHEN p.pesel = sl.maz THEN CASE WHEN sl.zona is null THEN sl.dane_zony||' (osoba spoza parafii)' ELSE (SELECT par.imie||' '||par.nazwisko FROM parafianie par WHERE par.pesel = sl.zona)||' (PESEL: '||sl.zona||')' END
		ELSE CASE WHEN sl.maz is null THEN sl.dane_meza||' (osoba spoza parafii)' ELSE (SELECT par.imie||' '||par.nazwisko FROM parafianie par WHERE par.pesel = sl.maz)||' (PESEL: '||sl.maz||')' END END 
		FROM sakramenty s JOIN sluby sl ON sl.id_sakramentu = s.id WHERE sl.maz = p.pesel or sl.zona = p.pesel) as malzonek,
	(SELECT pog.data_smierci FROM pogrzeby pog WHERE pog.osoba = p.pesel) as data_smierci,
	(SELECT pog.data_pogrzebu FROM pogrzeby pog WHERE pog.osoba = p.pesel) as pogrzeb
	FROM parafianie p;
	
BEGIN;

SELECT insert_chrzty ('31022213752', 'Lula', 'Edmund', date '1931-02-22', 'M', null, null, to_date('4 Jun 1931', 'DD Mon YYYY'), 'Edmund Mariusz', 'Stanislaw Lula', 'Karolina Jablecka');
SELECT insert_chrzty ('35041219490', 'Drawski', 'Lukasz', date '1935-02-12', 'M', null, null, to_date('10 Aug 1935', 'DD Mon YYYY'), 'Lukasz Michal', 'Stanislaw Barski', 'Alina Kowalska');
SELECT insert_chrzty ('35070604524', 'Skalska', 'Janina', date '1935-07-06', 'K', null, null, to_date('7 Sep 1935', 'DD Mon YYYY'), 'Janina Anna', 'Marek Skalski', 'Maria Jaworowska');
SELECT insert_chrzty ('49092610032', 'Wozniak', 'Stanislaw', date '1949-09-26', 'M', null, null, to_date('10 Dec 1949', 'DD Mon YYYY'), 'Stanislaw Boleslaw', 'Jan Wozniak', 'Zofia Biernacka');
SELECT insert_chrzty ('46021718566', 'Wlodarczyk', 'Boguslawa', date '1946-02-17', 'K', null, null, to_date('11 May 1946', 'DD Mon YYYY'), 'Boguslawa Irena', 'Marek Samborski', 'Sabina Decowska');
SELECT insert_komunie (date '1939-05-21', '31022213752');
SELECT insert_komunie (date '1944-05-05', '35041219490');
SELECT insert_komunie (date '1944-05-05', '35070604524');
SELECT insert_komunie (date '1958-05-24', '49092610032');
SELECT insert_komunie (date '1955-05-23', '46021718566');
SELECT insert_bierzmowania (date '1947-06-08', '31022213752', 'Marek', 'Anna Janicka');
SELECT insert_bierzmowania (date '1951-06-16', '35070604524', 'Dorota', 'Marek Jarecki');
SELECT insert_bierzmowania (date '1951-06-16', '35041219490', 'Jan', 'Karol Drawski');
SELECT insert_bierzmowania (date '1962-06-03', '46021718566', 'Dorota', 'Wanda Wlodarczyk');
SELECT insert_bierzmowania (date '1965-06-26', '49092610032', 'Jan', 'Maria Laba');
SELECT insert_sluby (date '1958-08-08', '31022213752', null, '35070604524', null, 'Halina Janik', 'Jan Kowalski');
SELECT insert_sluby (date '1970-04-22', '49092610032', null, '46021718566', null, 'Franciszek Rybka', 'Bernadeta Jasiel');
SELECT insert_chrzty ('62110917647', 'Kowalska', 'Krystyna', date '1962-11-09', 'K', null, null, date '1963-02-01', 'Krystyna Maria', 'Teofil Kobak', 'Leokadia Dobek');
SELECT insert_chrzty ('61042507098', 'Michalski', 'Jacek', date '1961-04-25', 'M', null, null, date '1961-08-22', 'Jacek Krzysztof', 'Piotr Karwowski', 'Marta Radosz');
SELECT insert_chrzty ('60031318886', 'Lula', 'Maria', date '1960-03-13', 'K', '31022213752', '35070604524', date '1960-06-24', 'Maria Julia', 'Leszek Lula', 'Beata Godek');
SELECT insert_chrzty ('74022217333', 'Wozniak', 'Piotr', date '1974-02-22', 'M', '49092610032', '46021718566', date '1974-06-26', 'Piotr Jakub', 'Mieczyslaw Wlodarczyk', 'Maria Wozniak');
SELECT insert_chrzty ('75062412296', 'Kowalski', 'Jan', to_date('24 Jun 1975', 'DD Mon YYYY'), 'M', null, null, to_date('10 Aug 1975', 'DD Mon YYYY'), 'Jan Andrzej', 'Krzysztof Rubaszko', 'Anna Janicka');
SELECT insert_chrzty ('72020815393', 'Iksinski', 'Piotr', to_date('8 Feb 1972', 'DD Mon YYYY'), 'M', null, null, to_date('10 May 1972', 'DD Mon YYYY'), 'Piotr Janusz', 'Marek Jarecki', 'Ilona Pawlicka');
SELECT insert_chrzty ('77101901503', 'Marecka', 'Anna', to_date('19 Oct 1977', 'DD Mon YYYY'), 'K', null, null, to_date('28 Dec 1977', 'DD Mon YYYY'), 'Anna Maria', 'Marek Jarecki', 'Anna Janicka');
SELECT insert_chrzty ('80030209312', 'Kolek', 'Jakub', to_date('2 Mar 1980', 'DD Mon YYYY'), 'M', null, null, to_date('10 Aug 1980', 'DD Mon YYYY'), 'Jakub Krzysztof', 'Jan Adamczyk', 'Anna Brzostek');
SELECT insert_chrzty ('79111418404', 'Wolska', 'Karolina', to_date('14 Nov 1979', 'DD Mon YYYY'), 'K', null, null, to_date('11 Feb 1980', 'DD Mon YYYY'), 'Karolina Krystyna', 'Stanislaw Barski', 'Ilona Pawlicka');
SELECT insert_komunie (to_date('11 May 1971', 'DD Mon YYYY'), '62110917647');
SELECT insert_komunie (to_date('20 May 1970', 'DD Mon YYYY'), '61042507098');
SELECT insert_komunie (to_date('24 May 1969', 'DD Mon YYYY'), '60031318886');
SELECT insert_komunie (to_date('2 May 1983', 'DD Mon YYYY'), '74022217333');
SELECT insert_komunie (to_date('24 May 1984', 'DD Mon YYYY'), '75062412296');
SELECT insert_komunie (to_date('20 May 1982', 'DD Mon YYYY'), '72020815393');
SELECT insert_komunie (to_date('27 May 1986', 'DD Mon YYYY'), '77101901503');
SELECT insert_komunie (to_date('21 May 1989', 'DD Mon YYYY'), '80030209312');
SELECT insert_komunie (to_date('17 May 1988', 'DD Mon YYYY'), '79111418404');
SELECT insert_bierzmowania (to_date('11 Jun 1978', 'DD Mon YYYY'), '62110917647', 'Anna', 'Anna Janicka');
SELECT insert_bierzmowania (to_date('24 Jun 1977', 'DD Mon YYYY'), '61042507098', 'Karol', 'Marek Jarecki');
SELECT insert_bierzmowania (to_date('17 Jun 1991', 'DD Mon YYYY'), '60031318886', 'Alina', 'Jolanta Malicka');
SELECT insert_bierzmowania (to_date('27 Jun 1988', 'DD Mon YYYY'), '74022217333', 'Karol', 'Anna Wozniak');
SELECT insert_bierzmowania (to_date('17 Jun 1991', 'DD Mon YYYY'), '75062412296', 'Marek', 'Anna Janicka');
SELECT insert_bierzmowania (to_date('27 Jun 1988', 'DD Mon YYYY'), '72020815393', 'Karol', 'Marek Jarecki');
SELECT insert_bierzmowania (to_date('12 Jun 1994', 'DD Mon YYYY'), '77101901503', 'Teresa', 'Ilona Pawlicka');
SELECT insert_bierzmowania (to_date('17 Jun 1996', 'DD Mon YYYY'), '80030209312', 'Jan', 'Anna Brzostek');
SELECT insert_bierzmowania (to_date('3 Jun 1995', 'DD Mon YYYY'), '79111418404', 'Teresa', 'Stanislaw Barski');
SELECT insert_sluby (to_date('14 Jun 1997', 'DD Mon YYYY'), '72020815393', null, '77101901503', null, 'Anna Janicka', 'Jan Kowalski');
SELECT insert_sluby (to_date('14 Feb 1999', 'DD Mon YYYY'), '80030209312', null, '79111418404', null, 'Krzysztof Kowalski', 'Anna Marecka');
SELECT insert_sluby (to_date('3 Sep 1995', 'DD Mon YYYY'), null, 'Marek Cyrkiel', '60031318886', null, 'Ilona Stankiewicz', 'Krzysztof Rubaszko');
SELECT insert_sluby (to_date('11 Jan 1999', 'DD Mon YYYY'), '74022217333', null, null, 'Malgorzata Sury', 'Marta Sapal', 'Kazimierz Skalka');
SELECT insert_chrzty ('01261203906', 'Cyrkiel', 'Emilia', to_date('12 Jun 2001', 'DD Mon YYYY'), 'K', null, '60031318886', to_date('24 Sep 2001', 'DD Mon YYYY'), 'Emilia Dorota', 'Anna Brzostek', 'Marek Jarecki');
SELECT insert_chrzty ('02262319168', 'Wozniak', 'Julia', to_date('23 Jun 2002', 'DD Mon YYYY'), 'K', '74022217333', null, to_date('11 Oct 2002', 'DD Mon YYYY'), 'Julia Marta', 'Stanislaw Barski', 'Ilona Pawlicka');
SELECT insert_chrzty ('06262505972', 'Wozniak', 'Bartosz', to_date('25 Jun 2006', 'DD Mon YYYY'), 'M', '74022217333', null, to_date('19 Oct 2006', 'DD Mon YYYY'), 'Bartosz Andrzej', 'Andrzej Wozniak', 'Marta Sury');
SELECT insert_chrzty ('99061104826', 'Kolek', 'Iwona', to_date('11 Jun 1999', 'DD Mon YYYY'), 'K', '80030209312', '77101901503', to_date('24 Sep 1999', 'DD Mon YYYY'), 'Iwona Anna', 'Marek Jarecki', 'Anna Brzostek');
SELECT insert_chrzty ('00282516372', 'Iksinski', 'Adam', to_date('25 Aug 2000', 'DD Mon YYYY'), 'M', '72020815393', '79111418404', to_date('11 Oct 2000', 'DD Mon YYYY'), 'Adam Karol', 'Stanislaw Barski', 'Ilona Pawlicka');
INSERT INTO pogrzeby values ('35041219490', to_date('3 Jun 2011', 'DD Mon YYYY'), to_date('5 Jun 2011', 'DD Mon YYYY'));
INSERT INTO pogrzeby values ('31022213752', null, date '2001-04-17');
INSERT INTO pogrzeby values ('35070604524', date '2013-05-11', date '2013-05-13');

COMMIT;