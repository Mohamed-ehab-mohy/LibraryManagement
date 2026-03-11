# 📚 City Library Management System

A console-based library management system built with **C# (.NET)** that handles all day-to-day operations of a library branch — from registering members and borrowing books to tracking fines and generating reports.

---

## 🗂️ Project Structure

```
LibraryManagement/
├── Contracts/
│   ├── IBorrowable.cs        # Interface for borrowable entities
│   ├── IDisplayable.cs       # Interface for displayable entities
│   └── IValidatable.cs       # Interface for validatable entities
├── DataSeeder/
│   └── Seeder.cs             # Seeds the branch with initial data
├── Entities/
│   ├── Book.cs               # Book model (ISBN, title, author, genre, year)
│   ├── BookCopy.cs           # Physical copy of a book (sealed)
│   ├── BorrowTransaction.cs  # Borrow/return record (sealed)
│   ├── Librarian.cs          # Librarian user
│   ├── LibraryBranch.cs      # Core branch — owns all members and copies
│   ├── Member.cs             # Library member with loyalty tier
│   └── User.cs               # Abstract base for Librarian and Member
├── Enums/
│   ├── BookCondition.cs      # New / Good / Worn
│   ├── CopyStatus.cs         # Available / Borrowed / Damaged
│   ├── LoyaltyTier.cs        # None / Bronze / Silver / Gold
│   └── TransactionStatus.cs  # Active / Returned / Overdue
├── Helpers/
│   ├── ConsoleHelper.cs      # Renders the main menu
│   ├── FineCalculator.cs     # Fine calculation logic
│   ├── IDGenerator.cs        # Auto-generates member IDs
│   ├── InputValidator.cs     # Input validation helpers
│   └── OperationResult.cs    # Generic result wrapper
├── Services/
│   ├── DisplayService.cs     # All console output and screen rendering
│   └── LibraryService.cs     # Handles user input and orchestrates operations
├── StringExtensions.cs       # Extension methods (validation, masking, formatting)
└── Program.cs                # Entry point — main loop
```

---

## ✨ Features

### 1 — View Branch Information
Displays the full profile of the library branch including Branch ID, name, address, phone, opening hours, manager name, total registered members, and total book copies. Member and copy counts update automatically as data changes.

---

### 2 — Register a New Member
Allows the librarian to register a new member by entering their full name, phone number, and an optional email address.

- **Name** is automatically converted to Title Case (`ahmed kamal` → `Ahmed Kamal`)
- **Member ID** is auto-generated in the format `MEM-001`, `MEM-002`, etc.
- **Membership date** is set automatically to today
- **Phone** is always displayed masked — digits 5–8 replaced with `****` (e.g. `01098765432` → `0109****432`)
- New members start with **Tier = None** and **Borrow Count = 0**

**Validation rules:**
| Field | Rule |
|-------|------|
| Name | Letters and spaces only, 3–50 characters |
| Phone | Exactly 11 digits, starts with 010 / 011 / 012 / 015 |
| Phone | Must be unique — duplicate phone rejected |
| Email | Optional. If provided, must contain `@` and a dot after it |

---

### 3 — View All Users
Displays the full profile of the librarian first, followed by a profile card for every registered member. Each member card shows their ID, masked phone, email (or `N/A`), join date, all-time borrow count, and current loyalty tier. If no members are registered yet, an info message is shown after the librarian profile.

---

### 4 — View Available Books
Shows a table of all book copies currently available for borrowing — Copy ID, book title, and condition — along with the total available count. Borrowed and damaged copies are excluded.

---

### 5 — Borrow a Book
Processes a new borrow for a member.

1. Librarian enters the Member ID → system shows member name, active borrow count, and loyalty tier
2. Available copies table is displayed
3. Librarian enters a Copy ID → transaction is created

**Business rules:**
- Copy must have status `Available` (both `Borrowed` and `Damaged` are blocked)
- Member cannot have more than **3 active borrows** simultaneously
- Member cannot borrow a copy they already have on an active loan
- Due date is always **borrow date + 14 days** (not configurable)
- Transaction is added to the member's history and their all-time borrow count increases by 1
- Loyalty tier is re-evaluated after every successful borrow

---

### 6 — Return a Book
Processes a return using the Copy ID only — the member is identified automatically from the active transaction.

- Copy status changes back to `Available`
- Return date is set to today
- **Fine = days overdue × 10 EGP** (zero if returned on time)
- If the member has a loyalty tier, a discount is applied to the fine automatically:

| Tier | Discount |
|------|----------|
| 🥇 Gold (10+ borrows) | 20% off fine |
| 🥈 Silver (5–9 borrows) | 10% off fine |
| 🥉 Bronze (3–4 borrows) | 5% off fine |
| None (0–2 borrows) | No discount |

- On-time return → status = `Returned`, fine = `None`
- Late return → status = `Overdue`, fine breakdown shown with original fine, discount, and final amount

---

### 7 — View Borrowing History
Displays the full borrow history for a given Member ID — sorted chronologically (oldest first). Each record shows the transaction ID, book title, copy ID, borrow date, due date, return date, status, and fine. Active transactions are highlighted in yellow. Discount notes are included in the fine field when applicable (e.g. `40.00 EGP (after Gold discount)`).

---

### 8 — Top Borrowers & Loyalty Report
Displays all members ranked by their all-time borrow count (highest first). Ties are broken alphabetically by name. Each row shows rank, name, borrow count, and loyalty tier — color-coded (gold / grey / bronze). A summary at the bottom shows counts per tier including tiers with zero members.

**Tier thresholds (computed live, never stored):**
```
10+ borrows  →  🥇 Gold    →  20% fine discount
5–9  borrows →  🥈 Silver  →  10% fine discount
3–4  borrows →  🥉 Bronze  →   5% fine discount
0–2  borrows →     None    →   no discount
```

---

### 9 — Overdue Alert (Automatic)
Every time the main menu is rendered, the system automatically checks all active transactions. If any copy is past its due date, a red warning banner appears above the menu listing each overdue copy — Copy ID, book title, member name, and days overdue. If everything is on time, the menu appears cleanly with no banner.

---

### 10 — Search Member by Name
Searches for members using a partial, case-insensitive name match. Input `ahmed` matches `Ahmed Kamal`, `Ahmed Salem`, etc. All matching member profiles are displayed in full, separated by dividers, with a result count header showing the search term and number of matches found.

---

### 11 — Inventory Report
Displays a full snapshot of the book collection grouped by title. Each row shows the count of Available, Borrowed, and Damaged copies for that title, plus a total. A color-coded totals row at the bottom sums all columns (green = available, yellow = borrowed, red = damaged).

---

### 12 — Fine Summary Report
Lists all transactions with outstanding fines, ranked by fine amount (highest first). Ties are broken alphabetically by member name. Each row shows rank, member name, transaction ID, book title, and the final fine amount after any loyalty discount. The summary footer shows the total outstanding fines and the number of distinct members with fines.

---

## 🛡️ Validation & Error Handling

All user input is validated before any operation is performed. The system uses extension methods in `StringExtensions.cs` and throws `InvalidOperationException` for all business-rule violations, which are caught at the top-level loop in `Program.cs` and displayed as red error messages.

| Message Style | Color | Prefix |
|---------------|-------|--------|
| Success | Green | `✔` |
| Error | Red | `✘` |
| Warning | Yellow | `⚠` |
| Info | Magenta | `ℹ` |

---

## 🚀 Getting Started

**Requirements:** .NET 10 SDK or later

```bash
# Clone the repo
git clone https://github.com/Mohamed-ehab-mohy/LibraryManagement.git
cd LibraryManagement

# Run the project
dotnet run
```

The app seeds a branch with 3 members, 5 books, and 8 copies automatically on startup.

---

## 🧩 Design Highlights

- **`LibraryBranch`** is the central aggregate — it owns all members and copies and enforces all business rules
- **`BookCopy`** is `sealed` — the physical copy is the borrowable unit, not the book itself
- **`BorrowTransaction`** is `sealed` and immutable after creation; only `CompleteReturn()` mutates it
- **Loyalty tier** is a computed property on `Member` — never stored, always derived from `BorrowCount`
- **`DisplayService`** and **`LibraryService`** are fully separated — display logic never touches business logic
- **`StringExtensions`** centralizes all validation (phone, email, name, ID formats) as readable extension methods
- **`DataSeeder`** bypasses registration validation via `SeedMember()` to allow seeding pre-existing historical data

---

## 📋 Seeded Data

| Type | Details |
|------|---------|
| Branch | City Library — Nasr City Branch, BR-01 |
| Manager | Sara Ahmed (LIB-01) |
| Members | Ahmed Kamal, Nour Hassan, Omar Fathy |
| Books | Clean Code, The Pragmatic Programmer, Design Patterns, Atomic Habits, Sapiens |
| Copies | 8 copies total (COPY-001 to COPY-008), including 1 damaged copy |
