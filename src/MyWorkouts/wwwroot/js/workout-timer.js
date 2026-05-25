var WorkoutTimer = (function () {
    'use strict';

    var _exercises = [];
    var _els = {};
    var _idx = 0;
    var _setIdx = 0;
    var _sidePhase = null; // null | 'left' | 'right'
    var _secondsLeft = 0;
    var _phase = 'idle'; // idle | work | intraset | rest | done
    var _timerId = null;
    var _voice = null;
    var _halfAnnounced = false;
    var _startedAt = null;

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
        _els.phase.classList.add(...(cssClass || 'bg-secondary').split(' '));
        _els.phase.style.fontSize = '1rem';
    }

    function highlightExercise(ex) {
        _exercises.forEach(function (e) {
            var row = _els.listItems(e.id);
            if (row) row.classList.remove('list-group-item-primary', 'active');
        });
        if (ex) {
            var row = _els.listItems(ex.id);
            if (row) row.classList.add('list-group-item-primary');
        }
    }

    function showBtnDone(show) {
        if (_els.btnDone) {
            _els.btnDone.classList.toggle('d-none', !show);
        }
    }

    function buildInfoLabel(ex) {
        var parts = [];
        if (ex.equipment && ex.equipment !== 'Bodyweight') {
            // Convert PascalCase enum name to spaced words
            var eq = ex.equipment.replace(/([a-z])([A-Z])/g, '$1 $2');
            parts.push(eq);
        }
        if (ex.prescribedWeight) {
            parts.push(ex.prescribedWeight + '\u00a0' + (ex.weightUnit || 'lbs'));
        }
        return parts.join(' \u00b7 ');
    }

    function startPhase() {
        var ex = _exercises[_idx];
        _halfAnnounced = false;

        var sideText = '';
        if (ex.isOneSided && _sidePhase) {
            sideText = _sidePhase === 'left' ? 'left side' : 'right side';
        }

        var setLabelShort = ex.sets > 1 ? ' \u00b7 Set ' + (_setIdx + 1) + '/' + ex.sets : '';
        var sideLabelShort = sideText ? ' \u00b7 ' + (sideText === 'left side' ? 'Left' : 'Right') : '';

        _els.name.textContent = ex.name;
        if (_els.info) _els.info.textContent = buildInfoLabel(ex);
        _els.notes.textContent = ex.notes || '';
        _els.progress.textContent = 'Exercise ' + (_idx + 1) + ' of ' + _exercises.length;

        if (_phase === 'work') {
            highlightExercise(ex);

            if (ex.isRepBased) {
                _els.display.textContent = ex.reps + ' reps';
                setPhaseLabel('Reps' + setLabelShort + sideLabelShort, 'bg-primary');
                showBtnDone(true);

                var repAnn = 'Next up: ' + ex.name;
                if (sideText) repAnn += ', ' + sideText;
                repAnn += '. Do ' + ex.reps + ' reps.';
                if (ex.sets > 1) repAnn = 'Set ' + (_setIdx + 1) + ' of ' + ex.sets + '. ' + repAnn;
                speak(repAnn);
            } else {
                _secondsLeft = ex.durationSeconds;
                _els.display.textContent = formatTime(_secondsLeft);
                setPhaseLabel('Work' + setLabelShort + sideLabelShort, 'bg-primary');
                showBtnDone(false);

                var workAnn;
                if (ex.sets > 1) {
                    workAnn = 'Set ' + (_setIdx + 1) + ' of ' + ex.sets + '. ' + ex.name;
                    if (sideText) workAnn += ', ' + sideText;
                    workAnn += '. Go!';
                } else {
                    workAnn = 'Next up: ' + ex.name;
                    if (sideText) workAnn += ', ' + sideText;
                    workAnn += '. ' + ex.durationSeconds + ' seconds. Go!';
                }
                speak(workAnn);
                _timerId = setInterval(tick, 1000);
            }
        } else if (_phase === 'intraset') {
            setPhaseLabel('Rest between sets', 'bg-warning text-dark');
            _secondsLeft = ex.intraSetRestSeconds;
            _els.display.textContent = formatTime(_secondsLeft);
            showBtnDone(false);
            speak('Rest. ' + ex.intraSetRestSeconds + ' seconds.');
            _timerId = setInterval(tick, 1000);
        } else if (_phase === 'rest') {
            setPhaseLabel('Rest', 'bg-success');
            _secondsLeft = ex.restSeconds;
            _els.display.textContent = formatTime(_secondsLeft);
            showBtnDone(false);
            speak('Rest. ' + ex.restSeconds + ' seconds.');
            _timerId = setInterval(tick, 1000);
        }
    }

    function tick() {
        _secondsLeft--;
        _els.display.textContent = formatTime(_secondsLeft);

        var ex = _exercises[_idx];

        if (_phase === 'work' && !ex.isRepBased) {
            var half = Math.floor(ex.durationSeconds / 2);
            if (_secondsLeft === half && half > 5 && !_halfAnnounced) {
                _halfAnnounced = true;
                speak('Halfway!');
            }
            if (_secondsLeft <= 5 && _secondsLeft > 0) {
                speak(String(_secondsLeft));
            }
        }

        if ((_phase === 'rest' || _phase === 'intraset') && _secondsLeft === 3) {
            speak('Get ready.');
        }

        if (_secondsLeft <= 0) {
            clearInterval(_timerId);
            advance();
        }
    }

    function advance() {
        var ex = _exercises[_idx];

        if (_phase === 'work') {
            // One-sided: left side done, switch to right
            if (ex.isOneSided && _sidePhase === 'left') {
                _sidePhase = 'right';
                startPhase();
                return;
            }

            // All sides done. Check for more sets.
            if (_setIdx < ex.sets - 1) {
                _setIdx++;
                if (ex.isOneSided) _sidePhase = 'left';
                if (ex.intraSetRestSeconds > 0) {
                    _phase = 'intraset';
                }
                startPhase();
            } else {
                // All sets complete. Inter-exercise rest or next exercise.
                if (ex.restSeconds > 0) {
                    _phase = 'rest';
                    startPhase();
                } else {
                    moveNext();
                }
            }
        } else if (_phase === 'intraset') {
            _phase = 'work';
            if (ex.isOneSided) _sidePhase = 'left';
            startPhase();
        } else if (_phase === 'rest') {
            moveNext();
        }
    }

    function moveNext() {
        _idx++;
        _setIdx = 0;
        if (_idx >= _exercises.length) {
            finish();
        } else {
            var nextEx = _exercises[_idx];
            _sidePhase = nextEx.isOneSided ? 'left' : null;
            _phase = 'work';
            startPhase();
        }
    }

    function finish() {
        _phase = 'done';
        _els.display.textContent = '\uD83C\uDF89';
        _els.name.textContent = 'Workout complete!';
        if (_els.info) _els.info.textContent = '';
        _els.notes.textContent = '';
        setPhaseLabel('Done', 'bg-warning text-dark');
        _els.progress.textContent = '';
        showBtnDone(false);
        _els.btnPause.classList.add('d-none');
        _els.btnResume.classList.add('d-none');
        _els.btnStop.classList.add('d-none');
        _els.btnStart.classList.remove('d-none');
        _els.btnStart.textContent = '\u21ba Restart';
        speak('Workout complete. Well done!');
        highlightExercise(null);

        var logSection = document.getElementById('log-section');
        if (logSection) {
            logSection.classList.remove('d-none');
            var startedInput = document.getElementById('logStartedAt');
            if (startedInput && _startedAt) {
                startedInput.value = _startedAt.toISOString();
            }
        }
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
                isRepBased: e.isRepBased || false,
                durationSeconds: e.durationSeconds,
                reps: e.reps || 10,
                sets: e.sets || 1,
                intraSetRestSeconds: e.intraSetRestSeconds != null ? e.intraSetRestSeconds : 30,
                restSeconds: e.restSeconds,
                prescribedWeight: e.prescribedWeight || null,
                weightUnit: e.weightUnit || 'lbs',
                equipment: e.equipment || '',
                isOneSided: e.isOneSided || false,
                order: e.order,
                notes: e.notes || ''
            };
        });
        _els = elements;

        if (window.speechSynthesis) {
            _voice = pickVoice();
            window.speechSynthesis.onvoiceschanged = function () {
                _voice = pickVoice();
            };
        }

        _els.btnStart.addEventListener('click', function () {
            if (_phase === 'done' || _phase === 'idle') {
                clearInterval(_timerId);
                _idx = 0;
                _setIdx = 0;
                _sidePhase = _exercises.length > 0 && _exercises[0].isOneSided ? 'left' : null;
                _phase = 'work';
                _halfAnnounced = false;
                _startedAt = new Date();
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
            showBtnDone(false);
            speak('Paused.');
        });

        _els.btnResume.addEventListener('click', function () {
            _els.btnResume.classList.add('d-none');
            _els.btnPause.classList.remove('d-none');
            var ex = _exercises[_idx];
            var label = _phase === 'work' ? 'Work' : 'Rest';
            var cssClass = _phase === 'work' ? 'bg-primary'
                : (_phase === 'intraset' ? 'bg-warning text-dark' : 'bg-success');
            setPhaseLabel(label, cssClass);
            speak('Resuming.');
            if (_phase === 'work' && ex.isRepBased) {
                showBtnDone(true);
            } else {
                _timerId = setInterval(tick, 1000);
            }
        });

        _els.btnStop.addEventListener('click', function () {
            clearInterval(_timerId);
            _timerId = null;
            _phase = 'idle';
            _idx = 0;
            _setIdx = 0;
            _sidePhase = null;
            _halfAnnounced = false;
            _els.display.textContent = '\u2014';
            _els.name.textContent = '\u2014';
            if (_els.info) _els.info.textContent = '';
            _els.notes.textContent = '';
            _els.progress.textContent = 'Exercise 0 of ' + _exercises.length;
            setPhaseLabel('Ready', 'bg-secondary');
            showBtnDone(false);
            _els.btnPause.classList.add('d-none');
            _els.btnResume.classList.add('d-none');
            _els.btnStop.classList.add('d-none');
            _els.btnStart.classList.remove('d-none');
            _els.btnStart.textContent = '\u25b6 Start';
            highlightExercise(null);
            speak('Stopped.');
        });

        if (_els.btnDone) {
            _els.btnDone.addEventListener('click', function () {
                clearInterval(_timerId);
                advance();
            });
        }
    }

    return { init: init };
})();
