// Browser glue for shelltype. Blazor calls into window.shellType via IJSRuntime.
window.shellType = (() => {
    let handler = null;

    const isFormField = (el) =>
        !!el && (el.tagName === "INPUT" || el.tagName === "TEXTAREA" || el.tagName === "SELECT" || el.isContentEditable);

    function onKeyDown(e) {
        if (!handler || e.isComposing || isFormField(e.target) || e.metaKey || e.altKey) {
            return;
        }

        if (e.ctrlKey) {
            if (e.key === "Backspace") {
                e.preventDefault();
                handler.invokeMethodAsync("OnKey", "DeleteWord");
            }
            return;
        }

        const key = e.key;
        if (key === "Tab" || key === "Enter" || key === "Backspace" || key === "Escape" || key.length === 1) {
            e.preventDefault();
            handler.invokeMethodAsync("OnKey", key);
        }
    }

    document.addEventListener("mousemove", () => document.body.classList.remove("is-typing"));
    window.addEventListener("resize", () => window.shellType.ui.moveCaret());

    function onBlur() {
        handler?.invokeMethodAsync("OnFocusChanged", false);
    }

    function onFocus() {
        handler?.invokeMethodAsync("OnFocusChanged", true);
    }

    return {
        // localStorage can throw (private mode, blocked storage), so every access is guarded.
        storage: {
            get(key) {
                try { return localStorage.getItem(key); } catch { return null; }
            },
            set(key, value) {
                try { localStorage.setItem(key, value); } catch { }
            },
            remove(key) {
                try { localStorage.removeItem(key); } catch { }
            },
        },
        ui: {
            setTheme(name) {
                document.documentElement.dataset.theme = name;
            },
            setCssVar(name, value) {
                document.documentElement.style.setProperty(name, value);
            },
            // Glide the smooth caret to the character marked .caret. Big jumps (a new
            // command, a restart) snap instead of sweeping across the line.
            moveCaret() {
                const caret = document.querySelector(".terminal .smooth-caret");
                const target = document.querySelector(".terminal .line.current .char.caret");
                if (!caret || !target) {
                    return;
                }

                const box = caret.parentElement.getBoundingClientRect();
                const rect = target.getBoundingClientRect();
                const x = rect.left - box.left;
                const y = rect.top - box.top;
                const last = caret._pos;
                const far = !last || Math.abs(x - last.x) > rect.width * 6 || y !== last.y && x > last.x;

                if (far) {
                    caret.style.transition = "none";
                }
                caret.style.setProperty("--cw", rect.width + "px");
                caret.style.setProperty("--ch", rect.height + "px");
                caret.style.transform = `translate(${x}px, ${y}px)`;
                caret._pos = { x, y };
                if (far) {
                    caret.getBoundingClientRect(); // commit the jump before re-enabling the transition
                    caret.style.transition = "";
                }
            },
            // While typing, hide the header/footer. Moving the mouse brings them back.
            setTyping(on) {
                document.body.classList.toggle("is-typing", on);
            },
        },
        // Tiny synthesized clicks, so there are no audio files to download.
        sound: {
            ctx: null,
            play(correct) {
                try {
                    const ctx = (this.ctx ??= new AudioContext());
                    const osc = ctx.createOscillator();
                    const gain = ctx.createGain();
                    osc.type = correct ? "triangle" : "square";
                    osc.frequency.value = correct ? 1400 + Math.random() * 200 : 180;
                    gain.gain.setValueAtTime(correct ? 0.05 : 0.04, ctx.currentTime);
                    gain.gain.exponentialRampToValueAtTime(0.0001, ctx.currentTime + (correct ? 0.03 : 0.08));
                    osc.connect(gain).connect(ctx.destination);
                    osc.start();
                    osc.stop(ctx.currentTime + 0.09);
                } catch { }
            },
        },
        keyboard: {
            register(dotNetRef) {
                handler = dotNetRef;
                document.addEventListener("keydown", onKeyDown);
                window.addEventListener("blur", onBlur);
                window.addEventListener("focus", onFocus);
            },
            unregister() {
                handler = null;
                document.removeEventListener("keydown", onKeyDown);
                window.removeEventListener("blur", onBlur);
                window.removeEventListener("focus", onFocus);
            },
        },
    };
})();
