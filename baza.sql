--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2
-- Dumped by pg_dump version 17.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: kreiraj_obracun(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.kreiraj_obracun() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    ukupni_trosak NUMERIC(10, 2);
    projekt_id INT;
    posljednji_obracun INT;
BEGIN
    SELECT COUNT(*)
    INTO posljednji_obracun
    FROM Obracuni
    WHERE zaposlenik = NEW.zaposlenik;

    IF (SELECT COUNT(*) 
        FROM Radni_sati 
        WHERE zaposlenik = NEW.zaposlenik) % 5 = 0 
        AND posljednji_obracun < (SELECT COUNT(*) FROM Radni_sati WHERE zaposlenik = NEW.zaposlenik) / 5 THEN
        SELECT SUM(rs.odradjeni_sati * z.satnica)
        INTO ukupni_trosak
        FROM (
            SELECT * 
            FROM Radni_sati
            WHERE zaposlenik = NEW.zaposlenik
            ORDER BY id_log DESC
            LIMIT 5
        ) rs
        JOIN Zaposlenici z ON rs.zaposlenik = z.id_zaposlenik;

        SELECT projekt
        INTO projekt_id
        FROM Zadaci
        WHERE id_zadatak = NEW.zadatak;

        INSERT INTO Obracuni (zaposlenik, projekt, datum_obracuna, ukupni_trosak)
        VALUES (
            NEW.zaposlenik,
            projekt_id,
            CURRENT_DATE,
            ukupni_trosak
        );
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.kreiraj_obracun() OWNER TO postgres;

--
-- Name: ogranici_radne_sate(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.ogranici_radne_sate() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    ukupno_sati NUMERIC(10, 2);
BEGIN
    SELECT COALESCE(SUM(odradjeni_sati), 0)
    INTO ukupno_sati
    FROM (
        SELECT odradjeni_sati
        FROM Radni_sati
        WHERE zaposlenik = NEW.zaposlenik
        ORDER BY id_log DESC
        LIMIT 4
    ) AS posljednjih_unosa;

    IF ukupno_sati + NEW.odradjeni_sati > 40 THEN
        RAISE EXCEPTION 'Ukupni radni sati za posljednjih 5 unosa ne mogu biti veci od 40. Trenutno: %, Novi unos: %', ukupno_sati, NEW.odradjeni_sati;
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.ogranici_radne_sate() OWNER TO postgres;

--
-- Name: ogranici_unos_po_danu(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.ogranici_unos_po_danu() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    postoji_unos INT;
BEGIN
    SELECT COUNT(*)
    INTO postoji_unos
    FROM Radni_sati
    WHERE zaposlenik = NEW.zaposlenik AND datum = NEW.datum;

    IF postoji_unos > 0 THEN
        RAISE EXCEPTION 'Za zaposlenika % vec postoji unos za datum %', NEW.zaposlenik, NEW.datum;
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.ogranici_unos_po_danu() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: klijenti; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.klijenti (
    id_klijent integer NOT NULL,
    naziv_klijenta character varying(30) NOT NULL,
    kontakt_email character varying(30) NOT NULL
);


ALTER TABLE public.klijenti OWNER TO postgres;

--
-- Name: klijenti_id_klijent_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.klijenti_id_klijent_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.klijenti_id_klijent_seq OWNER TO postgres;

--
-- Name: klijenti_id_klijent_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.klijenti_id_klijent_seq OWNED BY public.klijenti.id_klijent;


--
-- Name: obracuni; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.obracuni (
    id_obracun integer NOT NULL,
    zaposlenik integer NOT NULL,
    projekt integer NOT NULL,
    datum_obracuna date NOT NULL,
    ukupni_trosak numeric(10,2) NOT NULL
);


ALTER TABLE public.obracuni OWNER TO postgres;

--
-- Name: obracuni_id_obracun_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.obracuni_id_obracun_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.obracuni_id_obracun_seq OWNER TO postgres;

--
-- Name: obracuni_id_obracun_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.obracuni_id_obracun_seq OWNED BY public.obracuni.id_obracun;


--
-- Name: projekti; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.projekti (
    id_projekt integer NOT NULL,
    naziv_projekta character varying(30) NOT NULL,
    datum_pocetka date NOT NULL,
    datum_zavrsetka date
);


ALTER TABLE public.projekti OWNER TO postgres;

--
-- Name: projekti_id_projekt_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.projekti_id_projekt_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.projekti_id_projekt_seq OWNER TO postgres;

--
-- Name: projekti_id_projekt_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.projekti_id_projekt_seq OWNED BY public.projekti.id_projekt;


--
-- Name: projekti_klijenata; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.projekti_klijenata (
    klijent integer NOT NULL,
    projekt integer NOT NULL
);


ALTER TABLE public.projekti_klijenata OWNER TO postgres;

--
-- Name: radni_sati; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.radni_sati (
    id_log integer NOT NULL,
    zaposlenik integer NOT NULL,
    zadatak integer NOT NULL,
    datum date NOT NULL,
    odradjeni_sati numeric(10,2) NOT NULL
);


ALTER TABLE public.radni_sati OWNER TO postgres;

--
-- Name: radni_sati_id_log_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.radni_sati_id_log_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.radni_sati_id_log_seq OWNER TO postgres;

--
-- Name: radni_sati_id_log_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.radni_sati_id_log_seq OWNED BY public.radni_sati.id_log;


--
-- Name: status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.status (
    id_status integer NOT NULL,
    tekst character varying(15)
);


ALTER TABLE public.status OWNER TO postgres;

--
-- Name: status_id_status_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.status_id_status_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.status_id_status_seq OWNER TO postgres;

--
-- Name: status_id_status_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.status_id_status_seq OWNED BY public.status.id_status;


--
-- Name: uloge; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.uloge (
    id_uloga integer NOT NULL,
    naziv character varying(20)
);


ALTER TABLE public.uloge OWNER TO postgres;

--
-- Name: uloge_id_uloga_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.uloge_id_uloga_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.uloge_id_uloga_seq OWNER TO postgres;

--
-- Name: uloge_id_uloga_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.uloge_id_uloga_seq OWNED BY public.uloge.id_uloga;


--
-- Name: uloge_zaposlenika; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.uloge_zaposlenika (
    zaposlenik integer NOT NULL,
    uloga integer NOT NULL
);


ALTER TABLE public.uloge_zaposlenika OWNER TO postgres;

--
-- Name: zadaci; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.zadaci (
    id_zadatak integer NOT NULL,
    projekt integer NOT NULL,
    naziv_zadatka character varying(30) NOT NULL,
    predvidjeni_sati numeric NOT NULL,
    status integer NOT NULL
);


ALTER TABLE public.zadaci OWNER TO postgres;

--
-- Name: zadaci_id_zadatak_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.zadaci_id_zadatak_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.zadaci_id_zadatak_seq OWNER TO postgres;

--
-- Name: zadaci_id_zadatak_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.zadaci_id_zadatak_seq OWNED BY public.zadaci.id_zadatak;


--
-- Name: zaposlenici; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.zaposlenici (
    id_zaposlenik integer NOT NULL,
    ime character varying(20) NOT NULL,
    prezime character varying(30) NOT NULL,
    pocetak_zaposlenja date NOT NULL,
    zavrsetak_zaposlenja date,
    satnica numeric(10,2) NOT NULL
);


ALTER TABLE public.zaposlenici OWNER TO postgres;

--
-- Name: zaposlenici_id_zaposlenik_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.zaposlenici_id_zaposlenik_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.zaposlenici_id_zaposlenik_seq OWNER TO postgres;

--
-- Name: zaposlenici_id_zaposlenik_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.zaposlenici_id_zaposlenik_seq OWNED BY public.zaposlenici.id_zaposlenik;


--
-- Name: klijenti id_klijent; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.klijenti ALTER COLUMN id_klijent SET DEFAULT nextval('public.klijenti_id_klijent_seq'::regclass);


--
-- Name: obracuni id_obracun; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.obracuni ALTER COLUMN id_obracun SET DEFAULT nextval('public.obracuni_id_obracun_seq'::regclass);


--
-- Name: projekti id_projekt; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projekti ALTER COLUMN id_projekt SET DEFAULT nextval('public.projekti_id_projekt_seq'::regclass);


--
-- Name: radni_sati id_log; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.radni_sati ALTER COLUMN id_log SET DEFAULT nextval('public.radni_sati_id_log_seq'::regclass);


--
-- Name: status id_status; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status ALTER COLUMN id_status SET DEFAULT nextval('public.status_id_status_seq'::regclass);


--
-- Name: uloge id_uloga; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.uloge ALTER COLUMN id_uloga SET DEFAULT nextval('public.uloge_id_uloga_seq'::regclass);


--
-- Name: zadaci id_zadatak; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zadaci ALTER COLUMN id_zadatak SET DEFAULT nextval('public.zadaci_id_zadatak_seq'::regclass);


--
-- Name: zaposlenici id_zaposlenik; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zaposlenici ALTER COLUMN id_zaposlenik SET DEFAULT nextval('public.zaposlenici_id_zaposlenik_seq'::regclass);


--
-- Data for Name: klijenti; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.klijenti (id_klijent, naziv_klijenta, kontakt_email) FROM stdin;
1	TechNova Solutions	kontakt@technova.com
2	GlobalSoft Ltd	info@globalsoft.com
3	InnovaTech	support@innovatech.io
4	FutureVision Inc	contact@futurevision.com
5	IT Dynamics	office@itdynamics.net
6	DataWare Systems	info@dataware.com
7	Smart Solutions Co.	sales@smartsolutions.co
8	NextGen Enterprises	contact@nextgenent.com
9	CloudTech Services	support@cloudtech.com
10	CyberSec Corp	security@cybersec.net
11	BlueSky Technologies	info@blueskytech.com
12	GreenField IT	support@greenfieldit.org
13	Quantum Innovations	contact@quantuminnovations.com
14	RedLine Solutions	sales@redlinesolutions.net
15	AlphaTech Partners	contact@alphatechpartners.com
\.


--
-- Data for Name: obracuni; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.obracuni (id_obracun, zaposlenik, projekt, datum_obracuna, ukupni_trosak) FROM stdin;
\.


--
-- Data for Name: projekti; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.projekti (id_projekt, naziv_projekta, datum_pocetka, datum_zavrsetka) FROM stdin;
1	Razvoj ERP sustava	2023-01-15	2023-06-30
2	Modernizacija web stranice	2022-11-01	2023-04-01
3	Migracija baze podataka	2023-03-01	2023-08-15
4	Implementacija CRM sustava	2022-09-10	2023-02-28
5	Sigurnosna analiza	2023-02-01	2023-05-31
6	Razvoj mobilne aplikacije	2024-01-01	\N
7	Uvodenje AI sustava	2024-03-01	\N
8	Optimizacija performansi	2024-02-15	\N
\.


--
-- Data for Name: projekti_klijenata; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.projekti_klijenata (klijent, projekt) FROM stdin;
1	3
2	6
5	5
7	8
3	1
13	2
10	4
9	7
\.


--
-- Data for Name: radni_sati; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.radni_sati (id_log, zaposlenik, zadatak, datum, odradjeni_sati) FROM stdin;
\.


--
-- Data for Name: status; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.status (id_status, tekst) FROM stdin;
1	Nije zapoceto
2	U tijeku
3	Dovrseno
\.


--
-- Data for Name: uloge; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.uloge (id_uloga, naziv) FROM stdin;
1	Izvrsni direktor
2	Visi menadzer
3	Menadzer
4	Ljudski resursi
5	Programer
6	Racunovoda
7	Ostalo
\.


--
-- Data for Name: uloge_zaposlenika; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.uloge_zaposlenika (zaposlenik, uloga) FROM stdin;
8	1
28	2
11	2
21	2
18	3
4	3
30	3
15	3
6	3
29	3
1	4
2	4
3	7
5	7
7	7
9	5
10	5
12	5
13	5
14	5
16	5
17	5
19	5
20	5
22	5
23	6
24	6
25	6
26	6
27	6
31	6
32	6
33	6
34	6
35	6
\.


--
-- Data for Name: zadaci; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.zadaci (id_zadatak, projekt, naziv_zadatka, predvidjeni_sati, status) FROM stdin;
1	1	Analiza zahtjeva	40	3
2	1	Dizajn sustava	50	3
3	2	Izrada prototipa	30	2
4	2	Implementacija frontenda	60	2
5	3	Migracija podataka	80	3
6	3	Testiranje sustava	40	2
7	4	Uvodenje u produkciju	20	2
8	5	Sigurnosno testiranje	25	3
9	6	Razvoj mobilne aplikacije	100	2
10	7	Optimizacija AI modela	50	2
\.


--
-- Data for Name: zaposlenici; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.zaposlenici (id_zaposlenik, ime, prezime, pocetak_zaposlenja, zavrsetak_zaposlenja, satnica) FROM stdin;
1	Ivan	Horvat	2020-01-15	\N	7.50
2	Ana	Kovac	2019-03-20	\N	8.00
3	Marko	Maric	2021-07-10	\N	6.50
5	Luka	Babic	2022-02-01	\N	7.00
6	Iva	Bozic	2020-11-30	\N	8.50
7	Karlo	Juric	2019-04-12	\N	7.20
8	Marija	Peric	2017-08-20	\N	10.00
9	Nikola	Radic	2021-01-01	\N	6.80
10	Ema	Vukovic	2023-03-15	\N	7.00
11	Toma	Matic	2016-05-25	\N	9.50
12	Lea	Simic	2022-07-01	\N	8.00
13	Filip	Lovric	2020-09-10	\N	7.50
14	Klara	Sertic	2021-11-20	\N	8.20
15	Sara	Grgic	2019-12-01	\N	8.70
16	Ante	Varga	2020-10-05	\N	6.50
17	Ivana	Bilic	2022-03-25	\N	7.30
18	Tomislav	Sarac	2015-06-15	\N	9.00
19	Maja	Barbir	2019-07-11	\N	8.10
20	Kristina	Rajic	2020-04-22	\N	7.80
21	Ivan	Petkovic	2018-03-01	\N	9.20
22	Andrej	Krpan	2021-02-15	\N	6.90
23	Dora	Bogdan	2022-06-10	\N	7.40
24	Leon	Spoljaric	2023-01-05	\N	7.00
25	Nina	Dzidic	2021-10-12	\N	7.90
26	Hrvoje	Saric	2020-08-25	\N	6.80
27	Tanja	Knezevic	2023-05-20	\N	7.50
28	Anamarija	Jukic	2017-11-15	\N	9.80
29	Dino	Babic	2018-09-05	\N	8.40
30	Helena	Majstorovic	2019-01-20	\N	8.70
31	Robert	Kralj	2020-12-01	\N	7.10
32	Andrea	Oreskovic	2021-05-10	\N	8.00
33	Emanuel	Markovic	2022-08-25	\N	7.60
34	Tamara	Vidovic	2023-02-11	\N	7.30
35	Fran	Prpic	2022-10-01	\N	7.80
36	Marijan	Ban	2022-02-14	2024-12-25	88.00
4	Petra	Novak	2018-09-05	\N	72.00
\.


--
-- Name: klijenti_id_klijent_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.klijenti_id_klijent_seq', 15, true);


--
-- Name: obracuni_id_obracun_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.obracuni_id_obracun_seq', 8, true);


--
-- Name: projekti_id_projekt_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.projekti_id_projekt_seq', 8, true);


--
-- Name: radni_sati_id_log_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.radni_sati_id_log_seq', 19, true);


--
-- Name: status_id_status_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.status_id_status_seq', 3, true);


--
-- Name: uloge_id_uloga_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.uloge_id_uloga_seq', 7, true);


--
-- Name: zadaci_id_zadatak_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.zadaci_id_zadatak_seq', 10, true);


--
-- Name: zaposlenici_id_zaposlenik_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.zaposlenici_id_zaposlenik_seq', 36, true);


--
-- Name: klijenti klijenti_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.klijenti
    ADD CONSTRAINT klijenti_pkey PRIMARY KEY (id_klijent);


--
-- Name: obracuni obracuni_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.obracuni
    ADD CONSTRAINT obracuni_pkey PRIMARY KEY (id_obracun);


--
-- Name: projekti_klijenata projekti_klijenata_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projekti_klijenata
    ADD CONSTRAINT projekti_klijenata_pkey PRIMARY KEY (klijent, projekt);


--
-- Name: projekti projekti_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projekti
    ADD CONSTRAINT projekti_pkey PRIMARY KEY (id_projekt);


--
-- Name: radni_sati radni_sati_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.radni_sati
    ADD CONSTRAINT radni_sati_pkey PRIMARY KEY (id_log);


--
-- Name: status status_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_pkey PRIMARY KEY (id_status);


--
-- Name: uloge uloge_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.uloge
    ADD CONSTRAINT uloge_pkey PRIMARY KEY (id_uloga);


--
-- Name: uloge_zaposlenika uloge_zaposlenika_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.uloge_zaposlenika
    ADD CONSTRAINT uloge_zaposlenika_pkey PRIMARY KEY (zaposlenik, uloga);


--
-- Name: zadaci zadaci_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zadaci
    ADD CONSTRAINT zadaci_pkey PRIMARY KEY (id_zadatak);


--
-- Name: zaposlenici zaposlenici_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zaposlenici
    ADD CONSTRAINT zaposlenici_pkey PRIMARY KEY (id_zaposlenik);


--
-- Name: radni_sati trig_kreiraj_obracun; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trig_kreiraj_obracun AFTER INSERT ON public.radni_sati FOR EACH ROW EXECUTE FUNCTION public.kreiraj_obracun();


--
-- Name: radni_sati trig_ogranici_radne_sate; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trig_ogranici_radne_sate BEFORE INSERT OR UPDATE ON public.radni_sati FOR EACH ROW EXECUTE FUNCTION public.ogranici_radne_sate();


--
-- Name: radni_sati trig_ogranici_unos_po_danu; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trig_ogranici_unos_po_danu BEFORE INSERT OR UPDATE ON public.radni_sati FOR EACH ROW EXECUTE FUNCTION public.ogranici_unos_po_danu();


--
-- Name: obracuni obracuni_projekt_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.obracuni
    ADD CONSTRAINT obracuni_projekt_fkey FOREIGN KEY (projekt) REFERENCES public.projekti(id_projekt);


--
-- Name: obracuni obracuni_zaposlenik_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.obracuni
    ADD CONSTRAINT obracuni_zaposlenik_fkey FOREIGN KEY (zaposlenik) REFERENCES public.zaposlenici(id_zaposlenik);


--
-- Name: projekti_klijenata projekti_klijenata_klijent_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projekti_klijenata
    ADD CONSTRAINT projekti_klijenata_klijent_fkey FOREIGN KEY (klijent) REFERENCES public.klijenti(id_klijent);


--
-- Name: projekti_klijenata projekti_klijenata_projekt_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projekti_klijenata
    ADD CONSTRAINT projekti_klijenata_projekt_fkey FOREIGN KEY (projekt) REFERENCES public.projekti(id_projekt);


--
-- Name: radni_sati radni_sati_zadatak_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.radni_sati
    ADD CONSTRAINT radni_sati_zadatak_fkey FOREIGN KEY (zadatak) REFERENCES public.zadaci(id_zadatak);


--
-- Name: radni_sati radni_sati_zaposlenik_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.radni_sati
    ADD CONSTRAINT radni_sati_zaposlenik_fkey FOREIGN KEY (zaposlenik) REFERENCES public.zaposlenici(id_zaposlenik);


--
-- Name: uloge_zaposlenika uloge_zaposlenika_uloga_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.uloge_zaposlenika
    ADD CONSTRAINT uloge_zaposlenika_uloga_fkey FOREIGN KEY (uloga) REFERENCES public.uloge(id_uloga);


--
-- Name: uloge_zaposlenika uloge_zaposlenika_zaposlenik_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.uloge_zaposlenika
    ADD CONSTRAINT uloge_zaposlenika_zaposlenik_fkey FOREIGN KEY (zaposlenik) REFERENCES public.zaposlenici(id_zaposlenik);


--
-- Name: zadaci zadaci_projekt_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zadaci
    ADD CONSTRAINT zadaci_projekt_fkey FOREIGN KEY (projekt) REFERENCES public.projekti(id_projekt);


--
-- Name: zadaci zadaci_status_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zadaci
    ADD CONSTRAINT zadaci_status_fkey FOREIGN KEY (status) REFERENCES public.status(id_status);


--
-- PostgreSQL database dump complete
--

