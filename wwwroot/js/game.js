document.addEventListener("DOMContentLoaded", function () {
    const categoryId = window.triviaCategoryId || 1;
    const storageKey = `trivia_game_state_${categoryId}`;

    let questions = [];
    let currentIndex = 0;
    let score = 0;
    let correctCount = 0;
    let timer = null;
    let timeLeft = 30; // 30 segundos por pregunta

    // Efectos de sonido
    const soundCorrect = new Audio('/audio/correcto.wav');
    const soundIncorrect = new Audio('/audio/incorrecto.wav');
    const soundTick = new Audio('/audio/tictac.wav');
    soundTick.loop = true;

    function playSound(audioObj) {
        audioObj.pause();
        audioObj.currentTime = 0;
        audioObj.play().catch(e => console.log("Reproducción de audio bloqueada:", e));
    }

    function stopSound(audioObj) {
        audioObj.pause();
        audioObj.currentTime = 0;
    }

    // Función para mezclar aleatoriamente las respuestas (Algoritmo Fisher-Yates)
    function shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

    function saveGameState() {
        const state = {
            questions: questions,
            currentIndex: currentIndex,
            score: score,
            correctCount: correctCount,
            timeLeft: timeLeft
        };
        sessionStorage.setItem(storageKey, JSON.stringify(state));
    }

    function initGame() {
        const savedState = sessionStorage.getItem(storageKey);

        if (savedState) {
            const state = JSON.parse(savedState);
            questions = state.questions;
            currentIndex = state.currentIndex;
            score = state.score;
            correctCount = state.correctCount;
            timeLeft = state.timeLeft || 30;

            if (currentIndex >= questions.length) {
                sessionStorage.removeItem(storageKey);
                startFreshGame();
                return;
            }

            showQuestion(false);
        } else {
            startFreshGame();
        }
    }

    function startFreshGame() {
        questions = window.triviaQuestions || [];
        currentIndex = 0;
        score = 0;
        correctCount = 0;
        
        if (questions.length === 0) return;

        saveGameState();
        showQuestion(true);
    }

    function showQuestion(resetTime = true) {
        if (currentIndex >= questions.length) {
            endGame();
            return;
        }

        clearInterval(timer);
        stopSound(soundTick);

        if (resetTime) {
            timeLeft = 30;
        }
        
        updateTimerDisplay();
        startTimer();

        const q = questions[currentIndex];
        document.getElementById('question-counter').innerText = `Pregunta ${currentIndex + 1} de ${questions.length}`;
        document.getElementById('question-text').innerText = q.QuestionText;

        const answersDiv = document.getElementById('answers-container');
        answersDiv.innerHTML = '';
        document.getElementById('feedback-container').innerText = '';

        // Creamos una copia de las respuestas y las barajamos aleatoriamente
        const shuffledAnswers = shuffleArray([...q.Answers]);

        shuffledAnswers.forEach(ans => {
            const btn = document.createElement('button');
            btn.className = 'btn answer-option-btn text-start p-3 px-4 fs-6 fw-semibold shadow-sm';
            btn.innerText = ans.AnswerText;
            btn.onclick = () => selectAnswer(ans.IsCorrect, btn, shuffledAnswers);
            answersDiv.appendChild(btn);
        });

        saveGameState();
    }

    function startTimer() {
        playSound(soundTick);

        timer = setInterval(() => {
            timeLeft--;
            updateTimerDisplay();
            saveGameState();

            if (timeLeft <= 0) {
                clearInterval(timer);
                stopSound(soundTick);
                handleTimeOut();
            }
        }, 1000);
    }

    function updateTimerDisplay() {
        const badge = document.getElementById('timer-badge');
        const text = document.getElementById('timer-text');
        text.innerText = `${timeLeft}s`;
        
        if (timeLeft <= 10) {
            badge.className = 'timer-pill d-flex align-items-center gap-2 px-3 py-2 rounded-pill fw-bold text-warning bg-warning-subtle pulse-animation';
        } else {
            badge.className = 'timer-pill d-flex align-items-center gap-2 px-3 py-2 rounded-pill fw-bold text-danger bg-danger-subtle';
        }
    }

    function selectAnswer(isCorrect, selectedBtn, allAnswers) {
        clearInterval(timer);
        stopSound(soundTick);

        const feedback = document.getElementById('feedback-container');
        const buttons = document.getElementById('answers-container').getElementsByTagName('button');
        
        for (let b of buttons) b.disabled = true;

        if (isCorrect) {
            selectedBtn.classList.add('answer-correct');
            feedback.innerText = '✨ ¡Respuesta Correcta!';
            feedback.className = 'mt-3 text-center fw-bold fs-5 text-success animate-fade-in';
            score += 10;
            correctCount++;
            playSound(soundCorrect);
        } else {
            selectedBtn.classList.add('answer-incorrect');
            feedback.innerText = '❌ Respuesta Incorrecta';
            feedback.className = 'mt-3 text-center fw-bold fs-5 text-danger animate-fade-in';
            playSound(soundIncorrect);

            for (let i = 0; i < buttons.length; i++) {
                if (allAnswers[i].IsCorrect) {
                    buttons[i].classList.add('answer-correct');
                }
            }
        }

        currentIndex++;
        saveGameState();

        setTimeout(() => {
            showQuestion(true);
        }, 1600);
    }

    function handleTimeOut() {
        stopSound(soundTick);
        playSound(soundIncorrect);
        
        const feedback = document.getElementById('feedback-container');
        feedback.innerText = '⏰ ¡Se acabó el tiempo!';
        feedback.className = 'mt-3 text-center fw-bold fs-5 text-warning animate-fade-in';

        const buttons = document.getElementById('answers-container').getElementsByTagName('button');
        const q = questions[currentIndex];
        
        for (let i = 0; i < buttons.length; i++) {
            buttons[i].disabled = true;
            if (q.Answers[i].IsCorrect) {
                buttons[i].classList.add('answer-correct');
            }
        }

        currentIndex++;
        saveGameState();

        setTimeout(() => {
            showQuestion(true);
        }, 1600);
    }

    function endGame() {
        stopSound(soundTick);
        sessionStorage.removeItem(storageKey);

        document.getElementById('question-container').classList.add('d-none');
        document.getElementById('timer-badge').classList.add('d-none');
        document.getElementById('question-counter').classList.add('d-none');
        document.getElementById('feedback-container').classList.add('d-none');

        document.getElementById('final-score').innerText = score;
        document.getElementById('result-container').classList.remove('d-none');

        // Petición AJAX que ahora sí se enviará con éxito a la base de datos
        fetch('/Trivia/SaveResult', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                categoryId: categoryId,
                pointsEarned: score,
                correctAnswersCount: correctCount
            })
        })
        .then(res => res.json())
        .then(data => console.log("Puntaje guardado con éxito en el ranking:", data))
        .catch(err => console.error("Error al guardar puntuación:", err));
    }

    initGame();
});