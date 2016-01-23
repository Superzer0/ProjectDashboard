DROP TABLE IF EXISTS "Plugins";
CREATE TABLE [Plugins] ( 
  [Id] uniqueidentifier NOT NULL,
  [Version] nvarchar(20) NOT NULL,
  [Name] nvarchar(100) NOT NULL,
  [CommunicationType] INTEGER not null,
  [StartingProgram] nvarchar(50) not null,
  [Configuration] nvarchar(4000) NOT NULL,
  [Xml] nvarchar(4000) NOT NULL ,
  [Added] datetime NOT NULL,
  [AddedBy] nvarchar(100) not null ,
  CONSTRAINT [PK_Plugins] PRIMARY KEY ([Id],[Version]) 
);

DROP TABLE IF EXISTS "PluginMethods";
CREATE TABLE [PluginMethods] (
	[Id] uniqueidentifier NOT NULL,
	[Plugin_Id] uniqueidentifier NOT NULL,
    [Plugin_Version] nvarchar(20) NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	[InputType] INTEGER not null,
	[OutputType] INTEGER not null,
	FOREIGN KEY (Plugin_Id,Plugin_Version) REFERENCES Plugins(Id,Version)
);

DROP TABLE IF EXISTS "PluginsUiConfiguration";
CREATE TABLE [PluginsUiConfiguration] (
	[Id] uniqueidentifier NOT NULL,
	[Version] nvarchar(20) NOT NULL,
	[UserId] uniqueidentifier NOT NULL,
	[JsonConfiguration] nvarchar(4000) NOT NULL,
	PRIMARY KEY ("Id", "Version","UserId")
 );
 
 CREATE TABLE "InstanceSettings" ( 
  [Id] nvarchar(1000) NOT NULL 
, [Value] nvarchar(1000) NOT NULL 
, CONSTRAINT [PK_Settings] PRIMARY KEY ([Id]) 
)
