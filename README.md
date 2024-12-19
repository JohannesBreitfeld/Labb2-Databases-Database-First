# Bookstore Manager
This is an application created in **C#** using **WPF** for managing a fictional bookstore chain's inventory. I have used a version of the **SQL Server** database from [previous assignment](https://github.com/JohannesBreitfeld/NET24-Databases/tree/main/Labb1-Relationsdatabas-efter-specifikation) and **Entity Framework Core** for data access.

### Technologies, frameworks and patterns used:
- C#
- WPF (Windows Presentation Foundation)
- SQL Server
- Entity Framework Core
- MVVM design pattern

## Application overview
### List inventories for each store. Add or remove books from the book catalog to each stores inventory and update quantities in stock.

![InventoryView.png](https://github.com/JohannesBreitfeld/Labb2-Databases-Database-First/blob/master/ReadmeImages/InventoryView.png)

### Handle the book catalog. Edit the titles already included in the catalog or add a new!

![BookCatalogViewEdit.png](https://github.com/JohannesBreitfeld/Labb2-Databases-Database-First/blob/master/ReadmeImages/BookCatalogViewEdit.png)

![BookCatalogViewAdd.png](https://github.com/JohannesBreitfeld/Labb2-Databases-Database-First/blob/master/ReadmeImages/BookCatalogViewAdd.png)

### Add or edit authors

![AuthorView.png](https://github.com/JohannesBreitfeld/Labb2-Databases-Database-First/blob/master/ReadmeImages/AuthorView.png)

## Installation

**1. Clone the repository to your local machine** 

**2. Set up the database**  
- Restore the database backup file BookStoreLight.bak on your local machine
- Then setup a user secret in Bookstore.Infrastructure with the key value pairs:  
`{  
  "ConnectionString": "Initial Catalog=Bookstorelight;Integrated Security=True;Trust Server Certificate=True;Server SPN=localhost"
}`
- OR you can go to Bookstore.Infrastructure -> Data -> Model -> BookstoreContext.cs and change the connection string on line 36 to:  
`"Initial Catalog=Bookstorelight;Integrated Security=True;Trust Server Certificate=True;Server SPN=localhost"`