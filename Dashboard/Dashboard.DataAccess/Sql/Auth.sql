DROP TABLE IF EXISTS AuthClients;
CREATE TABLE [AuthClients] ( 
  [Id] nvarchar(40) NOT NULL,
  [Secret] nvarchar(200) NOT NULL,
  [Name] nvarchar(100) NOT NULL,
  [ApplicationType] INTEGER not null,
  [Active] bit not null,
  [RefreshTokenLifeTime] INTEGER NOT NULL,
  [AllowedOrigin] nvarchar(4000) NOT NULL ,
  [CreatedAt] Date NOT NULL ,
  CONSTRAINT [PK_AuthClients] PRIMARY KEY ([Id]) 
);

DROP TABLE IF EXISTS "AuthRefreshToken";
CREATE TABLE [AuthRefreshToken] (
	[Id] nvarchar(40) NOT NULL,
	[Subject] nvarchar(50) NOT NULL,
    [ClientId] nvarchar(50) NOT NULL,
	[IssuedUtc] Date NOT NULL,
	[ExpiresUtc] INTEGER not null,
	[ProtectedTicket] INTEGER not null,
	CONSTRAINT [PK_AuthRefreshTokens] PRIMARY KEY ([Id]) 
);