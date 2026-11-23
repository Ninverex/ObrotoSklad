# ObrotoSkład

**ObrotoSkład** to aplikacja w .NET wspierająca obsługę produktów, klientów, stanów magazynowych, zamówień i faktur. System łączy proces sprzedaży z gospodarką magazynową: od utworzenia produktu i przyjęcia dostawy, przez rezerwację towaru, po realizację zamówienia i wygenerowanie faktury.

Repozytorium zawiera backend w **ASP.NET Core 10** oraz frontend w **Angular 22**. Dane aplikacji są przechowywane w **PostgreSQL**.

## Spis treści

1. [Funkcjonalności](#funkcjonalności)
2. [Technologie](#technologie)
3. [Architektura i struktura repozytorium](#architektura-i-struktura-repozytorium)
4. [Role użytkowników](#role-użytkowników)
5. [Proces obsługi zamówienia](#proces-obsługi-zamówienia)


## Funkcjonalności

### Użytkownicy i autoryzacja

- Rejestracja konta oraz logowanie za pomocą adresu e-mail i hasła.
- Uwierzytelnianie żądań z wykorzystaniem tokenów JWT.
- Obsługa ról: `Admin`, `Handlowiec` i `Magazynier`.
- Automatyczne przypisanie roli `Handlowiec` podczas rejestracji.
- Przechowywanie informacji o zalogowanej sesji po stronie przeglądarki.

### Produkty i klienci

- Dodawanie, wyświetlanie, aktualizacja i usuwanie produktów.
- Ewidencja produktów z identyfikatorem SKU, nazwą, opisem, jednostką i ceną.
- Dodawanie, wyświetlanie, aktualizacja i usuwanie klientów.
- Przechowywanie danych klientów, w tym numeru NIP.

### Magazyn

- Podgląd ilości towaru na stanie, ilości zarezerwowanej i dostępnej.
- Przyjmowanie towaru do magazynu.
- Rezerwacja oraz zwalnianie rezerwacji produktów.
- Wydawanie produktów z magazynu.
- Przegląd historii ruchów magazynowych z datą, rodzajem operacji i powiązanym zamówieniem.

### Zamówienia i faktury

- Tworzenie zamówień powiązanych z klientami i produktami.
- Potwierdzanie zamówień i rezerwowanie wymaganych ilości towaru.
- Realizacja zamówień po zarezerwowaniu towaru.
- Anulowanie zamówień zgodnie z regułami biznesowymi.
- Automatyczne generowanie faktury po zrealizowaniu zamówienia.
- Przeglądanie listy faktur oraz szczegółów pojedynczej faktury.

## Technologie

| Warstwa | Technologie |
| --- | --- |
| Frontend | Angular 22, TypeScript 6, RxJS, Angular Router, Angular Forms |
| Interfejs | CSS, Tailwind CSS 4 |
| Backend | C# i ASP.NET Core 10 (Web API) |
| Baza danych | PostgreSQL |
| Dostęp do danych | Entity Framework Core 10, Npgsql |
| Konta i uprawnienia | ASP.NET Core Identity, JWT Bearer Authentication |
| Walidacja | FluentValidation |
| Testy frontendu | Vitest / Angular test builder |
| Narzędzia | .NET SDK, Node.js, npm, Git |

## Architektura i struktura repozytorium

Backend jest podzielony na cztery projekty odpowiadające za API, logikę aplikacyjną, model domenowy i infrastrukturę. Frontend komunikuje się z backendem przez HTTP API.

```text
ObrotoSklad/
├── backend/
│   └── src/
│       ├── ObrotoSklad.Api/              # Kontrolery HTTP, konfiguracja i uruchamianie
│       ├── ObrotoSklad.Application/      # Usługi aplikacyjne, DTO i reguły biznesowe
│       ├── ObrotoSklad.Domain/           # Encje i model domenowy
│       └── ObrotoSklad.Infrastructure/   # EF Core, PostgreSQL i dostęp do danych
├── frontend/
│   ├── src/
│   │   └── app/
│   │       ├── core/                    # Uwierzytelnianie i konfiguracja
│   │       └── features/                # Moduły funkcjonalne aplikacji
│   ├── angular.json
│   └── package.json
└── .gitignore
```

Przepływ wywołania jest następujący:

```text
Przeglądarka (Angular)
        |
        v
REST API (ASP.NET Core)
        |
        v
Usługi aplikacyjne
        |
        v
Entity Framework Core
        |
        v
PostgreSQL
```

W bazie znajdują się między innymi encje `Product`, `Customer`, `StockItem`, `StockMovement`, `Order`, `OrderItem`, `Invoice` oraz `InvoiceItem`, a także tabele tożsamości ASP.NET Core Identity.

## Role użytkowników

| Rola | Przeznaczenie |
| --- | --- |
| `Admin` | Zarządzanie operacjami wymagającymi uprawnień administracyjnych |
| `Handlowiec` | Tworzenie i potwierdzanie zamówień |
| `Magazynier` | Realizacja zamówień i obsługa procesów magazynowych |

Backend wymaga roli `Handlowiec` lub `Admin` przy tworzeniu i potwierdzaniu zamówień oraz roli `Magazynier` lub `Admin` przy ich realizacji. Odczyt zamówień i faktur wymaga zalogowania. Szczegółowe uprawnienia pozostałych operacji należy sprawdzić w atrybutach kontrolerów; nie wszystkie endpointy mają obecnie ograniczenia dostępu.

## Proces obsługi zamówienia

1. Użytkownik dodaje produkty oraz klientów.
2. Towar jest przyjmowany do magazynu, co zwiększa jego stan.
3. Handlowiec tworzy zamówienie przypisane do klienta, zawierające produkty i ich ilości.
4. Potwierdzenie zamówienia uruchamia rezerwację wymaganych produktów.
5. Magazynier realizuje zamówienie: towar zostaje wydany, a stan magazynowy jest aktualizowany.
6. Po realizacji system generuje fakturę powiązaną z zamówieniem.

Anulowanie potwierdzonego zamówienia zwalnia rezerwacje. Zamówienia już zrealizowanego lub anulowanego nie można anulować ponownie.

