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
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE TABLE [EntradasLibroApertura] (
        [Id] int NOT NULL IDENTITY,
        [ClavePosicion] nvarchar(max) NOT NULL,
        [Origen] nvarchar(2) NOT NULL,
        [Destino] nvarchar(2) NOT NULL,
        [PromocionA] nvarchar(max) NULL,
        [VecesJugado] int NOT NULL,
        [Victorias] int NOT NULL,
        [Derrotas] int NOT NULL,
        [Tablas] int NOT NULL,
        CONSTRAINT [PK_EntradasLibroApertura] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE TABLE [Partidas] (
        [Id] uniqueidentifier NOT NULL,
        [TurnoActual] nvarchar(max) NOT NULL,
        [Estado] nvarchar(max) NOT NULL,
        [BlancoEnroqueCorto] bit NOT NULL,
        [BlancoEnroqueLargo] bit NOT NULL,
        [NegroEnroqueCorto] bit NOT NULL,
        [NegroEnroqueLargo] bit NOT NULL,
        [CasillaAlPasoFila] int NULL,
        [CasillaAlPasoColumna] int NULL,
        [ContadorMediosMovimientosSinCaptura] int NOT NULL,
        [Ganador] nvarchar(max) NULL,
        [MotivoFinalizacion] nvarchar(max) NULL,
        CONSTRAINT [PK_Partidas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE TABLE [ClavesPosicion] (
        [Id] int NOT NULL IDENTITY,
        [PartidaId] uniqueidentifier NOT NULL,
        [Orden] int NOT NULL,
        [Clave] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ClavesPosicion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClavesPosicion_Partidas_PartidaId] FOREIGN KEY ([PartidaId]) REFERENCES [Partidas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE TABLE [Movimientos] (
        [Id] int NOT NULL IDENTITY,
        [PartidaId] uniqueidentifier NOT NULL,
        [Orden] int NOT NULL,
        [OrigenFila] int NOT NULL,
        [OrigenColumna] int NOT NULL,
        [DestinoFila] int NOT NULL,
        [DestinoColumna] int NOT NULL,
        [PromocionA] nvarchar(20) NULL,
        [EsEnroqueCorto] bit NOT NULL,
        [EsEnroqueLargo] bit NOT NULL,
        [EsCapturaAlPaso] bit NOT NULL,
        CONSTRAINT [PK_Movimientos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Movimientos_Partidas_PartidaId] FOREIGN KEY ([PartidaId]) REFERENCES [Partidas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE TABLE [PiezasTablero] (
        [Id] int NOT NULL IDENTITY,
        [PartidaId] uniqueidentifier NOT NULL,
        [Fila] int NOT NULL,
        [Columna] int NOT NULL,
        [Tipo] nvarchar(20) NOT NULL,
        [Color] nvarchar(10) NOT NULL,
        CONSTRAINT [PK_PiezasTablero] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PiezasTablero_Partidas_PartidaId] FOREIGN KEY ([PartidaId]) REFERENCES [Partidas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE INDEX [IX_ClavesPosicion_PartidaId_Orden] ON [ClavesPosicion] ([PartidaId], [Orden]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE INDEX [IX_Movimientos_PartidaId_Orden] ON [Movimientos] ([PartidaId], [Orden]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    CREATE INDEX [IX_PiezasTablero_PartidaId] ON [PiezasTablero] ([PartidaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901015507_EsquemaInicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260901015507_EsquemaInicial', N'10.0.11');
END;

COMMIT;
GO

