CREATE TABLE [User] (
          [Id] int NOT NULL IDENTITY,
          [Fullname] nvarchar(max) NOT NULL,
          [Username] nvarchar(max) NOT NULL,
          [Email] nvarchar(max) NOT NULL,
          [Address] nvarchar(max) NOT NULL,
          [CreatedBy] nvarchar(max) NULL,
          [CreatedOn] datetime2 NULL,
          [UpdatedBy] nvarchar(max) NULL,
          [UpdatedOn] datetime2 NULL,
          CONSTRAINT [PK_User] PRIMARY KEY ([Id])
      );
CREATE TABLE [Orders] (
          [Id] int NOT NULL IDENTITY,
          [InvoiceNumber] nvarchar(max) NOT NULL,
          [ProductName] nvarchar(max) NOT NULL,
          [Quantity] int NOT NULL,
          [Description] nvarchar(max) NOT NULL,
          [UserId] int NOT NULL,
          [CreatedBy] nvarchar(max) NULL,
          [CreatedOn] datetime2 NULL,
          [UpdatedBy] nvarchar(max) NULL,
          [UpdatedOn] datetime2 NULL,
          CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
          CONSTRAINT [FK_Orders_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
      );
CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);