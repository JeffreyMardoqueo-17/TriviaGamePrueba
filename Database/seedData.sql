USE TriviaDB;
GO

--categorias 
INSERT INTO Categories (Name, Description) VALUES 
('Animales', 'Preguntas sobre el reino animal, su hábitat, características y curiosidades.'),
('Carros', 'Preguntas sobre vehículos, marcas, motores y cultura automotriz.');
GO
---pregutas de animales
INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuál es el animal terrestre más rápido del mundo?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El guepardo', 1),
(SCOPE_IDENTITY(), 'El león', 0),
(SCOPE_IDENTITY(), 'La gacela', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Qué mamífero es conocido por volar a pesar de ser un mamífero?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El murciélago', 1),
(SCOPE_IDENTITY(), 'La ardilla voladora', 0),
(SCOPE_IDENTITY(), 'El colibrí', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuál es el animal más grande que habita en los océanos de la Tierra?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'La ballena azul', 1),
(SCOPE_IDENTITY(), 'El tiburón blanco', 0),
(SCOPE_IDENTITY(), 'El calamar gigante', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuántos corazones tiene un pulpo?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Tres', 1),
(SCOPE_IDENTITY(), 'Uno', 0),
(SCOPE_IDENTITY(), 'Dos', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Qué animal es capaz de cambiar de color para camuflarse con su entorno?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El camaleón', 1),
(SCOPE_IDENTITY(), 'La rana arborícola', 0),
(SCOPE_IDENTITY(), 'El zorro ártico', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuál es el ave que no puede volar pero es una excelente nadadora en aguas frías?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El pingüino', 1),
(SCOPE_IDENTITY(), 'El avestruz', 0),
(SCOPE_IDENTITY(), 'El flamenco', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuál de los siguientes animales es herbívoro?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'La jirafa', 1),
(SCOPE_IDENTITY(), 'El lobo', 0),
(SCOPE_IDENTITY(), 'El león', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Qué insecto es famoso por producir miel y vivir en colmenas organizadas?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'La abeja', 1),
(SCOPE_IDENTITY(), 'La avispa', 0),
(SCOPE_IDENTITY(), 'La hormiga', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cuál es el único mamífero capaz de poner huevos?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El ornitorrinco', 1),
(SCOPE_IDENTITY(), 'El canguro', 0),
(SCOPE_IDENTITY(), 'El armadillo', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (1, '¿Cómo se le llama a un grupo de leones?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Una manada', 1),
(SCOPE_IDENTITY(), 'Una jauría', 0),
(SCOPE_IDENTITY(), 'Una bandada', 0);
GO


--preguntas sobre carros

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué marca de autos deportivos de lujo utiliza un toro furioso en su logotipo?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Lamborghini', 1),
(SCOPE_IDENTITY(), 'Ferrari', 0),
(SCOPE_IDENTITY(), 'Porsche', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Cuál es la función principal de la caja de cambios manual en un vehículo?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Modificar la relación de transmisión y velocidad entre el motor y las ruedas', 1),
(SCOPE_IDENTITY(), 'Detener el vehículo de forma inmediata', 0),
(SCOPE_IDENTITY(), 'Generar la energía eléctrica para las luces', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué significa el acrónimo SUV aplicado a los automóviles?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Sport Utility Vehicle (Vehículo Utilitario Deportivo)', 1),
(SCOPE_IDENTITY(), 'Super Urban Vehicle', 0),
(SCOPE_IDENTITY(), 'Simple Universal Van', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué componente del motor se encarga de encender la mezcla de aire y combustible mediante una chispa?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'La bujía', 1),
(SCOPE_IDENTITY(), 'El alternador', 0),
(SCOPE_IDENTITY(), 'El radiador', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué marca japonesa fabrica el popular modelo todoterreno robusto llamado Land Cruiser?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Toyota', 1),
(SCOPE_IDENTITY(), 'Nissan', 0),
(SCOPE_IDENTITY(), 'Honda', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué sistema de seguridad evita que las ruedas se bloqueen al frenar bruscamente?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'El sistema ABS', 1),
(SCOPE_IDENTITY(), 'El control de tracción (TCS)', 0),
(SCOPE_IDENTITY(), 'La dirección asistida', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Cuál es el país de origen de la marca de vehículos de lujo Volvo?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Suecia', 1),
(SCOPE_IDENTITY(), 'Alemania', 0),
(SCOPE_IDENTITY(), 'Japón', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Cómo se le llama comúnmente al combustible utilizado por la mayoría de camiones pesados y camionetas diésel?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Gasóleo o Diésel', 1),
(SCOPE_IDENTITY(), 'Gasolina súper', 0),
(SCOPE_IDENTITY(), 'Gas licuado de petróleo', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué parte del vehículo se encarga de absorber los impactos y baches del camino?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'La suspensión y amortiguadores', 1),
(SCOPE_IDENTITY(), 'El chasis estructural', 0),
(SCOPE_IDENTITY(), 'El sistema de escape', 0);

INSERT INTO Questions (CategoryId, QuestionText) VALUES (2, '¿Qué marca de vehículos deportivos de origen italiano tiene un caballo encabritado como emblema?');
INSERT INTO Answers (QuestionId, AnswerText, IsCorrect) VALUES 
(SCOPE_IDENTITY(), 'Ferrari', 1),
(SCOPE_IDENTITY(), 'Maserati', 0),
(SCOPE_IDENTITY(), 'Alfa Romeo', 0);
GO