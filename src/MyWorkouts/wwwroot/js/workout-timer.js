var WorkoutTimer = (function () {
    'use strict';

    var _exercises = [];
    var _els = {};
    var _idx = 0;
    var _secondsLeft = 0;
    var _phase = 'idle'; // idle | work | rest | done
    var _timerId = null;
    var _voice = null;

    function pickVoice() {
        if (!window.speechSynthesis) return null;
        var voices = window.speechSynthesis.getVoices();
        if (!voices.length) return null;

        // 1. en-AU female — prefer Karen / Catherine
        var preferred = voices.filter(function (v) {
            return v.lang === 'en-AU' && /karen|catherine/i.test(v.name);
        });
        if (preferred.length) return preferred[0];

        // 2. en-GB female
        var gbFemale = voices.filter(function (v) {
            return v.lang === 'en-GB' && !/male/i.test(v.name);
        });
        if (gbFemale.length) return gbFemale[0];

        // 3. any en-AU
        var auAny = voices.filter(function (v) { return v.lang === 'en-AU'; });
        if (auAny.length) return auAny[0];

        // 4. any en-GB
        var gbAny = voices.filter(function (v) { return v.lang === 'en-GB'; });
        if (gbAny.length) return gbAny[0];

        // 5. any en-*
        var enAny = voices.filter(function (v) { return /^en/i.test(v.lang); });
        if (enAny.length) return enAny[0];

        return null;
    }

    function speak(text) {
        if (!window.speechSynthesis) return;
        var utt = new SpeechSynthesisUtterance(text);
        if (_voice) utt.voice = _voice;
        utt.rate = 0.95;
        utt.pitch = 1.1;
        window.speechSynthesis.cancel();
        window.speechSynthesis.speak(utt);
    }

    function formatTime(s) {
        var m = Math.floor(s / 60);
        var sec = s % 60;
        return m > 0
            ? m + ':' + (sec < 10 ? '0' : '') + sec
            : sec + 's';
    }

    function setPhaseLabel(label, cssClass) {
        _els.phase.textContent = label;
        _els.phase.className = 'badge mb-2';
        _els.phase.classList.add(cssClass || 'bg-secondary');
        _els.phase.style.fontSize = '1rem';
    }

    function highlightExercise(ex) {
        // Remove previous highlight
        _exercises.forEach(function (e) {
            var row = _els.listItems(e.id);
            if (row) row.classList.remove('list-group-item-primary', 'active');
        });
        if (ex) {
            var row = _els.listItems(ex.id);
            if (row) row.classList.add('list-group-item-primary');
        }
    }

    function startPhase() {
        var ex = _exercises[_idx];

        if (_phase === 'work') {
            _els.name.textContent = ex.name;
            _els.notes.textContent = ex.notes || '';
            _secondsLeft = ex.durationSeconds;
            setPhaseLabel('Work', 'bg-primary');
            _els.progress.textContent = 'Exercise ' + (_idx + 1) + ' of ' + _exercises.length;
            highlightExercise(ex);
            speak('Next up: ' + ex.name + '. ' + ex.durationSeconds + ' seconds. Go!');
        } else if (_phase === 'rest') {
            setPhaseLabel('Rest', 'bg-success');
            _secondsLeft = ex.restSeconds;
            speak('Rest. ' + ex.restSeconds + ' seconds.');
        }

        _els.display.textContent = formatTime(_secondsLeft);
        _timerId = setInterval(tick, 1000);
    }

    function tick() {
        _secondsLeft--;
        _els.display.textContent = formatTime(_secondsLeft);

        if (_secondsLeft === 3 && _phase === 'rest') {
            speak('Get ready.');
        }

        if (_secondsLeft <= 0) {
            clearInterval(_timerId);
            advance();
        }
    }

    function advance() {
        if (_phase === 'work') {
            var ex = _exercises[_idx];
            if (ex.restSeconds > 0) {
                _phase = 'rest';
                startPhase();
            } else {
                // no rest — move to next exercise
                moveNext();
            }
        } else if (_phase === 'rest') {
            moveNext();
        }
    }

    function moveNext() {
        _idx++;
        if (_idx >= _exercises.length) {
            finish();
        } else {
            _phase = 'work';
            startPhase();
        }
    }

    function finish() {
        _phase = 'done';
        _els.display.textContent = '🎉';
        _els.name.textContent = 'Workout complete!';
        _els.notes.textContent = '';
        setPhaseLabel('Done', 'bg-warning text-dark');
        _els.progress.textContent = '';
        _els.btnPause.classList.add('d-none');
        _els.btnResume.classList.add('d-none');
        _els.btnStop.classList.add('d-none');
        _els.btnStart.classList.remove('d-none');
        _els.btnStart.textContent = '↺ Restart';
        speak('Workout complete. Well done!');
        highlightExercise(null);
    }

    function showRunning() {
        _els.btnStart.classList.add('d-none');
        _els.btnPause.classList.remove('d-none');
        _els.btnResume.classList.add('d-none');
        _els.btnStop.classList.remove('d-none');
    }

    function init(exercises, elements) {
        _exercises = exercises.map(function (e) {
            return {
                id: e.id,
                name: e.name,
                durationSeconds: e.durationSeconds,
                restSeconds: e.restSeconds,
                order: e.order,
                notes: e.notes || ''
            };
        });
        _els = elements;

        // Load voices (may be async)
        if (window.speechSynthesis) {
            _voice = pickVoice();
            window.speechSynthesis.onvoiceschanged = function () {
                _voice = pickVoice();
            };
        }

        _els.btnStart.addEventListener('click', function () {
            if (_phase === 'done' || _phase === 'idle') {
                // (re)start
                clearInterval(_timerId);
                _idx = 0;
                _phase = 'work';
                showRunning();
                startPhase();
            }
        });

        _els.btnPause.addEventListener('click', function () {
            clearInterval(_timerId);
            _timerId = null;
            _els.btnPause.classList.add('d-none');
            _els.btnResume.classList.remove('d-none');
            setPhaseLabel('Paused', 'bg-warning text-dark');
            speak('Paused.');
        });

        _els.btnResume.addEventListener('click', function () {
            _els.btnResume.classList.add('d-none');
            _els.btnPause.classList.remove('d-none');
            setPhaseLabel(_phase === 'work' ? 'Work' : 'Rest', _phase === 'work' ? 'bg-primary' : 'bg-success');
            speak('Resuming.');
            _timerId = setInterval(tick, 1000);
        });

        _els.btnStop.addEventListener('click', function () {
            clearInterval(_timerId);
            _timerId = null;
            _phase = 'idle';
            _idx = 0;
            _els.display.textContent = '—';
            _els.name.textContent = '—';
            _els.notes.textContent = '';
            _els.progress.textContent = 'Exercise 0 of ' + _exercises.length;
            setPhaseLabel('Ready', 'bg-secondary');
            _els.btnPause.classList.add('d-none');
            _els.btnResume.classList.add('d-none');
            _els.btnStop.classList.add('d-none');
            _els.btnStart.classList.remove('d-none');
            _els.btnStart.textContent = '▶ Start';
            highlightExercise(null);
            speak('Stopped.');
        });
    }

    return { init: init };
})();
