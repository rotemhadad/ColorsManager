# Clors Management Web Application

This is a web application developed in ASP.NET Web Forms using **VB.NET** for managing a colors database. The application allows users to view, edit, delete, and add colors to the database using a web-based interface.

## Project Overview

The project consists of:

- **ASP.NET Web Forms** using VB.NET for the backend.
- **SQL Server Database** for storing color information (Microsoft Access could also be used).
- **jQuery and AJAX** for client-side interactions.
- **Bootstrap** for UI styling and layout.

## Features Implemented

- **Display Colors Table:** A table displaying the list of colors with their properties such as name, price, display order, color code, and stock availability.
- **CRUD Operations:** Users can:
  - **Create:** Add a new color to the table.
  - **Read:** View all colors stored in the database.
  - **Update:** Edit existing color details.
  - **Delete:** Remove a color from the table.
- **Color Preview:** When adding or editing a color, the selected color code is previewed in real-time.
- **SQL Server Integration:** The application interacts with a SQL Server database using `System.Data.SqlClient`.

## Tools and Technologies Used

- **ASP.NET Web Forms (VB.NET)**
- **SQL Server Management Studio (SSMS)**
- **jQuery and jQuery UI**
- **AJAX for Server Communication**
- **Bootstrap for Styling**

## Database Structure

The database (`ColorsDB`) includes a single table:

- **Colors**
  - `Id` (Primary Key)
  - `ColorName` (nvarchar)
  - `Price` (int)
  - `DisplayOrder` (int)
  - `InStock` (bit)
  - `ColorCode` (nvarchar)
