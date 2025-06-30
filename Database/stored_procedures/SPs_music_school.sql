CREATE PROCEDURE SP_CreateSchool
    @Code NVARCHAR(50),
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Schools (Code, Name, Description, CreatedAt, UpdatedAt)
    VALUES (@Code, @Name, @Description, @CreatedAt, @UpdatedAt);

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

CREATE PROCEDURE SP_UpdateSchool
    @Id INT,
    @Code NVARCHAR(50),
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Schools
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Retornar cuántas filas fueron afectadas (por si quieres validar si realmente existía)
    SELECT @@ROWCOUNT AS AffectedRows;
END;
GO

CREATE PROCEDURE SP_CreateStudent
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @BirthDate DATE,
    @IdentificationNumber NVARCHAR(50),
    @SchoolId INT,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Insertar el estudiante
    INSERT INTO Students (FirstName, LastName, BirthDate, IdentificationNumber, CreatedAt, UpdatedAt)
    VALUES (@FirstName, @LastName, @BirthDate, @IdentificationNumber, @CreatedAt, @UpdatedAt);

    -- Obtener el nuevo ID generado
    DECLARE @NewStudentId INT = SCOPE_IDENTITY();

    -- Relacionar con la escuela
    INSERT INTO SchoolStudents (StudentId, SchoolId, EnrollmentDate, CreatedAt, UpdatedAt)
    VALUES (@NewStudentId, @SchoolId, @CreatedAt, @CreatedAt, @UpdatedAt);

    -- Retornar el ID del nuevo estudiante
    SELECT @NewStudentId AS NewId;
END;
GO

CREATE PROCEDURE SP_UpdateStudent
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @BirthDate DATE,
    @IdentificationNumber NVARCHAR(50),
    @SchoolId INT,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el estudiante existe
    IF NOT EXISTS (SELECT 1 FROM Students WHERE Id = @Id)
    BEGIN
        RAISERROR('Student not found.', 16, 1);
        RETURN;
    END

    -- Actualizar datos del estudiante
    UPDATE Students
    SET 
        FirstName = @FirstName,
        LastName = @LastName,
        BirthDate = @BirthDate,
        IdentificationNumber = @IdentificationNumber,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Verificar si ya existe relación con esa escuela
    DECLARE @CurrentSchoolId INT;
    SELECT TOP 1 @CurrentSchoolId = SchoolId
    FROM SchoolStudents
    WHERE StudentId = @Id;

    IF @CurrentSchoolId IS NULL
    BEGIN
        -- Insertar nueva relación
        INSERT INTO SchoolStudents (StudentId, SchoolId, EnrollmentDate, CreatedAt, UpdatedAt)
        VALUES (@Id, @SchoolId, GETDATE(), GETDATE(), GETDATE());
    END
    ELSE IF @CurrentSchoolId != @SchoolId
    BEGIN
        -- Eliminar relación antigua y crear nueva
        DELETE FROM SchoolStudents WHERE StudentId = @Id AND SchoolId = @CurrentSchoolId;

        INSERT INTO SchoolStudents (StudentId, SchoolId, EnrollmentDate, CreatedAt, UpdatedAt)
        VALUES (@Id, @SchoolId, GETDATE(), GETDATE(), GETDATE());
    END

    -- Confirmar éxito
    SELECT 1 AS Success;
END;
GO

CREATE PROCEDURE SP_CreateTeacher
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @IdentificationNumber NVARCHAR(50),
    @SchoolId INT,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Insertar al docente
    INSERT INTO Teachers (FirstName, LastName, IdentificationNumber, CreatedAt, UpdatedAt)
    VALUES (@FirstName, @LastName, @IdentificationNumber, @CreatedAt, @UpdatedAt);

    DECLARE @NewTeacherId INT = SCOPE_IDENTITY();

    -- Insertar relación con escuela
    INSERT INTO SchoolTeachers (TeacherId, SchoolId, CreatedAt, UpdatedAt)
    VALUES (@NewTeacherId, @SchoolId, @CreatedAt, @UpdatedAt);

    -- Retornar ID del nuevo docente
    SELECT @NewTeacherId AS NewId;
END;
GO

CREATE PROCEDURE SP_UpdateTeacher
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @IdentificationNumber NVARCHAR(50),
    @SchoolId INT,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar existencia del docente
    IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Id = @Id)
    BEGIN
        RAISERROR('Teacher not found.', 16, 1);
        RETURN;
    END

    -- Actualizar datos personales
    UPDATE Teachers
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        IdentificationNumber = @IdentificationNumber,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Verificar relación actual
    DECLARE @CurrentSchoolId INT;
    SELECT TOP 1 @CurrentSchoolId = SchoolId
    FROM SchoolTeachers
    WHERE TeacherId = @Id;

    IF @CurrentSchoolId IS NULL
    BEGIN
        -- Insertar si no hay relación previa
        INSERT INTO SchoolTeachers (TeacherId, SchoolId, CreatedAt, UpdatedAt)
        VALUES (@Id, @SchoolId, @UpdatedAt, @UpdatedAt);
    END
    ELSE IF @CurrentSchoolId != @SchoolId
    BEGIN
        -- Cambiar la relación si es diferente
        DELETE FROM SchoolTeachers WHERE TeacherId = @Id AND SchoolId = @CurrentSchoolId;

        INSERT INTO SchoolTeachers (TeacherId, SchoolId, CreatedAt, UpdatedAt)
        VALUES (@Id, @SchoolId, @UpdatedAt, @UpdatedAt);
    END

    -- Confirmar éxito
    SELECT 1 AS Success;
END;
GO

--Insert para el usuario por defecto
INSERT INTO [music_school].[dbo].[Users] 
([Username], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt], [UpdatedAt])
VALUES 
('admin', 'admin@gmail.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 'User', 1, GETDATE(), GETDATE());