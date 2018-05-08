GO

CREATE TABLE [dbo].[genre] (
    [id]   INT          IDENTITY (1, 1) NOT NULL,
    [name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_genre] PRIMARY KEY CLUSTERED ([id] ASC)
);

GO

CREATE TABLE [dbo].[book] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [name]     VARCHAR (100) NOT NULL,
    [count]    INT           DEFAULT ((0)) NOT NULL,
    [price]    REAL          NOT NULL,
    [genre_id] INT           NOT NULL,
    [deleted]  INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_book] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_book_genre] FOREIGN KEY ([genre_id]) REFERENCES [dbo].[genre] ([id])
);

GO

CREATE TABLE [dbo].[author] (
    [id]   INT           IDENTITY (1, 1) NOT NULL,
    [name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_author] PRIMARY KEY CLUSTERED ([id] ASC)
);

GO

CREATE TABLE [dbo].[author_book_link] (
    [book_id]   INT NOT NULL,
    [author_id] INT NOT NULL,
    CONSTRAINT [PK_author_book_link] PRIMARY KEY CLUSTERED ([book_id] ASC, [author_id] ASC),
    CONSTRAINT [FK_author_book_link_book] FOREIGN KEY ([book_id]) REFERENCES [dbo].[book] ([id]),
    CONSTRAINT [FK_author_book_link_author] FOREIGN KEY ([author_id]) REFERENCES [dbo].[author] ([id])
);

GO

CREATE TABLE [dbo].[users] (
    [id]   INT           IDENTITY (1, 1) NOT NULL,
    [name] VARCHAR (100) NULL,
    CONSTRAINT [PK_user] PRIMARY KEY CLUSTERED ([id] ASC)
);

GO

CREATE TABLE [dbo].[buy] (
    [id]      INT        IDENTITY (1, 1) NOT NULL,
    [date]    DATETIME   DEFAULT (getdate()) NOT NULL,
    [user_id] INT        NOT NULL,
    [price]   FLOAT (53) NOT NULL,
    CONSTRAINT [PK_buy] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_buy_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([id])
);

GO

CREATE TABLE [dbo].[buy_item] (
    [book_id] INT NOT NULL,
    [count]   INT NOT NULL,
    [buy_id]  INT NOT NULL,
    CONSTRAINT [FK_buy_item_buy] FOREIGN KEY ([buy_id]) REFERENCES [dbo].[buy] ([id]),
    CONSTRAINT [FK_buy_item_book] FOREIGN KEY ([book_id]) REFERENCES [dbo].[book] ([id])
);


