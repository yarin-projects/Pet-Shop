# Pet Shop Management System

## Overview

The Pet Shop Management System is a web application designed to manage a pet shop's operations. It provides functionalities for managing animals, categories, comments, and users within a pet shop environment. The system includes user authentication and authorization, enabling both regular users and administrators to interact with the application in different ways.

The project was completed on July 11th, 2024.

## Features

- **Animal Management**: View, create, edit, and delete animals in the pet shop. Animals are categorized, and comments can be added to individual animals.
- **Category Management**: Create and delete categories for organizing animals.
- **Comment Management**: Add, edit, and delete comments on animals. Comments are managed by both users and administrators.
- **User Management**: Register, log in, and manage user accounts. Users can update their profiles and view their comments. Admins have additional capabilities such as managing all users and comments.
- **Authentication and Authorization**: Secure access to various parts of the application using JWT tokens and cookies. Different user roles (Admin, User) have different access permissions.
- **Image Handling**: Upload and manage images associated with animals.

## Technology Stack

- **Backend**: ASP.NET Core
- **Database**: SQLite
- **Frontend**: HTML, CSS, and JavaScript (with ASP.NET Core MVC for views)
- **Security**: JWT (JSON Web Tokens) for authentication, Argon2 for password hashing, AES for encryption
