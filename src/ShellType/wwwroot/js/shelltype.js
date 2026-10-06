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
