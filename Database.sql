CREATE TABLE [Category] (
    [CategoryID] smallint NOT NULL IDENTITY,
    [CategoryName] nvarchar(100) NOT NULL,
    [CategoryDesciption] nvarchar(250) NOT NULL,
    [ParentCategoryID] smallint NULL,
    [IsActive] bit NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([CategoryID]),
    CONSTRAINT [FK_Category_Category] FOREIGN KEY ([ParentCategoryID]) REFERENCES [Category] ([CategoryID])
);
GO


CREATE TABLE [SystemAccount] (
    [AccountID] smallint NOT NULL,
    [AccountName] nvarchar(100) NULL,
    [AccountEmail] nvarchar(70) NULL,
    [AccountRole] int NULL,
    [AccountPassword] nvarchar(70) NULL,
    CONSTRAINT [PK_SystemAccount] PRIMARY KEY ([AccountID])
);
GO


CREATE TABLE [Tag] (
    [TagID] int NOT NULL,
    [TagName] nvarchar(50) NULL,
    [Note] nvarchar(400) NULL,
    CONSTRAINT [PK_HashTag] PRIMARY KEY ([TagID])
);
GO


CREATE TABLE [NewsArticle] (
    [NewsArticleID] nvarchar(20) NOT NULL,
    [NewsTitle] nvarchar(400) NULL,
    [Headline] nvarchar(150) NOT NULL,
    [CreatedDate] datetime NULL,
    [NewsContent] nvarchar(4000) NULL,
    [NewsSource] nvarchar(400) NULL,
    [CategoryID] smallint NULL,
    [NewsStatus] bit NULL,
    [CreatedByID] smallint NULL,
    [UpdatedByID] smallint NULL,
    [ModifiedDate] datetime NULL,
    CONSTRAINT [PK_NewsArticle] PRIMARY KEY ([NewsArticleID]),
    CONSTRAINT [FK_NewsArticle_Category] FOREIGN KEY ([CategoryID]) REFERENCES [Category] ([CategoryID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NewsArticle_SystemAccount] FOREIGN KEY ([CreatedByID]) REFERENCES [SystemAccount] ([AccountID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NewsArticle_SystemAccount_UpdatedBy] FOREIGN KEY ([UpdatedByID]) REFERENCES [SystemAccount] ([AccountID])
);
GO


CREATE TABLE [NewsTag] (
    [NewsArticleID] nvarchar(20) NOT NULL,
    [TagID] int NOT NULL,
    CONSTRAINT [PK_NewsTag] PRIMARY KEY ([NewsArticleID], [TagID]),
    CONSTRAINT [FK_NewsTag_NewsArticle] FOREIGN KEY ([NewsArticleID]) REFERENCES [NewsArticle] ([NewsArticleID]),
    CONSTRAINT [FK_NewsTag_Tag] FOREIGN KEY ([TagID]) REFERENCES [Tag] ([TagID])
);
GO


CREATE INDEX [IX_Category_ParentCategoryID] ON [Category] ([ParentCategoryID]);
GO


CREATE INDEX [IX_NewsArticle_CategoryID] ON [NewsArticle] ([CategoryID]);
GO


CREATE INDEX [IX_NewsArticle_CreatedByID] ON [NewsArticle] ([CreatedByID]);
GO


CREATE INDEX [IX_NewsArticle_UpdatedByID] ON [NewsArticle] ([UpdatedByID]);
GO


CREATE INDEX [IX_NewsTag_TagID] ON [NewsTag] ([TagID]);
GO


