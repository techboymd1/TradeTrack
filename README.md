# TradeTrack

**Business Management System**

TradeTrack is a web-based business management application built with ASP.NET Core Razor Pages. It brings product and inventory management, sales, customer records, expense tracking and business reporting into one workspace.

## Core capabilities

### Overview
- Business summary with sales recorded today, active products with available stock, monthly expenses and estimated net profit.
- Inventory health indicators and direct access to the main business modules.

### Products & inventory
- Create, view and edit product records.
- Maintain SKUs, categories, units, cost prices, selling prices and stock quantities.
- Search the catalogue and filter by category or stock status.
- Identify in-stock, low-stock, out-of-stock and inactive products.
- Deactivate products without deleting their records.

### Sales
- Record transactions containing multiple products.
- Select a registered customer or record a walk-in sale.
- Calculate line totals, subtotals, discounts and sale totals.
- Validate product availability before completing a transaction.
- Save invoice details and item-level price and cost snapshots.
- Deduct sold quantities from inventory as part of the sale transaction.
- Review recorded sales and customer details in sales history.

### Customers
- Create and update customer records.
- Store contact details and notes.
- Activate or deactivate customer records.
- Associate sales with registered customers while retaining support for walk-in sales.

### Expenses
- Record business expenses with categories, dates, amounts and payment methods.
- Store descriptions, references and notes.
- Edit existing expense records.
- Review expense history and spending totals.

### Reports
- Filter sales and expense information by date range.
- Review revenue, cost of goods sold, business expenses, gross profit and estimated net profit.
- Export report records to CSV for use in spreadsheet software.

## Technology

| Area | Technology |
|---|---|
| Application | ASP.NET Core Razor Pages, .NET 10 |
| Backend | C# |
| Data access | Entity Framework Core |
| Database | MySQL |
| Frontend | HTML, CSS and JavaScript |
| Validation | ASP.NET Core model validation and client-side validation |
| Source control | Git and GitHub |

## Application structure

```text
TradeTrack/
├── Data/                 # Entity Framework database context and mappings
├── Models/               # Product, sale, sale item, customer and expense models
├── Pages/
│   ├── Customers/        # Customer directory and customer forms
│   ├── Expenses/         # Expense history, creation and editing
│   ├── Products/         # Product catalogue and product forms
│   ├── Reports/          # Date-filtered business reports and CSV export
│   ├── Sales/            # Sales history and transaction entry
│   └── Shared/           # Shared layout and sidebar
├── database/             # Ordered MySQL schema and seed scripts
└── wwwroot/
    ├── css/              # Application styles
    └── js/               # Shared, inventory and sales scripts
```

## Requirements

- .NET 10 SDK
- MySQL Server 8.0 or later
- Git

## Local setup

1. Clone the repository:

   ```powershell
   git clone https://github.com/techboymd1/TradeTrack.git
   cd TradeTrack
   ```

2. Create the MySQL database and tables by running the SQL files in the `database` folder in numerical order:

   - `001-initial-schema.sql`
   - `002-seed-development-data.sql`
   - `003-sales-schema.sql`
   - `004-expenses-schema.sql`
   - `005-customers-schema.sql`

   Run each schema script once against the `tradetrack` database. The seed script is for development data.

3. Configure the database connection with .NET User Secrets. From the project directory, set the connection string using your own MySQL username and password:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:TradeTrack" "Server=127.0.0.1;Port=3306;Database=tradetrack;User ID=YOUR_MYSQL_USERNAME;Password=YOUR_MYSQL_PASSWORD;"
   ```

   Do not commit database passwords or other credentials to source control.

4. Restore dependencies and build:

   ```powershell
   dotnet restore
   dotnet build
   ```

5. Run the application:

   ```powershell
   dotnet watch
   ```

6. Open the local URL shown in the terminal, typically `http://localhost:5123` when using the project's current launch settings.

## Financial calculations

- Sales revenue is based on completed sale totals after discounts.
- Cost of goods sold uses the unit-cost snapshots saved on each sale item, rather than the product's current cost price.
- Estimated net profit is calculated as sales revenue less cost of goods sold and recorded expenses for the selected period.
- These figures reflect records entered into TradeTrack; they are operational summaries, not a substitute for a full accounting or tax system.

## Security and configuration

- Database credentials should be stored in .NET User Secrets during local development and in an appropriate secure configuration provider in production.
- `bin/`, `obj/`, local IDE state and user-specific files are excluded through `.gitignore`.
- Configure production hosting, HTTPS, database backups and access controls before using the application with live business data.

## Repository

[github.com/techboymd1/TradeTrack](https://github.com/techboymd1/TradeTrack)
