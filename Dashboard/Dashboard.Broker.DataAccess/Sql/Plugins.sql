DROP TABLE IF EXISTS "Plugins";
CREATE TABLE [Plugins] ( 
  [Id] nvarchar(40) NOT NULL,
  [Version] nvarchar(20) NOT NULL,
  [Name] nvarchar(100) NOT NULL,
  [CommunicationType] INTEGER not null,
  [StartingProgram] nvarchar(50) not null,
  [CheckSum] nvarchar(100) not null,
  [ExecutablePath] nvarchar(100) not null,
  CONSTRAINT [PK_Plugins] PRIMARY KEY ([Id],[Version]) 
);