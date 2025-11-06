# RoomMate Finder – Documentație Tehnică

Team: Stativa Darius, Grigorovici Tudor, Gociu Radu, Popovici Iulian Cosmin

## 🧩 Prezentare generală

RoomMate Finder este o platformă inteligentă care îi ajută pe studenți să își găsească colegi de cameră compatibili, bazată pe stil de viață, preferințe și personalitate. Aplicația gestionează profiluri, potriviri, conversații, anunțuri de camere și recenzii, oferind o experiență completă de conectare și încredere între utilizatori.

---

## ⚙️ Tehnologii utilizate

* **Backend:** .NET 8 Minimal API
* **Arhitectură:** Vertical Slice Architecture (CQRS + MediatR)
* **Bază de date:** PostgreSQL (producție) 
* **Validare:** FluentValidation
* **Testare:** xUnit (unit tests), NSubstitute (integration tests)
* **Frontend (opțional):** Blazor WebAssembly pentru interfață client modernă

---

## 🧱 Arhitectură și structură

Proiectul urmează principiile arhitecturii **Vertical Slice**, în care fiecare funcționalitate este separată într-un modul propriu. Această abordare permite o dezvoltare independentă, testare clară și menținerea unei structuri curate.

```
RoomMateFinder/
├─ Api/                → Aplicația principală (.NET Minimal API)
├─ Application/        → Features (CQRS + MediatR)
├─ Domain/             → Entități, Value Objects, logica de domeniu
├─ Infrastructure/     → EF Core, DbContext, configurații
└─ Tests/              → Teste unitare și de integrare
```

Fiecare feature conține propriile comenzi, query‑uri, validatori și endpoint‑uri:

```
Features/
├── Profiles/
│   ├── CreateProfile/
│   ├── UpdateProfile/
│   ├── GetMyProfile/
│   ├── GetProfileById/
│   └── CompleteOnboarding/
├── Matching/
│   ├── GetMatches/
│   ├── CalculateCompatibility/
│   ├── LikeProfile/
│   └── PassProfile/
├── Conversations/
│   ├── StartConversation/
│   ├── SendMessage/
│   ├── GetConversations/
│   └── GetMessages/
├── RoomListings/
│   ├── CreateListing/
│   ├── UpdateListing/
│   ├── SearchListings/
│   └── GetListingById/
└── Reviews/
    ├── CreateReview/
    ├── GetUserReviews/
    └── GetReviewStats/
```

---

## 🧠 Descrierea principalelor funcționalități

### 1. Profile Management

Utilizatorii își pot crea și gestiona profilul complet:

* **CreateProfile:** înregistrarea inițială cu informații personale și chestionar despre stilul de viață.
* **UpdateProfile:** modificarea preferințelor, fotografiilor și datelor personale.
* **GetMyProfile:** vizualizarea propriului profil și a statisticilor de potrivire.
* **GetProfileById:** afișarea profilului altui utilizator.
* **CompleteOnboarding:** finalizarea procesului de onboarding pas cu pas.

### 2. Matching Algorithm

Sistemul de potrivire calculează un scor de compatibilitate între utilizatori folosind criterii precum:

* obiceiuri de viață (somn, curățenie, fumat, animale);
* preferințe de buget și distanță;
* răspunsuri la un quiz de personalitate.

Funcționalități:

* **GetMatches:** returnează lista utilizatorilor compatibili.
* **CalculateCompatibility:** calculează scorul de compatibilitate între doi utilizatori.
* **LikeProfile / PassProfile:** interacțiuni de tip „swipe left/right” pentru potrivire.

### 3. Conversations & Messaging

După o potrivire reciprocă, utilizatorii pot comunica în siguranță prin sistemul intern de mesagerie:

* **StartConversation:** inițierea unei conversații între doi utilizatori potriviți.
* **SendMessage:** trimiterea de mesaje.
* **GetConversations / GetMessages:** afișarea listelor de conversații și a istoricului de mesaje.

### 4. Room Listings

Platforma permite publicarea și căutarea anunțurilor de camere disponibile:

* **CreateListing:** crearea unui anunț de închiriere.
* **UpdateListing:** editarea unui anunț existent.
* **SearchListings:** filtrarea anunțurilor după locație, preț, facilități.
* **GetListingById:** detalii complete ale unui anunț.

### 5. Reviews & Trust System

După o perioadă de coabitare, utilizatorii pot lăsa recenzii și ratinguri pentru colegii lor:

* **CreateReview:** adăugarea unei recenzii.
* **GetUserReviews:** vizualizarea recenziilor primite.
* **GetReviewStats:** calcularea mediei și afișarea scorurilor de încredere.

---

## 💾 Bază de date și model de date

Aplicația folosește **Entity Framework Core** pentru maparea entităților în PostgreSQL.

### Entități principale

* **User:** informații de autentificare și identitate.
* **Profile:** detalii personale, preferințe, stil de viață, poze.
* **Match:** perechi de utilizatori cu scor de compatibilitate.
* **Conversation / Message:** structură de mesagerie.
* **RoomListing:** anunțuri de camere disponibile.
* **Review:** evaluări și feedback între utilizatori.

Toate entitățile includ atribute de audit (CreatedAt, UpdatedAt) și relațiile corespunzătoare (1‑la‑1, 1‑la‑n, n‑la‑n acolo unde este cazul).

---

## 🧩 API – Exemplu de endpoint

```csharp
app.MapPost("/profiles", async (CreateProfileCommand cmd, IMediator mediator) =>
{
    var result = await mediator.Send(cmd);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
});
```

Acest model este folosit în toate slice‑urile, cu validare FluentValidation și procesare prin MediatR.

---

## 🧪 Testare

* **xUnit:** teste unitare pentru Handlers și Validators.
* **NSubstitute:** mock‑uri pentru dependențe și teste de integrare (bază de date in‑memory).

Exemplu:

```csharp
[Fact]
public async Task Should_Create_Profile_When_Valid()
{
    var handler = new CreateProfileHandler(...);
    var command = new CreateProfileCommand("John", "Student", ...);
    var result = await handler.Handle(command, CancellationToken.None);
    Assert.True(result.IsSuccess);
}
```

---

## 🗓️ Etape de dezvoltare

| Săptămâna | Activități principale                            |
| --------- | ------------------------------------------------ |
| 1         | Configurare arhitectură, soluție și bază de date |
| 2         | Implementare Profiles + validare                 |
| 3         | Matching Algorithm + logica de compatibilitate   |
| 4         | Conversations & Messaging                        |
| 5         | Room Listings + filtre de căutare                |
| 6         | Reviews + sistem de încredere                    |
| 7         | Testare completă, îmbunătățiri UI                |
| 8         | Documentație finală și prezentare                |

---

## 🎯 Concluzie

RoomMate Finder este o aplicație completă, modulară, bazată pe o arhitectură curată, concepută pentru a conecta studenții și a le oferi un mediu sigur pentru a‑și găsi colegi de cameră potriviți. Platforma pune accent pe experiența utilizatorului, pe siguranță și pe potriviri relevante generate printr-un algoritm de compatibilitate inteligent.
