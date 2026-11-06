
CREATE OR ALTER PROCEDURE sp_GetCategories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Description
    FROM Categories;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetRandomQuestionsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @CategoryId)
    BEGIN
        RAISERROR('La categoría especificada no existe.', 16, 1);
        RETURN;
    END

    -- Seleccionar aleatoriamente exactamente 3 preguntas de la categoría usando NEWID()
    SELECT TOP 3 Id AS QuestionId, CategoryId, QuestionText
    INTO #RandomQuestions
    FROM Questions
    WHERE CategoryId = @CategoryId
    ORDER BY NEWID();

    -- Conjunto de Resultados 1: Las 3 preguntas seleccionadas
    SELECT QuestionId, CategoryId, QuestionText
    FROM #RandomQuestions;

    -- Conjunto de Resultados 2: Las respuestas asociadas a esas 3 preguntas
    SELECT a.Id AS AnswerId, a.QuestionId, a.AnswerText, a.IsCorrect
    FROM Answers a
    INNER JOIN #RandomQuestions rq ON a.QuestionId = rq.QuestionId;

    -- Limpiar la tabla temporal
    DROP TABLE #RandomQuestions;
END;
GO

-- 3. Guardar Resultado de Partida y Actualizar Puntuación (Upsert Transaccional)
CREATE OR ALTER PROCEDURE sp_SaveGameResult
    @UserId INT,
    @CategoryId INT,
    @PointsEarned INT,
    @CorrectAnswersCount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Registrar la partida en el historial
        INSERT INTO GameHistory (UserId, CategoryId, PointsEarned, CorrectAnswersCount)
        VALUES (@UserId, @CategoryId, @PointsEarned, @CorrectAnswersCount);

        -- Actualizar o insertar el acumulado en UserScores
        IF EXISTS (SELECT 1 FROM UserScores WHERE UserId = @UserId)
        BEGIN
            UPDATE UserScores
            SET TotalPoints = TotalPoints + @PointsEarned,
                GamesPlayed = GamesPlayed + 1,
                CorrectAnswers = CorrectAnswers + @CorrectAnswersCount,
                UpdatedAt = SYSDATETIME()
            WHERE UserId = @UserId;
        END
        ELSE
        BEGIN
            INSERT INTO UserScores (UserId, TotalPoints, GamesPlayed, CorrectAnswers, UpdatedAt)
            VALUES (@UserId, @PointsEarned, 1, @CorrectAnswersCount, SYSDATETIME());
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

--(Ranking)
CREATE OR ALTER PROCEDURE sp_GetLeaderboard
    @TopCount INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopCount)
        ROW_NUMBER() OVER (ORDER BY us.TotalPoints DESC, us.UpdatedAt ASC) AS RankPosition,
        u.Id AS UserId,
        u.Username,
        us.TotalPoints,
        us.GamesPlayed,
        us.CorrectAnswers
    FROM UserScores us
    INNER JOIN Users u ON us.UserId = u.Id
    ORDER BY us.TotalPoints DESC, us.UpdatedAt ASC;
END;
GO