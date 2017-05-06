Instalacja programu:
0. Przygotowanie środowiska.
1. 

Ad. 0

Development aplikacji prowadzę na VS2015 Community Edition, wiem że na nim działa. Janek z tego co wiem robi na 2012, co do Karola - nie mam pojęcia :P.
Wymagane pakiety powinny być w pliku projektu zawarte i VS powinien dociągnąć je automatycznie. Jeżeli tak nie jest, to do działania program wymaga pakietów:
0. IKVM
1. TikaOnDotnet
2. TikeOnDotnet.TextExtractor
3. FirebirdSql.Data.FirebirdClient
Instaluje się je w VS z poziomu NuGet'a (Project>Manage NuGet Packages).

Dalej, jako że mamy już bazę danych wymagany jest już serwer bazodanowy. Stąd należy:
0. Zainstalować serwer bazodanowy - wersję 3.0.2 x64 Windows z strony 
https://www.firebirdsql.org/en/downloads/
Podczas instalacji wszystko zrobić po domyślnych konfiguracjach.
Instalator w pewnym momencie poprosi was o hasło dla użytkownika SYSDBA, wybierzcie prosze jakies.
1. Po zainstalowaniu trzeba jeszcze zejść do katalogu Firebird/Firebird_3_0/ (powinien być w Program Files) i w pliku firebird.conf dodać linijkę:
WireCrypt = Enabled
Wiąże się to z faktem że nasz provider niestety nie supportuje jeszcze nowej funkcji szyfrowania połączenia WireCrypt, stąd musimy ją ustawić na Enabled (czyt. użyj jeżeli możesz), a nie Required ustawiany domyślnie.
2. Po tym wszystkim należy zrestartować usługę Firebirda:
Prawy przycisk myszy na Komputer>Zarządzaj>Usługi i aplikacje>Usługi>Firebird Server - DefaultInstance 
Prawym przyciskiem myszy w to i Zatrzymaj, poczekajcie aż się zatrzyma, potem tak samo prawy i uruchom
(mam system po angielsku i tłumacze w locie, więc nazwy mogą się troszkę różnić)

Po tych operacjach pozostaje jeszcze upewnić się, że projekt NIE JEST w katalogu wymagającym uprawnień administratora do modyfikacji plików (ja mam swoją roboczą wersję zaraz na dysku C:\, potem można dorobić mały plik konfiguracyjny w którym przechowujemy lokację poszczególnych rzeczy na dysku).

Ostatnie to uzupełnić w kodzie dla Form1.cs hasło połączenia do użytkownika SYSDBA - w handlerze BT_test_database_click są dwa łańcuchy tekstu wymagające wpisania hasła które ustawialiście przy instalacji Firebirda, są okomentowane gdzie i jak.

Gdy program uruchomi się poprawnie zauważycie trzy nowe rzeczy - button1 został przejęty (robie w nim Test bazy - chwilowo tylko stworzenie jej gdy nie istnieje lub wykonanie pustego połączenia gdy istnieje), dodałem nowy przycisk (na razie pusty, chce w nim zrobić wybór wielu katalogów do katalogowania) i jeden textfield (używam go do debugowania kilku rzeczy).
