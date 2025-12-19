# TimeTrackingApp
1.Stwórz baze danych w postgresie o nazwie "TimeTracking", i wprowadz w pliku appsettings.json hasło do swojego postgresa.
2.Odpal projekt w visual oraz wykonaj migrację(Add-Migration jakasnazwa) i uaktualnij baze danych(Update-Database).
3.Dodaj 3 użytkowników (admin jest wbudowany login: admin@admin.com Admin123!):
INSERT INTO "AspNetUsers" (
    "Id", "FirstName", "LastName", "Position", "Department", "IsActive", "CreatedAt",
    "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
    "PasswordHash", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount"
) VALUES
('72914c4b-197d-4f72-835a-b4bb76fedb3d', 'Kacper', 'Wąsik', 'Kierownik', 'Masterdev', TRUE, '2025-12-11 20:48:05.975843+00', 'MikolajLukoski@gmail.com', 'MIKOLAJLUKOSKI@GMAIL.COM', 'MikolajLukoski@gmail.com', 'MIKOLAJLUKOSKI@GMAIL.COM', TRUE, 'AQAAAAIAAYagAAAAELnyQUuz9/lmxtkAJFOtpXdCADgAJ0ENPqERMNk4VtcN0PJtFmY+/VHsYBqyN3tYlA==', FALSE, FALSE, FALSE, 0),
('7e020b16-df5f-4877-b858-697cbe517d64', 'Antoni', 'Wąsik', 'Developer', 'Masterdev', TRUE, '2025-12-17 13:49:56.254677+00', 'Antekwasik2008@gmail.com', 'ANTEKWASIK2008@GMAIL.COM', 'Antekwasik2008@gmail.com', 'ANTEKWASIK2008@GMAIL.COM', TRUE, 'AQAAAAIAAYagAAAAELmge83uvJap4Y9dCQk955LKVyCLQK1aRhUBJtkKzjCz6jnrOp6u3S4C2BeTTFPbfQ==', FALSE, FALSE, FALSE, 0),
('b78c4a2c-9b5c-4104-a672-d8d6d0bb6d53', 'White', 'Walter', 'Programista', 'Masterdev', TRUE, '2025-12-10 13:10:16.863222+00', 'whiteplwalter@gmail.com', 'WHITEPLWALTER@GMAIL.COM', 'whiteplwalter@gmail.com', 'WHITEPLWALTER@GMAIL.COM', TRUE, 'AQAAAAIAAYagAAAAEMlspaQNg6GXHlfWQdqQxg8fDrYRJr6Gz7Miaqa0GIvszj8J1hZ5zh/dEGb6/zSuEg==', FALSE, FALSE, FALSE, 0);


