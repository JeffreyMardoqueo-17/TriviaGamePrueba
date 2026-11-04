---traer las categorias
CREATE OR ALTER PROCEDURE sp_GetCategories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Description
    FROM Categories;
END;
GO

---Obtener 3 Preguntas aleatorias y sus respueestas por categoria, basicamente aca esta el corazon del juego
CREATE OR ALTER PROCEDURE sp_GetRandomQuestionsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar si la categoría existe
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