document.addEventListener("DOMContentLoaded", function () {
    const categoryId = window.triviaCategoryId || 1;
    const storageKey = `trivia_game_state_${categoryId}`;

    let questions = [];
    let currentIndex = 0;
    let score = 0;
    let correctCount = 0;
    let timer = null;
    let timeLeft = 30;
    let isGameOver = false;
    let isPaused = false;

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

    function shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

    function saveGameState() {
        if (isGameOver) return;
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
            try {
                const state = JSON.parse(savedState);
                questions = state.questions || [];
                currentIndex = state.currentIndex || 0;
                score = state.score || 0;
                correctCount = state.correctCount || 0;
                timeLeft = state.timeLeft || 30;

                if (currentIndex >= questions.length || questions.length === 0) {
                    sessionStorage.removeItem(storageKey);
                    startFreshGame();
                    return;
                }

                showQuestion(false);
            } catch (err) {
                console.error("Error al leer el estado guardado:", err);
                sessionStorage.removeItem(storageKey);
                startFreshGame();
            }
        } else {
            startFreshGame();
        }
    }

    function startFreshGame() {
        questions = window.triviaQuestions || [];
        currentIndex = 0;
        score = 0;
        correctCount = 0;
        isGameOver = false;
        isPaused = false;
        
        if (questions.length === 0) return;

        saveGameState();
        showQuestion(true);
    }

    function showQuestion(resetTime = true) {
        if (isGameOver || isPaused) return;

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
        
        const counterEl = document.getElementById('question-counter');
        const textEl = document.getElementById('question-text');
        
        if (counterEl) counterEl.innerText = `Pregunta ${currentIndex + 1} de ${questions.length}`;
        if (textEl) textEl.innerText = q.QuestionText;

        const answersDiv = document.getElementById('answers-container');
        const feedbackDiv = document.getElementById('feedback-container');
        
        if (answersDiv) answersDiv.innerHTML = '';
        if (feedbackDiv) feedbackDiv.innerText = '';

        const shuffledAnswers = shuffleArray([...q.Answers]);

        shuffledAnswers.forEach(ans => {
            const btn = document.createElement('button');
            btn.className = 'btn answer-option-btn text-start p-3 px-4 fs-6 fw-semibold shadow-sm';
            btn.innerText = ans.AnswerText;
            btn.onclick = () => {
                if (!isPaused && !isGameOver) {
                    selectAnswer(ans.IsCorrect, btn, shuffledAnswers);
                }
            };
            if (answersDiv) answersDiv.appendChild(btn);
        });

        saveGameState();
    }

    function startTimer() {
        if (isGameOver || isPaused) return;
        playSound(soundTick);

        timer = setInterval(() => {
            if (isGameOver || isPaused) return;

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
        
        if (!text || !badge) return;

        text.innerText = `${timeLeft}s`;
        
        if (timeLeft <= 10) {
            badge.className = 'timer-pill d-flex align-items-center gap-2 px-3 py-2 rounded-pill fw-bold text-warning bg-warning-subtle pulse-animation';
        } else {
            badge.className = 'timer-pill d-flex align-items-center gap-2 px-3 py-2 rounded-pill fw-bold text-danger bg-danger-subtle';
        }
    }

    function selectAnswer(isCorrect, selectedBtn, allAnswers) {
        if (isGameOver || isPaused) return;
        clearInterval(timer);
        stopSound(soundTick);

        const feedback = document.getElementById('feedback-container');
        const answersContainer = document.getElementById('answers-container');
        
        if (answersContainer) {
            const buttons = answersContainer.getElementsByTagName('button');
            for (let b of buttons) b.disabled = true;
        }

        if (feedback) {
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

                if (answersContainer) {
                    const buttons = answersContainer.getElementsByTagName('button');
                    for (let i = 0; i < buttons.length; i++) {
                        if (allAnswers[i].IsCorrect) {
                            buttons[i].classList.add('answer-correct');
                        }
                    }
                }
            }
        }

        currentIndex++;
        saveGameState();

        setTimeout(() => {
            if (!isPaused) {
                showQuestion(true);
            }
        }, 1600);
    }

    function handleTimeOut() {
        if (isGameOver || isPaused) return;
        stopSound(soundTick);
        playSound(soundIncorrect);
        
        const feedback = document.getElementById('feedback-container');
        if (feedback) {
            feedback.innerText = '⏰ ¡Se acabó el tiempo!';
            feedback.className = 'mt-3 text-center fw-bold fs-5 text-warning animate-fade-in';
        }

        const answersContainer = document.getElementById('answers-container');
        const q = questions[currentIndex];
        
        if (answersContainer && q) {
            const buttons = answersContainer.getElementsByTagName('button');
            for (let i = 0; i < buttons.length; i++) {
                buttons[i].disabled = true;
                if (q.Answers[i].IsCorrect) {
                    buttons[i].classList.add('answer-correct');
                }
            }
        }

        currentIndex++;
        saveGameState();

        setTimeout(() => {
            if (!isPaused) {
                showQuestion(true);
            }
        }, 1600);
    }

    function endGame() {
        isGameOver = true;
        clearInterval(timer);
        stopSound(soundTick);
        sessionStorage.removeItem(storageKey);

        const qContainer = document.getElementById('question-container');
        const timerBadge = document.getElementById('timer-badge');
        const qCounter = document.getElementById('question-counter');
        const feedbackContainer = document.getElementById('feedback-container');
        const finalScore = document.getElementById('final-score');
        const resultContainer = document.getElementById('result-container');

        if (qContainer) qContainer.classList.add('d-none');
        if (timerBadge) timerBadge.classList.add('d-none');
        if (qCounter) qCounter.classList.add('d-none');
        if (feedbackContainer) feedbackContainer.classList.add('d-none');

        if (finalScore) finalScore.innerText = score;
        if (resultContainer) resultContainer.classList.remove('d-none');

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

    // Exponer métodos globales para controlar la pausa desde la vista
    window.triviaGame = {
        pause: function() {
            if (isGameOver) return;
            isPaused = true;
            clearInterval(timer);
            stopSound(soundTick); // Pausa el sonido de tictac del reloj
            console.log("Juego pausado por advertencia.");
        },
        resume: function() {
            if (isGameOver || !isPaused) return;
            isPaused = false;
            startTimer(); // Reanuda el tictac y la cuenta regresiva desde donde se quedó
            console.log("Juego reanudado.");
        }
    };

    initGame();
});