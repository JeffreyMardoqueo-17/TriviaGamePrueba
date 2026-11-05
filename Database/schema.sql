CREATE DATABASE TriviaDB;
GO

USE TriviaDB;
GO

CREATE TABLE Users (
    Id INT IDENTITY (1, 1) PRIMARY KEY,
    Username NVARCHAR (50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR (255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME ()
);
GO

CREATE TABLE Categories (
    Id INT IDENTITY (1, 1) PRIMARY KEY,
    [Name] NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (255) NULL
);
GO

CREATE TABLE Questions (
    Id INT IDENTITY (1, 1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    QuestionText NVARCHAR (500) NOT NULL,
    CONSTRAINT FK_Questions_Categories FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE CASCADE
);
GO

CREATE TABLE Answers (
    Id INT IDENTITY (1, 1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    AnswerText NVARCHAR (255) NOT NULL,
    IsCorrect BIT NOT NULL,
    CONSTRAINT FK_Answers_Questions FOREIGN KEY (QuestionId) REFERENCES Questions (Id) ON DELETE CASCADE
);
GO

----------esto es para la tabla de posiciones
CREATE TABLE UserScores (
    UserId INT PRIMARY KEY,
    TotalPoints INT NOT NULL DEFAULT 0,
    GamesPlayed INT NOT NULL DEFAULT 0,
    CorrectAnswers INT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_UserScores_Users FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE
);
GO

CREATE TABLE GameHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    PointsEarned INT NOT NULL,
    CorrectAnswersCount INT NOT NULL,
    PlayedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_GameHistory_Users FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    CONSTRAINT FK_GameHistory_Categories FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX IX_UserScores_TotalPoints 
ON UserScores (TotalPoints DESC);
GO