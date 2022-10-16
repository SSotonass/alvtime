CREATE TABLE [dbo].[EmploymentRate]
(
    [Id]       [int] IDENTITY (1,1) NOT NULL,
    [UserId]   [int]                NOT NULL,
    [Rate]     [decimal](7, 2)      NOT NULL,
    [FromDate] [datetime2]          NOT NULL,
    [ToDate]   [datetime2]          NOT NULL
    PRIMARY KEY CLUSTERED
(
[Id] ASC
) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
ALTER TABLE [dbo].[EmploymentRate]
    WITH CHECK ADD CONSTRAINT [FK_EmploymentRate_User] FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User] ([Id])
    GO