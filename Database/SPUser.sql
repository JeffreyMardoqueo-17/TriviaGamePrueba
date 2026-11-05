USE TriviaBD;
GO

---todos los SP de usuario

---CREAR
CREATE PROCEDURE sp_RegisterUser
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- validaciones
    IF LTRIM(RTRIM(@Username)) = '' OR LTRIM(RTRIM(@PasswordHash)) = ''
    BEGIN
        RAISERROR('El nombre de usuario y la contraseña no pueden estar vacíos.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        RAISERROR('El nombre de usuario ya está registrado. Por favor, elige otro.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        -- inserto y devuelvo 
        INSERT INTO Users (Username, PasswordHash)
        VALUES (@Username, @PasswordHash);

        SELECT SCOPE_IDENTITY() AS UserId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

---LOGIN
CREATE OR ALTER PROCEDURE sp_LoginUser
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@Username)) = ''
    BEGIN
        RAISERROR('Username cannot be empty.', 16, 1);
        RETURN;
    END
    SELECT Id, Username, PasswordHash
    FROM Users
    WHERE Username = @Username;
END;
GO
--
CREATE OR ALTER PROCEDURE sp_ExistUser
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@Username)) = ''
    BEGIN
        RAISERROR('El nombre de usuario no puede estar vacío.', 16, 1);
        RETURN;
    END

    -- Retorna 1 si existe, 0 si no existe (muy eficiente con IF EXISTS)
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT CAST(1 AS BIT) AS UserExists;
    END
    ELSE
    BEGIN
        SELECT CAST(0 AS BIT) AS UserExists;
    END
END;
GO

--PRUEBA
DECLARE @TestUsername NVARCHAR (50) = 'jeffreymardoqueo';
DECLARE @TestPasswordHash NVARCHAR (255) = '$2a$12$OEkq3tNbB0UAFvmwol1VVuMJqS3ObjT0qdAWKgS4F6oWOIxDeOOAq'; --1234prueba

EXEC sp_RegisterUser @Username = @TestUsername,
@PasswordHash = @TestPasswordHash;

SELECT * FROM Users;