IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200524122217_initial_create')
BEGIN
    CREATE TABLE [ConsoleLogs] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Information] nvarchar(max) NULL,
        [IsNeededToSendEmail] bit NOT NULL,
        [SendEmail] bit NOT NULL,
        CONSTRAINT [PK_ConsoleLogs] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200524122217_initial_create')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200524122217_initial_create', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200527123220_add_log_table')
BEGIN
    CREATE TABLE [Log] (
        [Id] int NOT NULL IDENTITY,
        [Message] nvarchar(max) NULL,
        [MessageTemplate] nvarchar(max) NULL,
        [Level] nvarchar(max) NULL,
        [TimeStamp] datetimeoffset NOT NULL,
        [Exception] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [LogEvent] nvarchar(max) NULL,
        CONSTRAINT [PK_Log] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200527123220_add_log_table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200527123220_add_log_table', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200530140807_add_ServerUser_table')
BEGIN
    CREATE TABLE [ServerUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(max) NULL,
        [Xuid] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ServerUsers] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200530140807_add_ServerUser_table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200530140807_add_ServerUser_table', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200530141205_fix_ServerUser_table')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServerUsers]') AND [c].[name] = N'Xuid');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ServerUsers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ServerUsers] ALTER COLUMN [Xuid] nvarchar(16) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200530141205_fix_ServerUser_table')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServerUsers]') AND [c].[name] = N'UserName');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ServerUsers] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ServerUsers] ALTER COLUMN [UserName] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200530141205_fix_ServerUser_table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200530141205_fix_ServerUser_table', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200531051650_addEmailTable')
BEGIN
    CREATE TABLE [Emails] (
        [Id] int NOT NULL IDENTITY,
        [EmailAddress] nvarchar(max) NULL,
        CONSTRAINT [PK_Emails] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200531051650_addEmailTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200531051650_addEmailTable', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200605055538_addDecsriptionColumnToServerUsersTable')
BEGIN
    ALTER TABLE [ServerUsers] ADD [Description] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200605055538_addDecsriptionColumnToServerUsersTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200605055538_addDecsriptionColumnToServerUsersTable', N'3.1.3');
END;

GO

