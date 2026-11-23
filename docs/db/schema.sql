IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE TABLE [RoomTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(60) NOT NULL,
        [Description] nvarchar(500) NULL,
        [PricePerNight] decimal(10,2) NOT NULL,
        [MaxGuests] int NOT NULL,
        [ImageUrl] nvarchar(300) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_RoomTypes] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_RoomTypes_MaxGuests] CHECK ([MaxGuests] > 0),
        CONSTRAINT [CK_RoomTypes_PricePerNight] CHECK ([PricePerNight] > 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [Phone] nvarchar(20) NULL,
        [Role] nvarchar(20) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Users_Role] CHECK ([Role] IN ('Guest', 'Receptionist', 'Admin'))
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE TABLE [Rooms] (
        [Id] int NOT NULL IDENTITY,
        [RoomNumber] nvarchar(10) NOT NULL,
        [Floor] int NOT NULL,
        [RoomTypeId] int NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [Notes] nvarchar(300) NULL,
        CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Rooms_Status] CHECK ([Status] IN ('Available', 'Maintenance')),
        CONSTRAINT [FK_Rooms_RoomTypes_RoomTypeId] FOREIGN KEY ([RoomTypeId]) REFERENCES [RoomTypes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE TABLE [Reservations] (
        [Id] int NOT NULL IDENTITY,
        [ReservationCode] nvarchar(20) NOT NULL,
        [GuestUserId] int NOT NULL,
        [RoomId] int NOT NULL,
        [CheckInDate] date NOT NULL,
        [CheckOutDate] date NOT NULL,
        [NumberOfGuests] int NOT NULL,
        [NightlyRate] decimal(10,2) NOT NULL,
        [TotalPrice] decimal(10,2) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [Notes] nvarchar(300) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Reservations_Dates] CHECK ([CheckOutDate] > [CheckInDate]),
        CONSTRAINT [CK_Reservations_NumberOfGuests] CHECK ([NumberOfGuests] > 0),
        CONSTRAINT [CK_Reservations_Status] CHECK ([Status] IN ('Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled')),
        CONSTRAINT [FK_Reservations_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reservations_Users_GuestUserId] FOREIGN KEY ([GuestUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [ReservationId] int NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [Method] nvarchar(20) NOT NULL,
        [PaidAt] datetime2 NOT NULL,
        [RecordedByUserId] int NOT NULL,
        [Notes] nvarchar(300) NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Payments_Amount] CHECK ([Amount] > 0),
        CONSTRAINT [CK_Payments_Method] CHECK ([Method] IN ('Cash', 'Card')),
        CONSTRAINT [FK_Payments_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Users_RecordedByUserId] FOREIGN KEY ([RecordedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_RecordedByUserId] ON [Payments] ([RecordedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_ReservationId] ON [Payments] ([ReservationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reservations_GuestUserId] ON [Reservations] ([GuestUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reservations_RoomId_Dates] ON [Reservations] ([RoomId], [CheckInDate], [CheckOutDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reservations_Status_CheckInDate] ON [Reservations] ([Status], [CheckInDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Reservations_ReservationCode] ON [Reservations] ([ReservationCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Rooms_RoomTypeId] ON [Rooms] ([RoomTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Rooms_RoomNumber] ON [Rooms] ([RoomNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_RoomTypes_Name] ON [RoomTypes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123144630_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261123144630_InitialCreate', N'10.0.15');
END;

COMMIT;
GO

